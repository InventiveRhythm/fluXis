layout(std140, set = 0, binding = 0) uniform m_SliceParameters
{
    vec2 TexSize;
    float Strength;
    float Angle;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture; 
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

void main(void)
{
    vec2 uv = gl_FragCoord.xy / TexSize;
    vec2 scaledUV = uv * 2.0 - 1.0;
    vec2 direction = vec2(
        cos(radians(Angle)),
        sin(radians(Angle))
    );

    float side = sign(dot(direction, scaledUV));

    vec2 movement = vec2(
        cos(radians(Angle + 90.0)),
        sin(radians(Angle + 90.0))
    );

    vec2 offsetUV = scaledUV;
    offsetUV += movement * side * Strength;
    offsetUV = offsetUV / 2.0 + 0.5;
    offsetUV = fract(offsetUV);

    vec4 pixelColor = textureLod(sampler2D(m_Texture, m_Sampler), offsetUV, 0.0);

    o_Colour = pixelColor;
}