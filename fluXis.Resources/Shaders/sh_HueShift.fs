layout(std140, set = 0, binding = 0) uniform m_HueShiftParameters
{
    vec2 g_TexSize;
    float g_Strength;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

vec3 hueShift(vec3 color, float hueAdjust) {
    const vec3 kRGBToYPrime = vec3(0.299, 0.587, 0.114);
    const vec3 kRGBToI      = vec3(0.596, -0.275, -0.321);
    const vec3 kRGBToQ      = vec3(0.212, -0.523, 0.311);

    const vec3 kYIQToR     = vec3(1.0, 0.956, 0.621);
    const vec3 kYIQToG     = vec3(1.0, -0.272, -0.647);
    const vec3 kYIQToB     = vec3(1.0, -1.107, 1.704);

    float YPrime = dot(color, kRGBToYPrime);
    float I      = dot(color, kRGBToI);
    float Q      = dot(color, kRGBToQ);
    float chroma = sqrt(I * I + Q * Q);

    if (chroma < 1e-5) {
        return color;
    }

    float hue = atan(Q, I);
    hue += hueAdjust;

    Q = chroma * sin(hue);
    I = chroma * cos(hue);

    vec3 yIQ = vec3(YPrime, I, Q);

    return vec3(dot(yIQ, kYIQToR), dot(yIQ, kYIQToG), dot(yIQ, kYIQToB));
}

void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;
    vec4 colour = texture(sampler2D(m_Texture, m_Sampler), uv);

    vec3 shiftedColour = hueShift(colour.rgb, radians(g_Strength * 360.0));

    o_Colour = vec4(shiftedColour, colour.a);
}
