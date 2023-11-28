layout(std140, set = 0, binding = 0) uniform m_ChromaticParameters
{
	mediump vec2 g_TexSize;
	mediump float g_Strength;
};

layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void)
{
    mediump vec2 uv = gl_FragCoord.xy / g_TexSize;
    mediump vec4 colour = texture(sampler2D(m_Texture, m_Sampler), uv);

    mediump vec2 offset = vec2(0.001, 0) * g_Strength;
    colour.r = texture(sampler2D(m_Texture, m_Sampler), uv + offset).r;
    colour.b = texture(sampler2D(m_Texture, m_Sampler), uv - offset).b;

    o_Colour = colour;
}