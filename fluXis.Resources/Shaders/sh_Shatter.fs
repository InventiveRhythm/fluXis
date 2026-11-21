layout(std140, set = 0, binding = 0) uniform m_ShatterParameters
{
    vec2 TexSize;
    float CellSize;
    float DistortionStrength;
    float Time;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture; 
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

highp float random(highp vec2 st)
{
    return fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123);
}

highp vec2 random2(highp vec2 st)
{
    return vec2(
        fract(sin(dot(st.xy, vec2(12.9898, 78.233))) * 43758.5453123),
        fract(sin(dot(st.xy, vec2(49.1263, 13.334))) * 43758.5453123)
    );
}

highp float noise(vec2 pos) {
    // pos *= Scale;
    // pos += Time * PhaseSpeed;

    vec2 ipos = floor(pos);
    vec2 fpos = fract(pos);

    fpos = fpos * fpos * (3.0 - 2.0*fpos);

    float interpolated = mix(
        mix(random(ipos), random(ipos + vec2(1.0, 0.0)), fpos.x),
        mix(random(ipos + vec2(0.0, 1.0)), random(ipos + vec2(1.0, 1.0)), fpos.x),
        fpos.y
    );

    return interpolated - 0.5;
}

highp vec2 voronoi(vec2 pos) {
    vec2 ipos = floor(pos);
    vec2 fpos = fract(pos);

    float minDist = 100000000.0;
    vec2 closestPoint = vec2(0.0);

    for (int y = -1; y <= 1; y++) {
        for (int x = -1; x <= 1; x++) {
            vec2 neighbour = vec2(float(x), float(y));
            vec2 point = random2(ipos + neighbour);

            point = 0.5 + 0.5 * sin(Time + 6.2834 * point);

            vec2 diff = neighbour + point - fpos;
            float dist = length(diff);

            if (dist < minDist) {
                minDist = dist;
                closestPoint = point;
            }
        }
    }

    return closestPoint;
}

void main(void)
{
    vec2 uv = gl_FragCoord.xy / TexSize;
    vec2 scaledUV = uv * CellSize;

    vec2 cell = voronoi(scaledUV);
    vec2 uvOffset = vec2(noise(cell)) * DistortionStrength;
    
    vec4 pixelColor = texture(sampler2D(m_Texture, m_Sampler), uv + uvOffset);
    o_Colour = pixelColor;
}