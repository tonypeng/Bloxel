#include "DeferredCommon.fxh"

float3 LightDirection;
float4 LightColor;
float LightMaxIntensity;

float4x4 ViewProjectionInverse;

texture NormalMap;

sampler NormalSampler = sampler_state
{
	Texture = NormalMap;
};

struct VertexInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinates : TEXCOORD0;
};

struct PixelInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinates : TEXCOORD0;
};

PixelInput DirectionalLightVertexShader(VertexInput input)
{
	PixelInput pi = ( PixelInput ) 0;

	pi.Position = input.Position;
	pi.TextureCoordinates = input.TextureCoordinates;

	return pi;
}

float4 DirectionalLightPixelShader(PixelInput input) : COLOR0
{
	float3 normal = DecodeNormal(tex2D(NormalSampler, input.TextureCoordinates));

	float normalComponent = saturate(dot(LightDirection, normal)) * LightMaxIntensity;

	return LightColor * normalComponent;
}

technique DirectionalLight
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 DirectionalLightVertexShader();
		PixelShader = compile ps_2_0 DirectionalLightPixelShader();
	}
}