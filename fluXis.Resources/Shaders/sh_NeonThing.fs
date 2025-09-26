layout(std140, set = 0, binding = 0) uniform m_NeonThingParameters
{
    vec2 g_TexSize;
    float g_Time;
    float g_Strength;  // color cycle speed
    float g_Strength2; // pattern density
    float g_Strength3; // edge thickness
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

// https://www.shadertoy.com/view/3fcXRf <-- modified from here
// i'll probably change the name later

float luminance(vec3 color) {
    return dot(color, vec3(0.2126, 0.7152, 0.0722));
}

float random(vec2 st) {
    return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;
    vec2 pixelSize = 1.0 / g_TexSize;

    float edgeThickness = 1.5;
    float sampleRadius = 0.5 * edgeThickness * g_Strength3;
    
    vec3 centerColor = texture(sampler2D(m_Texture, m_Sampler), uv).rgb;
    vec3 rightColor = texture(sampler2D(m_Texture, m_Sampler), uv + vec2(pixelSize.x * sampleRadius, 0.0)).rgb;
    vec3 leftColor = texture(sampler2D(m_Texture, m_Sampler), uv - vec2(pixelSize.x * sampleRadius, 0.0)).rgb;
    vec3 topColor = texture(sampler2D(m_Texture, m_Sampler), uv + vec2(0.0, pixelSize.y * sampleRadius)).rgb;
    vec3 bottomColor = texture(sampler2D(m_Texture, m_Sampler), uv - vec2(0.0, pixelSize.y * sampleRadius)).rgb;
    
    float centerLum = luminance(centerColor);
    float rightLum = luminance(rightColor);
    float leftLum = luminance(leftColor);
    float topLum = luminance(topColor);
    float bottomLum = luminance(bottomColor);
    
    float edgeX = abs(leftLum - rightLum);
    float edgeY = abs(topLum - bottomLum);
    float edge = max(edgeX, edgeY);
    
    //esto ya no lo estoy usando xd
    //edge = smoothstep(0.05, 0.3, edge * 4.0);
    //edge *= smoothstep(0.0, 0.2, centerLum);

    float edgeMultiplier = 3.3 * edgeThickness;
    float edgeThresholdLow = 0.02;
    float edgeThresholdHigh = 0.25;
    
    edge = smoothstep(edgeThresholdLow, edgeThresholdHigh, edge * edgeMultiplier);
    
    float finalEdge = edge * g_Strength3;
    
    float caAmount = 0.004 * finalEdge * (1.0 + 0.2 * sin(g_Time));
    vec3 chromaColor = centerColor;

    if (finalEdge > 0.01) {
        chromaColor.r = texture(sampler2D(m_Texture, m_Sampler), uv + vec2(caAmount, 0.0)).r;
        chromaColor.b = texture(sampler2D(m_Texture, m_Sampler), uv - vec2(caAmount, 0.0)).b;
    }
    
    float colorCycleSpeed = 1.5 * g_Strength;
    float patternDensity = 5.0 * g_Strength2;
    float time = g_Time * 0.001; // me when i don't even understand how the shader works

    vec3 neonColor = vec3(0.0);

    if (finalEdge > 0.1) {
        neonColor = 0.5 + 0.5 * cos(time * colorCycleSpeed + vec3(uv, uv) * patternDensity + vec3(0.0, 2.0, 4.0));
    }
    
    vec3 finalColor = mix(chromaColor, neonColor, finalEdge);
    
    o_Colour = vec4(finalColor, 1.0);
}