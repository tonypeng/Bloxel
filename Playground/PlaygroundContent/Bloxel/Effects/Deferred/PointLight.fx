#include "DeferredCommon.fxh"

float2 HalfPixel;

float3 LightPosition;
float4 LightColor;
float LightMaxIntensity;
float LightMaxRange;

float4x4 ModelViewProjection;
float4x4 ViewProjectionInverse;

texture DepthMap;
texture NormalMap;

sampler DepthSampler = sampler_state
{
	Texture = DepthMap;
};

sampler NormalSampler = sampler_state
{
	Texture = NormalMap;
};

struct VertexInput
{
	float4 Position : POSITION0;
};

struct PixelInput
{
	float4 Dummy : POSITION0;
	float4 ScreenSpacePosition : TEXCOORD0;
};

PixelInput PointLightVertexShader(VertexInput input)
{
	PixelInput pi = ( PixelInput ) 0;

	float4 transformedPos = mul(input.Position, ModelViewProjection);

	pi.Dummy = transformedPos;
	pi.ScreenSpacePosition = transformedPos;

	return pi;
}

float4 PointLightPixelShader(PixelInput input) : COLOR0
{
	float3 ssPosition = input.ScreenSpacePosition.xyz / input.ScreenSpacePosition.w;
	float2 uv = float2(ssPosition.x, -ssPosition.y) * 0.5f + 0.5f;

	uv += HalfPixel;

	float depth = tex2D(DepthSampler, uv);

	if(depth >= ssPosition.z) discard;

	float3 normal = DecodeNormal(tex2D(NormalSampler, uv));

	float4 posXYZW = float4(ssPosition.xy, depth, 1.0f);
	float4 worldPosition = mul(posXYZW, ViewProjectionInverse);
	worldPosition /= worldPosition.w;

	float3 lightVector = LightPosition - worldPosition.xyz;
	float3 lightDirection = normalize(lightVector);

	float dotProduct = dot(lightDirection, normalize(normal));

	if(dotProduct < 0.0f) discard;

	float lightDistance = length(lightVector);
	float falloff = saturate(1.0f - lightDistance / LightMaxRange);

	float intensity = saturate(dotProduct) * LightMaxIntensity * falloff * falloff;

	return LightColor * intensity;
}

technique PointLight
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 PointLightVertexShader();
		PixelShader = compile ps_2_0 PointLightPixelShader();
	}
}