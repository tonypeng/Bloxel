#include "DeferredCommon.fxh"

float4 ClearColor = { 0.0, 0.0, 0.0, 0.0 };

struct VertexInput
{
	float4 Position : POSITION0;
};

struct PixelInput
{
	float4 Position : POSITION0;
};

PixelInput ClearVertexShader(VertexInput input)
{
	PixelInput pi = ( PixelInput ) 0;

	pi.Position = input.Position;

	return pi;
}

GBufferTarget ClearPixelShader(PixelInput input)
{
	GBufferTarget output = ( GBufferTarget ) 0;

	output.Albedo = ClearColor;
	output.Normal = float4(0.5, 0.5, 0.5, 0.0);
	output.Depth = 1.0f;

	return output;
}

technique Clear
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 ClearVertexShader();
		PixelShader = compile ps_2_0 ClearPixelShader();
	}
}