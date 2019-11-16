Shader "Voxel/Basic"
{
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma target 4.0
			#pragma vertex Vertex
			#pragma geometry Geometry
			#pragma fragment Fragment
			#include "VoxelBasic.cginc"
			ENDCG
		}
	}
}