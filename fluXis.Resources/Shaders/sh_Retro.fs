layout(std140, set = 0, binding = 0) uniform m_RetroParameters
{
    vec2 g_TexSize;
    float g_Strength;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

// https://www.shadertoy.com/view/WsVSzV

void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;
    vec4 colour = texture(sampler2D(m_Texture, m_Sampler), uv);
    
    vec2 dc = abs(vec2(0.5) - uv);
    dc *= dc;
    
    uv.x -= 0.5; uv.x *= 1.0 + (dc.y * (0.3 * g_Strength)); uv.x += 0.5;
    uv.y -= 0.5; uv.y *= 1.0 + (dc.x * (0.4 * g_Strength)); uv.y += 0.5;
    
    if (uv.y > 1.0 || uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0) {
        o_Colour = vec4(0.0, 0.0, 0.0, 1.0);
    } else {
        float apply = abs(sin(gl_FragCoord.y) * 0.5 * g_Strength);
        o_Colour = vec4(mix(texture(sampler2D(m_Texture, m_Sampler), uv).rgb, vec3(0.0), apply), 1.0);
    }
}
