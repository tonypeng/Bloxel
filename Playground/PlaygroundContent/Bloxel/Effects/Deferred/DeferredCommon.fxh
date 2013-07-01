struct GBufferTarget
{
	half4 Albedo : COLOR0;
	half4 Normal : COLOR1;
	half4 Depth	: COLOR2;
};

float4 EncodeNormal(float3 normal)
{
	return float4((normal.xyz * 0.5f) + 0.5f, 0.0f);
}

float3 DecodeNormal(float4 encoded)
{
	return encoded * 2.0 - 1.0f;
}