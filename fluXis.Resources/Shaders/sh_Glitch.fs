layout(std140, set = 0, binding = 0) uniform m_GlitchParameters
{
    vec2 g_TexSize;
    float g_StrengthX;
    float g_StrengthY;
    float g_BlockSize;
    float g_Time;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture; 
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

highp float random(highp vec2 st, float seed)
{
    return fract(sin(dot(st.xy, vec2(12.9898, 78.233) + seed)) * 43758.5453123);
}


void main(void)
{
    vec2 uv = gl_FragCoord.xy / g_TexSize;
    
    float blockSizeInPixels = mix(1.0, min(g_TexSize.x, g_TexSize.y), g_BlockSize);
    
    vec2 blockUV = floor(uv * blockSizeInPixels) / blockSizeInPixels;
    
    float randomShiftX = (random(blockUV, g_Time) - 0.5) * g_StrengthX;
    float randomShiftY = (random(blockUV + vec2(5.0), g_Time) - 0.5) * g_StrengthY;

    vec2 fixedUV = uv;
    fixedUV.x += randomShiftX;
    fixedUV.y += randomShiftY;
    
    vec4 pixelColor = textureLod(sampler2D(m_Texture, m_Sampler), fixedUV, 0.0);

    o_Colour = pixelColor;
}