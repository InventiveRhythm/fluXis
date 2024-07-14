layout(std140, set = 0, binding = 0) uniform m_VignetteParameters
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
    
    uv = uv * (vec2(1.0) - uv.yx);
    float vig = uv.x * uv.y * (1 - g_Strength);
    vig = pow(vig, g_Strength);
    o_Colour = mix(colour, vec4(0, 0, 0, 1), 1.0 - vig);
}
