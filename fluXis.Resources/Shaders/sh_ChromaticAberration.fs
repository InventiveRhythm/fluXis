void main(void) {
    o_Colour = chromaticAberration(v_TexCoord, u_Resolution, vec2(1.0, 0.0), 0.01);
}

lowp vec4 chromaticAberration(lowp vec2 uv, lowp vec2 resolution, lowp vec2 direction, lowp float amount) {
    lowp vec2 offset = amount * direction;
    lowp vec4 cr = texture2D(u_Texture, uv + offset / resolution);
    lowp vec4 cga = texture2D(u_Texture, uv);
    lowp vec4 cb = texture2D(u_Texture, uv - offset / resolution);
    return vec4(cr.r, cga.g, cb.b, cga.a);
}