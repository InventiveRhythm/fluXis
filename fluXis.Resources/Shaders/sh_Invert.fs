layout(std140, set = 0, binding = 0) uniform m_InvertParameters
{
    vec2 g_TexSize;
    float g_Strength;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;
    vec4 colour = texture(sampler2D(m_Texture, m_Sampler), uv);
    
    vec3 inverted = 1.0 - colour.rgb;
    o_Colour = vec4(mix(colour.rgb, inverted, g_Strength), colour.a);
}
