layout(set = 0, binding = 0) uniform lowp texture2D u_Original;
layout(set = 0, binding = 1) uniform lowp sampler s_Original;

layout(set = 1, binding = 0) uniform lowp texture2D u_Blurred;
layout(set = 1, binding = 1) uniform lowp sampler s_Blurred;

layout(location = 2) in mediump vec2 v_TexCoord;
layout(location = 0) out vec4 o_Colour;

void main()
{
    vec3 scene = texture(sampler2D(u_Original, s_Original), v_TexCoord).rgb;
    vec3 blur  = texture(sampler2D(u_Blurred,  s_Blurred),  v_TexCoord).rgb;
    o_Colour = vec4(blur, 1.0);
}