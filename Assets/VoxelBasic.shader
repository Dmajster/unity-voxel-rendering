Shader "Voxel/Basic"
{
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma target 4.5
			#pragma require geometry
			#pragma enable_d3d11_debug_symbols

			#pragma vertex Vertex
			#pragma geometry Geometry
			#pragma fragment Fragment


			#include "VoxelBasic.cginc"
			ENDCG
		}
	}
}