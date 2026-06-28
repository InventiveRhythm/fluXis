#ifndef DITHER_FS
#define DITHER_FS

#include "sh_Utils.h"

layout(location = 2) in mediump vec2 v_TexCoord;
layout(set = 1, binding = 0) uniform lowp texture2D m_Texture;
layout(set = 1, binding = 1) uniform lowp sampler m_Sampler;

layout(std140, set = 0, binding = 0) uniform m_DitherParameters
{
    mediump float g_Strength;
};

layout(location = 0) out vec4 o_Colour;

mediump float dither(vec2 uv) {
    return fract(sin(dot(uv, vec2(12.9898, 78.233))) * 43758.5453) - 0.5;
}

void main(void)
{
    vec4 texColor = texture(sampler2D(m_Texture, m_Sampler), v_TexCoord);

    mediump vec3 noise = vec3(dither(gl_FragCoord.xy) * g_Strength) / 255.0;

    texColor.rgb += noise;

    o_Colour = texColor;
}
#endif
