layout(std140, set = 0, binding = 0) uniform m_FishEyeParameters
{
    vec2 g_TexSize;
    float g_Strength;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

#ifndef PI
#define PI 3.141593
#endif

vec2 FishEyeUV(vec2 uv)
{
    vec2 center = vec2(0.5);
    float corner = length(center);
    vec2 d = uv - 0.5;
    float r = length(d);

    if (g_Strength > 0.0)
    {
        float fac = g_Strength * PI * 0.5;
        uv = vec2(0.5) + normalize(d) * tan(r * fac) * corner / tan(corner * fac);
    }
    else
    {
        float fac = tan(g_Strength) * PI * 2;
        uv = vec2(0.5) + normalize(d) * atan(r * -fac) * 0.5 / atan (0.5 * -fac);
    }
    return uv;
}
void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;

    vec4 colour = texture(sampler2D(m_Texture, m_Sampler), FishEyeUV(uv));
    
    o_Colour = colour;
}