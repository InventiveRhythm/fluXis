layout(std140, set = 0, binding = 0) uniform m_MosaicParameters
{
    vec2 g_TexSize;
    float g_Strength; // 0 means no effect, 1 means 1x1 pixel blocks
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;
    float blockSize = g_TexSize.x * (1.0 - g_Strength);
    vec2 blockPos = floor(uv * blockSize) / blockSize;
    o_Colour = texture(sampler2D(m_Texture, m_Sampler), blockPos);
}
