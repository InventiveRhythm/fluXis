layout(std140, set = 0, binding = 0) uniform m_WarpParameters
{
    vec2 TexSize;
    float Scale;
    float Iterations;
    float Time;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture; 
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

highp float random(highp vec2 st)
{
    return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

highp float noise(vec2 pos) {
    pos *= Scale;
    pos += Time;

    vec2 ipos = floor(pos);
    vec2 fpos = fract(pos);

    fpos = fpos * fpos * (3.0 - 2.0*fpos);

    float interpolated = mix(
        mix(random(ipos), random(ipos + vec2(1.0, 0.0)), fpos.x),
        mix(random(ipos + vec2(0.0, 1.0)), random(ipos + vec2(1.0, 1.0)), fpos.x),
        fpos.y
    );

    return interpolated - (0.25 + float(Iterations - 1) * 0.04);
}

highp vec2 fbm(vec2 pos) {
    vec2 acc = vec2(0.0);
    const mat2 rot = mat2(0.8, 0.6, -0.6, 0.8);

    for (int i = 0; i < 4; i++) {
        float factor = 1.0 / float(i + 2);
        acc += vec2(
            noise(pos) * factor,
            noise(pos + vec2(34.32, 59.3)) * factor
        );

        pos *= rot;
    }

    return acc;
}

void main(void)
{
    vec2 uv = gl_FragCoord.xy / TexSize;
    
    float weight = 1.0;
    if (Iterations < 1.0) { weight = fract(Iterations); }
    
    vec2 warpedUV = fbm(uv) * weight;

    for (int i = 1; i < int(ceil(Iterations)); i++) {
        float weight = min(Iterations - float(i), 1.0);

        vec2 warp = fbm(warpedUV);
        warpedUV += warp * weight;
    }

    vec2 offsetFactor = vec2(Iterations / 4.0 * 0.07);

    warpedUV /= 10.0;
    warpedUV += uv - offsetFactor;

    vec4 pixelColor = textureLod(sampler2D(m_Texture, m_Sampler), warpedUV, 0.0);
    o_Colour = pixelColor;
}