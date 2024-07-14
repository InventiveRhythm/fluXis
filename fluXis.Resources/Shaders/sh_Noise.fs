layout(std140, set = 0, binding = 0) uniform m_NoiseParameters
{
    vec2 g_TexSize;
    float g_Strength;
    float g_Time;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

highp float random(highp vec2 st, float size)
{
    st = floor(st * size) / size;
    
    if (st.x == 0) // to avoid a column of non-changing pixels
        st = vec2(0.4, st.y);
    
    return fract(sin(dot(st.xy, vec2(g_Time, 78.233))) * 43758.5453123);
}

void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;
    vec4 colour = texture(sampler2D(m_Texture, m_Sampler), uv);
    
    float ratio = g_TexSize.x / g_TexSize.y;
    
    float rng = random(vec2(uv.x * ratio, uv.y), g_TexSize.y / 2);
    rng *= 0.75;
    vec3 rngColor = vec3(rng, rng, rng);
    
    o_Colour = vec4(mix(colour.rgb, rngColor, g_Strength), colour.a);
}
