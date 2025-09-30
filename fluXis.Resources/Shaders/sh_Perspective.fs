layout(std140, set = 0, binding = 0) uniform m_PerspectiveParameters
{
    vec2 g_TexSize;
    float g_Strength;
    float g_Strength2;
    float g_Strength3;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

// think this is a port of a godot shader          // saludos a mi compañero matias
// i'll probably change the name later

bool cull_back = true;
float inset = 0.0;

const float PI = 3.14159;

mat3 getRotationMatrix(float yaw, float pitch) {
    float sin_b = sin(yaw / 180.0 * PI);
    float cos_b = cos(yaw / 180.0 * PI);
    float sin_c = sin(pitch / 180.0 * PI);
    float cos_c = cos(pitch / 180.0 * PI);
    
    mat3 inv_rot_mat;
    inv_rot_mat[0][0] = cos_b;
    inv_rot_mat[0][1] = 0.0;
    inv_rot_mat[0][2] = -sin_b;
    
    inv_rot_mat[1][0] = sin_b * sin_c;
    inv_rot_mat[1][1] = cos_c;
    inv_rot_mat[1][2] = cos_b * sin_c;
    
    inv_rot_mat[2][0] = sin_b * cos_c;
    inv_rot_mat[2][1] = -sin_c;
    inv_rot_mat[2][2] = cos_b * cos_c;
    
    return inv_rot_mat;
}

void main(void) {
    vec2 uv = (gl_FragCoord.xy / g_TexSize) - 0.5;
    
    float x_rot = g_Strength2;
    float y_rot = g_Strength; 
    float fov   = g_Strength3 + 0.1;
    
    mat3 inv_rot_mat = getRotationMatrix(x_rot, y_rot);
    
    float t = tan(fov / 360.0 * PI);
    vec3 ray_direction = inv_rot_mat * vec3(uv, 0.5 / t);
    float v = (0.5 / t) + 0.5;
    
    ray_direction.xy *= v * inv_rot_mat[2].z;
    vec2 offset = v * inv_rot_mat[2].xy;
    
    if (cull_back && ray_direction.z <= 0.0) {
        o_Colour = vec4(0.0);
        return;
    }
    
    vec2 tex_uv = (ray_direction.xy / ray_direction.z) - offset;
    tex_uv += 0.5;
    
    vec4 color = texture(sampler2D(m_Texture, m_Sampler), tex_uv);
    
    float distance_from_center = max(abs(tex_uv.x - 0.5), abs(tex_uv.y - 0.5));
    color.a *= step(distance_from_center, 0.5);
    
    o_Colour = color;
}