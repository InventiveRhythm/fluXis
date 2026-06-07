layout(std140, set = 0, binding = 0) uniform m_Glitch2Parameters
{
    vec2 TexSize;
    float Amount;
    float Speed;
    float Time;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture; 
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

highp float random(highp vec2 st)
{
    return fract(sin(dot(st.xy, vec2(12.9898, 4.1414))) * 43758.5453123);
}

highp float randomRange(highp vec2 st, float min, float max)
{
    return min + random(st) * (max - min);
}

highp float insideRange(float v, float b, float t)
{
    return step(b, v) - step(t, v);
}

void main(void)
{
    vec2 uv = gl_FragCoord.xy / TexSize;
    vec4 colour = texture(sampler2D(m_Texture, m_Sampler), uv);

    float maxOff = Amount / 2.0;
    float time = floor(Time * Speed * 60.0);

    for (float i = 0; i < 10.0 * Amount; i++) {
        float slcY = random(vec2(time, 2345.0 + float(i)));
        float slcH = random(vec2(time, 9035.0 + float(i))) * 0.25;
        float hOff = randomRange(vec2(time, 9625.0 + float(i)), -maxOff, maxOff);

        vec2 uvOff = uv;
        uvOff.x += hOff;

        if (insideRange(uv.y, slcY, fract(slcY + slcH)) == 1.0) {
            colour = texture(sampler2D(m_Texture, m_Sampler), uvOff);
        }
    }

    float maxColOff = Amount / 6.0;
    float rnd = random(vec2(time, 9545.0));
    vec2 colOffset = vec2(
        randomRange(vec2(time, 9545.0), -maxColOff, maxColOff),
        randomRange(vec2(time, 7205.0), -maxColOff, maxColOff)
    );

    if (rnd < 0.33) {
        colour.r = texture(sampler2D(m_Texture, m_Sampler), uv + colOffset).r;
    } else if (rnd < 0.66) {
        colour.g = texture(sampler2D(m_Texture, m_Sampler), uv + colOffset).g;
    } else {
        colour.b = texture(sampler2D(m_Texture, m_Sampler), uv + colOffset).b;  
    }

    o_Colour = colour;
}