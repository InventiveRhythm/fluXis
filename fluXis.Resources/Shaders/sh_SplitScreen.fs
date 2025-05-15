layout(std140, set = 0, binding = 0) uniform m_SplitScreenParameters
{
    vec2 g_TexSize;
    vec2 g_SplitInv;
    float g_Strength;
    int g_SplitsX;
    int g_SplitsY;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;


void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;
    vec2 quad = trunc(uv * vec2(g_SplitsX, g_SplitsY));

    vec2 zoom = mix (vec2(1.0), g_SplitInv, vec2(g_Strength));
    vec2 offset = mix (vec2(0.0), quad * g_SplitInv, vec2(g_Strength));

    vec2 uvQuad = (uv - offset) / zoom;
    vec4 colour = texture(sampler2D(m_Texture, m_Sampler), uvQuad);
    
    o_Colour = colour;
}