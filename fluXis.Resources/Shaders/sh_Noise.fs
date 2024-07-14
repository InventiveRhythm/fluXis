layout(std140, set = 0, binding = 0) uniform m_NoiseParameters
{
    vec2 g_TexSize;
    float g_Strength;
    float g_Time;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

highp float random(highp vec2 st)
{
    return fract(sin(dot(st.xy, vec2(g_Time, 78.233))) * 43758.5453123);
}

void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;
    vec4 colour = texture(sampler2D(m_Texture, m_Sampler), uv);
    
    float rng = random(uv);
    vec3 rngColor = vec3(rng, rng, rng);
    
    o_Colour = vec4(mix(colour.rgb, rngColor, g_Strength), colour.a);
}
