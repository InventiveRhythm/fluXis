layout(std140, set = 0, binding = 0) uniform m_ReflectionsParameters
{
    vec2 g_TexSize;
    float g_Strength;
    float g_Scale;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

const float threshold = 0.01;
const float maxSamples = 10;

void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;
    //how many times the effect will iterate till it is barely visible, caps at 20
    float samples = maxSamples;
    if (g_Strength != 1)
        float samples = min(floor(log(threshold)/log(g_Strength)) + 1, maxSamples);
    vec2 toCenter = uv - vec2(0.5, 0.5);

    //init parameters
    vec3 color = vec3(0.0, 0.0, 0.0);
    vec2 scaleFac = g_Scale + vec2(1.0, 1.0);
    vec2 scale = vec2(1.0, 1.0);
    float strength = 1;

    for (float i=0; i<samples; i++)
    {
        vec2 sampleUV = toCenter / scale + vec2(0.5, 0.5);
        vec4 sampled = texture(sampler2D(m_Texture, m_Sampler), sampleUV);
        color += strength * sampled.w * sampled.xyz;
        scale *= scaleFac;
        strength *= g_Strength;
    }
    
    o_Colour = vec4(color,1.0);
}