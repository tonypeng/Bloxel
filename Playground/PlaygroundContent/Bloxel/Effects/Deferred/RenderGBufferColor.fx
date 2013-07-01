#include "DeferredCommon.fxh"

float ColorFactor;

float4x4 WorldViewProjection;
float4x4 WorldInverseTranspose;

texture ColorMap;

sampler ColorSampler = sampler_state
{
	Texture = ColorMap;
};

struct VertexInput
{
	float4 Position	: POSITION0;
	float3 Normal : NORMAL0;
	float4 Color	: COLOR0;
};

struct PixelInput
{
	float4 Position : POSITION0;
	float3 Normal : TEXCOORD0;
	float4 TPosition : TEXCOORD1;
	float4 WPosition : TEXCOORD2;
	float4 Color : COLOR0;
};

PixelInput RenderGBufferColorVertexShader(VertexInput input)
{
	PixelInput pi = ( PixelInput ) 0;

	pi.Position = mul(input.Position, WorldViewProjection);
	pi.Normal = mul(input.Normal, WorldInverseTranspose);
	pi.Color = input.Color;
	pi.TPosition = pi.Position;
	pi.WPosition = input.Position;
	
	return pi;	
}

GBufferTarget RenderGBufferColorPixelShader(PixelInput input)
{
	GBufferTarget output = ( GBufferTarget ) 0;

	float3 position = input.TPosition.xyz / input.TPosition.w;

	output.Albedo = lerp(float4(1.0f, 1.0f, 1.0f, 1.0f), input.Color, ColorFactor);
    output.Normal = EncodeNormal(input.Normal);
    output.Depth = position.z;

    return output;
}

technique RenderGBufferColor
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 RenderGBufferColorVertexShader();
		PixelShader = compile ps_2_0 RenderGBufferColorPixelShader();
	}
}