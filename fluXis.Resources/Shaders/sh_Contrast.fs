layout(std140, set = 0, binding = 0) uniform m_ContrastParameters
{
    vec2 TexSize;
    float Strength;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture; 
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void)
{
    vec2 uv = gl_FragCoord.xy / TexSize;
    vec4 pixelColor = texture(sampler2D(m_Texture, m_Sampler), uv);
    
    pixelColor -= 0.5;
    pixelColor *= Strength * 25.0;
    pixelColor += 0.5;

    pixelColor.a = 1.0;
    o_Colour = pixelColor;
}