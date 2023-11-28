layout(std140, set = 0, binding = 0) uniform m_BloomParameters
{
	vec2 g_TexSize;
};

layout(set = 1, binding = 0) uniform texture2D m_Texture;
layout(set = 1, binding = 1) uniform sampler m_Sampler;

layout(location = 0) out vec4 o_Colour;

#undef INV_SQRT_2PI
#define INV_SQRT_2PI 0.39894

float computeGauss(in float x, in float sigma)
{
	return INV_SQRT_2PI * exp(-0.5*x*x / (sigma*sigma)) / sigma;
}

vec4 blur(int radius, vec2 direction, vec2 texCoord, vec2 texSize, float sigma)
{
	float factor = computeGauss(0.0, sigma);
	vec4 sum = texture(sampler2D(m_Texture, m_Sampler), texCoord) * factor;

	float totalFactor = factor;

	for (int i = 2; i <= 200; i += 2)
	{
		float x = float(i) - 0.5;
		factor = computeGauss(x, sigma) * 2.0;
		totalFactor += 2.0 * factor;
		
		sum += texture(sampler2D(m_Texture, m_Sampler), texCoord + direction * x / texSize) * factor;
		sum += texture(sampler2D(m_Texture, m_Sampler), texCoord - direction * x / texSize) * factor;
		
		if (i >= radius) break;
	}

	return sum / totalFactor;
}

void main(void) {
    vec2 uv = gl_FragCoord.xy / g_TexSize;
    vec4 colour = texture(sampler2D(m_Texture, m_Sampler), uv);
    
    vec4 blurColour = blur(50, vec2(0.0, 0.0), uv, g_TexSize, 50.0);
    
    o_Colour = blurColour;
}