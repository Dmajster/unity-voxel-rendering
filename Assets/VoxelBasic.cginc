#include "UnityCG.cginc"
#include "UnityGBuffer.cginc"
#include "UnityStandardUtils.cginc"

struct appdata
{
	float4 position : POSITION;
	float2 size : TEXCOORD0;
	float3 color : COLOR;
};

struct v2g
{
	float4 position : POSITION;
	float2 size : TEXCOORD0;
	float3 color : COLOR;
};

struct g2f
{
	float4 position : SV_POSITION;
	float3 color : COLOR;
};

StructuredBuffer<appdata> in_points;

v2g Vertex(uint id : SV_InstanceID)
{
	v2g output;
	appdata input = in_points[id];

	output.position = input.position;
	output.size = input.size;
	output.color = input.color;

	return output;
}

[maxvertexcount(14)]
void Geometry(point v2g input_array[1], inout TriangleStream<g2f>outputStream) {
	v2g input = input_array[0];

	g2f corner0; corner0.color = input.color; corner0.position = UnityObjectToClipPos(input.position);
	g2f corner1; corner1.color = input.color; corner1.position = UnityObjectToClipPos(input.position + float4(0, 0, input.size.x, 0));
	g2f corner2; corner2.color = input.color; corner2.position = UnityObjectToClipPos(input.position + float4(0, input.size.x, 0, 0));
	g2f corner3; corner3.color = input.color; corner3.position = UnityObjectToClipPos(input.position + float4(0, input.size.x, input.size.x, 0));
	g2f corner4; corner4.color = input.color; corner4.position = UnityObjectToClipPos(input.position + float4(input.size.x, 0, 0, 0));
	g2f corner5; corner5.color = input.color; corner5.position = UnityObjectToClipPos(input.position + float4(input.size.x, 0, input.size.x, 0));
	g2f corner6; corner6.color = input.color; corner6.position = UnityObjectToClipPos(input.position + float4(input.size.x, input.size.x, 0, 0));
	g2f corner7; corner7.color = input.color; corner7.position = UnityObjectToClipPos(input.position + float4(input.size.x, input.size.x, input.size.x, 0));

	outputStream.Append(corner1);
	outputStream.Append(corner0);
	outputStream.Append(corner5);
	outputStream.Append(corner4);
	outputStream.Append(corner6);
	outputStream.Append(corner0);
	outputStream.Append(corner2);
	outputStream.Append(corner1);
	outputStream.Append(corner3);
	outputStream.Append(corner5);
	outputStream.Append(corner7);
	outputStream.Append(corner6);
	outputStream.Append(corner3);
	outputStream.Append(corner2);
	outputStream.RestartStrip();
}

float3 Fragment(g2f input) : SV_Target
{
	return input.color;
}