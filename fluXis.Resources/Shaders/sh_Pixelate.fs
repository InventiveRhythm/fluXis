layout(std140, set = 0, binding = 0) uniform m_PixelateParameters
{
    vec2 g_TexSize;
    float g_Strength;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;

    float pixelSizeFactor = mix(1.0, min(g_TexSize.x, g_TexSize.y), 1.0 - g_Strength);
    vec2 pixelSize = vec2(pixelSizeFactor, pixelSizeFactor * (g_TexSize.y / g_TexSize.x));
    vec2 pixelatedUV = (floor(uv * pixelSize) + 0.5) / pixelSize;
    
    o_Colour = textureLod(sampler2D(m_Texture, m_Sampler), pixelatedUV, 0.0);
}
