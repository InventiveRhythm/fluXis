#define SAMPLES 20

layout(std140, set = 0, binding = 0) uniform m_BlurParameters
{
    vec2 g_TexSize;
    vec2 g_Pos;
    float g_Sigma;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 2) in vec2 v_TexCoord;
layout(location = 0) out vec4 o_Colour;

mediump float dither(vec2 uv)
{
    return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453) - 0.5;
}

void main()
{
    vec2 uv = v_TexCoord;
    vec2 delta = uv - g_Pos;

    float aspect = g_TexSize.x / g_TexSize.y;
    float invAspect = 1.0 / aspect;
    delta.x *= aspect;

    float angleStep = ((g_Sigma - 0.5) * 4.0) / float(SAMPLES);

    float jitter = dither(uv) * angleStep;
    float sinJitter = sin(jitter);
    float cosJitter = cos(jitter);

    vec2 rotated = vec2(
        cosJitter * delta.x - sinJitter * delta.y,
        sinJitter * delta.x + cosJitter * delta.y
    );

    float sinStep = sin(angleStep);
    float cosStep = cos(angleStep);

    vec4 color = vec4(0.0);
    float validSamples = 0.0;

    for (int i = 0; i < SAMPLES; ++i)
    {
        vec2 sampleUV = g_Pos + vec2(rotated.x * invAspect, rotated.y);

        vec2 inside = step(vec2(0.0), sampleUV) * step(sampleUV, vec2(1.0));
        float factor = inside.x * inside.y;

        color += texture(sampler2D(m_Texture, m_Sampler), sampleUV) * factor;
        validSamples += factor;

        rotated = vec2(
            cosStep * rotated.x - sinStep * rotated.y,
            sinStep * rotated.x + cosStep * rotated.y
        );
    }

    o_Colour = (validSamples > 0.0)
        ? color / validSamples
        : texture(sampler2D(m_Texture, m_Sampler), uv);
}