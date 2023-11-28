layout(std140, set = 0, binding = 0) uniform m_GreyscaleParameters
{
    vec2 g_TexSize;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;
    vec4 colour = texture(sampler2D(m_Texture, m_Sampler), uv);
    
    float total = colour.r + colour.g + colour.b;
    total /= 3;
    o_Colour = vec4(total, total, total, 1.0);
}
