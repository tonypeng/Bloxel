#include "DeferredCommon.fxh"

float4x4 ViewProjectionInverse;

texture ColorMap;
texture LightMap;

sampler ColorSampler = sampler_state
{
	Texture = ColorMap;
};

sampler LightSampler = sampler_state
{
	Texture = LightMap;
};

struct VertexInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinates : TEXCOORD0;
};

struct PixelInput
{
	float4 Dummy : POSITION0;
	float2 TextureCoordinates : TEXCOORD0;
};

PixelInput RenderCombineVertexShader(VertexInput input)
{
	PixelInput pi = ( PixelInput ) 0;

	pi.Dummy = input.Position;
	pi.TextureCoordinates = input.TextureCoordinates;

	return pi;
}

float4 RenderCombinePixelShader(PixelInput input) : COLOR0
{
	float4 color = tex2D(ColorSampler, input.TextureCoordinates);
	float4 light = tex2D(LightSampler, input.TextureCoordinates);

	return color * light;
}

technique Combine
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 RenderCombineVertexShader();
		PixelShader = compile ps_2_0 RenderCombinePixelShader();
	}
}