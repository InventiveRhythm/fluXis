layout(set = 0, binding = 0) uniform texture2D u_Base;
layout(set = 0, binding = 1) uniform sampler s_base;

layout(set = 1, binding = 0) uniform texture2D u_Additive;
layout(set = 1, binding = 1) uniform sampler s_Additive;

layout(set = 2, binding = 0) uniform m_AdditiveComposeParameters
{
    float g_Strength;
};

layout(location = 2) in vec2 v_TexCoord;
layout(location = 0) out vec4 o_Colour;

mediump float dither(vec2 uv) {
    return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453) - 0.5;
}

void main()
{
    vec3 scene = texture(sampler2D(u_Base, s_base), v_TexCoord).rgb;
    vec3 add = texture(sampler2D(u_Additive,  s_Additive),  v_TexCoord).rgb;
    mediump vec3 noise = vec3(dither(gl_FragCoord.xy) * g_Strength * 5) / 255.0;

    o_Colour = vec4(scene + noise + add * g_Strength, 1.0);
}