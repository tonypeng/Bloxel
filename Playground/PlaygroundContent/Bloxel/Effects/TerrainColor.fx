struct VertexInput
{
	float4 Position	: POSITION0;
	float3 Normal : NORMAL0;
	float4 Color	: COLOR0;
	float Light		: COLOR1;
};

struct PixelInput
{
	float4 Position : POSITION0;
	float Distance : TEXCOORD0;
	float4 Color : COLOR0;
	float Light : COLOR1;
};

float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;

float3 CameraPosition;

float FogBegin;
float FogEnd;
float4 FogColor;

bool CPULightingEnabled = false;
float AmbientLight = 0.1;

float3 LightDirection;
float3 LightDirection2;

PixelInput SolidVertexShader(VertexInput input)
{
	PixelInput output = ( PixelInput ) 0;

	float4 worldPos = mul(input.Position, xWorld);
	float4 viewPos = mul(worldPos, xView);
	
	output.Position = mul(viewPos, xProjection);
	output.Distance = length(CameraPosition - worldPos);

	float3 normal = normalize(mul(normalize(input.Normal), xWorld));
	
	float normalComponent = min(saturate(saturate(dot(normal, -LightDirection)) + 0.4), saturate(saturate(dot(normal, -LightDirection2)) + 0.4));

	float lightComponent = normalComponent;

	if(CPULightingEnabled) {
		lightComponent *= input.Light;
	}

	output.Light = saturate(lightComponent + AmbientLight);

	output.Color = input.Color;

	return output;
}

float4 SolidPixelShader(PixelInput input) : COLOR0
{
	float4 color = input.Color;

	color.rgb = color.rgb * input.Light;

	if(color.a == 0)
	{
		clip(-1);
	}

	float fog = clamp((input.Distance - FogBegin) / (FogEnd - FogBegin), 0, 1);

	return lerp(color, FogColor, fog);
}

technique Solid
{
	pass Pass0
	{
		VertexShader = compile vs_2_0 SolidVertexShader();
		PixelShader = compile ps_2_0 SolidPixelShader();
	}
}