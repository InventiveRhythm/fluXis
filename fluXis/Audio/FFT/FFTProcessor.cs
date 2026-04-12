using System;
using fluXis.Audio.FFT.Structures.Data;
using fluXis.Audio.FFT.Structures.Processor;
using JetBrains.Annotations;
using osu.Framework.Utils;

namespace fluXis.Audio.FFT;

/// <summary>
/// A highly customizable post-processor for FFT data. How this data is processed is configured by <see cref="FFTParameters"/>.
/// </summary>
public class FFTProcessor
{
    [UsedImplicitly]
    public FFTParameters Parameters { get; set; }

    private float bassMax = 0.5f;
    private float midMax = 0.4f;
    private float highMax = 0.3f;

    private float[] envelope = [];
    private float[] buf = [];
    private float[] tmpMel = [];
    private float[] tmpSmooth = [];

    private int[] melLo = [];
    private int[] melHi = [];
    private float[] melFrac = [];
    private float[] releaseFactors = [];

    private int currentBinCount = 0;

    public FFTProcessor(FFTParameters? parameters = null)
    {
        Parameters = parameters ?? FFTParameters.Default;
    }

    public FFTFrame[] Process(ReadOnlySpan<FFTFrame> rawFrames)
    {
        if (rawFrames.Length == 0) return Array.Empty<FFTFrame>();

        int binCount = rawFrames[0].Amplitudes.Length;
        ensureCapacity(binCount);

        var result = new FFTFrame[rawFrames.Length];

        for (int f = 0; f < rawFrames.Length; f++)
        {
            var src = rawFrames[f].Amplitudes;
            Array.Copy(src, buf, binCount);

            applyModifiers();

            result[f] = new FFTFrame((float[])buf.Clone(), rawFrames[f].Bands);
        }

        return result;
    }

    public ReadOnlySpan<float> Process(ReadOnlySpan<float> rawAmplitudes)
    {
        int binCount = rawAmplitudes.Length;
        ensureCapacity(binCount);

        rawAmplitudes.CopyTo(buf);

        applyModifiers();

        return buf;
    }

    public ReadOnlySpan<float> Process(float[] rawAmplitudes)
        => Process((ReadOnlySpan<float>)rawAmplitudes);

    public FFTFrame ProcessFrame(FFTFrame raw)
    {
        int binCount = raw.Amplitudes.Length;
        ensureCapacity(binCount);

        Array.Copy(raw.Amplitudes, buf, binCount);

        applyModifiers();

        return new FFTFrame((float[])buf.Clone(), raw.Bands);
    }

    private void applyModifiers()
    {
        warpToMelScale();
        applyDynamics();
        applyEnvelope();
        applySpatialSmoothing();
    }

    private void applyDynamics()
    {
        float bMax = 0.01f, mMax = 0.01f, hMax = 0.01f;

        float bCutoffBins = currentBinCount * Parameters.BassCutoff;
        float mCutoffBins = currentBinCount * Parameters.MidCutoff;

        for (int i = 0; i < currentBinCount; i++)
        {
            float val = buf[i];
            if (i < bCutoffBins) bMax = MathF.Max(bMax, val);
            else if (i < mCutoffBins) mMax = MathF.Max(mMax, val);
            else hMax = MathF.Max(hMax, val);
        }

        float adapt = Parameters.MaxAdaptationRate;
        float keep = 1f - adapt;

        bassMax = bassMax * keep + MathF.Max(bMax, Parameters.BassFloor) * adapt;
        midMax = midMax * keep + MathF.Max(mMax, Parameters.MidFloor) * adapt;
        highMax = highMax * keep + MathF.Max(hMax, Parameters.HighFloor) * adapt;

        for (int i = 0; i < currentBinCount; i++)
        {
            float t = (float)i / (currentBinCount - 1);

            float norm;
            float multiplier;

            if (t < Parameters.BassCutoff)
            {
                float progress = t / Parameters.BassCutoff;
                norm = (float)Interpolation.Lerp(bassMax, midMax, progress);
                multiplier = (float)Interpolation.Lerp(Parameters.BassMultiplier, Parameters.MidMultiplier, progress);
            }
            else if (t < Parameters.MidCutoff)
            {
                float progress = (t - Parameters.BassCutoff) / (Parameters.MidCutoff - Parameters.BassCutoff);
                norm = (float)Interpolation.Lerp(midMax, highMax, progress);
                multiplier = (float)Interpolation.Lerp(Parameters.MidMultiplier, Parameters.HighMultiplier, progress);
            }
            else
            {
                norm = highMax;
                multiplier = Parameters.HighMultiplier;
            }

            buf[i] = (buf[i] / norm) * multiplier;

            buf[i] = MathF.Max(0, buf[i] - 0.02f);
            buf[i] = MathF.Pow(Math.Clamp(buf[i], 0f, 1f), Parameters.Gamma);
        }
    }

    private void warpToMelScale()
    {
        for (int i = 0; i < currentBinCount; i++)
            tmpMel[i] = buf[melLo[i]] + melFrac[i] * (buf[melHi[i]] - buf[melLo[i]]);

        Array.Copy(tmpMel, buf, currentBinCount);
    }

    private void applyEnvelope()
    {
        float attack = Parameters.Attack;

        for (int i = 0; i < currentBinCount; i++)
        {
            float factor = buf[i] > envelope[i] ? attack : releaseFactors[i];
            envelope[i] = buf[i] = envelope[i] + (buf[i] - envelope[i]) * factor;
        }
    }

    private void applySpatialSmoothing()
    {
        int windowSize = Parameters.SpatialWindowSize;
        if (windowSize <= 1) return;

        int half = windowSize / 2;
        float sum = 0;
        int count = 0;

        for (int i = 0; i <= half && i < currentBinCount; i++)
        {
            sum += buf[i];
            count++;
        }

        for (int i = 0; i < currentBinCount; i++)
        {
            tmpSmooth[i] = sum / count;

            int trailingIdx = i - half;
            int leadingIdx = i + half + 1;

            if (trailingIdx >= 0)
            {
                sum -= buf[trailingIdx];
                count--;
            }

            if (leadingIdx >= currentBinCount) continue;

            sum += buf[leadingIdx];
            count++;
        }

        Array.Copy(tmpSmooth, buf, currentBinCount);
    }

    private void ensureCapacity(int binCount)
    {
        if (currentBinCount == binCount) return;

        currentBinCount = binCount;

        envelope = new float[binCount];
        buf = new float[binCount];
        tmpMel = new float[binCount];
        tmpSmooth = new float[binCount];

        melLo = new int[binCount];
        melHi = new int[binCount];
        melFrac = new float[binCount];
        releaseFactors = new float[binCount];

        const float max_freq = 22050f;
        float maxMel = hzToMel(max_freq);
        float releaseLow = Parameters.ReleaseLow;
        float releaseDiff = Parameters.ReleaseHigh - releaseLow;

        for (int i = 0; i < binCount; i++)
        {
            float mel = maxMel * i / (binCount - 1);
            float pos = melToHz(mel) / max_freq * (binCount - 1);

            melLo[i] = (int)pos;
            melHi[i] = Math.Min(melLo[i] + 1, binCount - 1);
            melFrac[i] = pos - melLo[i];

            releaseFactors[i] = releaseLow + ((float)i / binCount * releaseDiff);
        }
    }

    private static float hzToMel(float hz) => 2595f * MathF.Log10(1f + hz / 700f);
    private static float melToHz(float mel) => 700f * (MathF.Pow(10f, mel / 2595f) - 1f);
}
