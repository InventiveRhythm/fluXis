---@meta

---@class AudioAnalyzer
AudioAnalyzer = {}

---@param startTime number
---@param endTime number
---@param interval number
---@param amplitudeCount number?
---@param parameters FFTParameters?
---@return table
---@nodiscard
function AudioAnalyzer:AmplitudesInRange(startTime, endTime, interval, amplitudeCount, parameters) end

---@class FFTParameters
---@field attack number How fast the amplitude values rise.             Lower values (e.g. 0.50): Slower rise and looks slightly delayed.Higher values (e.g. 0.95): Instant and snappy that closely resembles the raw audio.
---@field releaseLow number How fast the lows (Bass) fall back down after a sound stops.             Lower values (e.g. 0.05): Slow decay, causing bass peaks to linger.Higher values (e.g. 0.40): Fast decay, making kicks and bass end sharply and cleanly.
---@field releaseHigh number How fast the highs (Treble) fall back down after a sound stops.             Lower values (e.g. 0.20): Smoothes out rapidly fluctuating high frequencies (like cymbals).Higher values (e.g. 0.80): Fast and highly reactive. Can cause jitter if too high.
---@field gamma number Exponentiates the final amplitudes (value^gamma). This is essentially the contrast.             Lower values (< 1.0): Bows the curve upwards. Boosts quiet noise to be more visible.Higher values (> 2.0): Bows the curve downwards. Squashes quiet noise and isolates only the loudest peaks.
---@field spatialWindowSize number How many adjacent bins to average AKA. The smoothness of the final curves.             Lower values (1): No smoothing. Represents exact frequencies but can produce jittery amplitudes.Higher values (> 5): Blends the adjacent curves into a smoother overall shape.
---@field bassCutoff number The point on the frequency spectrum where the "Low (Bass)" ends and "Mid" begins. Spectrum range: [0.0 to 1.0].
---@field midCutoff number The point on the frequency spectrum where the "Mid" band ends and "High (Treble)" begins. Spectrum range: [0.0 to 1.0].
---@field bassMultiplier number How much to boost the bass frequency band.             Lower values (< 1.0): Lowers the intensity of kicks and bass.Higher values (> 1.0): Increases the intensity of kicks and bass.
---@field midMultiplier number How much to boost the mid frequency band.             Lower values (< 1.0): Lowers the intensity of vocals, synths, and guitars.Higher values (> 1.0): Increases the intensity of vocals, synths, and guitars.
---@field highMultiplier number How much to boost the high frequency band.             Lower values (< 1.0): Lowers the intensity of hi-hats, cymbals, and treble.Higher values (> 1.0): Increases the intensity of hi-hats, cymbals, and treble.
---@field baseFloor number The minimum peak volume for the bass frequency band. Prevents auto normalization from amplifying pure silence.             Lower values (e.g. 0.1): Quiet bass gets scaled up aggressively.Higher values (e.g. 0.4): Only loud bass hits register strongly.
---@field midFloor number The minimum peak volume for the mid frequency band.             Lower values (e.g. 0.1): Quiet mids get scaled up aggressively.Higher values (e.g. 0.4): Only loud mids register strongly.
---@field highFloor number The minimum peak volume for the high frequency band.             Lower values (e.g. 0.1): Quiet highs get scaled up aggressively.Higher values (e.g. 0.4): Only loud highs register strongly.
---@field maxAdaptationRate number How fast the dynamic auto-leveling normalizes the current volume of the track.             Lower values (e.g. 0.01): Slow adaptation. Preserves contrast between quiet and loud sections.Higher values (e.g. 0.15): Fast adaptation. Keeps output intensity roughly consistent regardless of song volume.
FFTParameters = {}

---@type FFTParameters
---@diagnostic disable-next-line: missing-fields
FFTParameters.Default = {}

---@type FFTParameters
---@diagnostic disable-next-line: missing-fields
FFTParameters.Reactive = {}

---@type FFTParameters
---@diagnostic disable-next-line: missing-fields
FFTParameters.Smooth = {}

---@class FFTBands
---@field low number
---@field mid number
---@field high number
---@field total number
FFTBands = {}

---@return number
---@nodiscard
function FFTBands:GetDominantBand() end

---@class FFTFrame
---@field amplitudes number[]
---@field bands FFTBands
FFTFrame = {}

---@param threshold number
---@return boolean
---@nodiscard
function FFTFrame:IsSilent(threshold) end

---@param threshold number
---@return boolean
---@nodiscard
function FFTFrame:DetectBeat(threshold) end

---@return number
---@nodiscard
function FFTFrame:GetPeakAmplitude() end

---@return number
---@nodiscard
function FFTFrame:GetAverageAmplitude() end

---@return number
---@nodiscard
function FFTFrame:GetPeakFrequencyBin() end