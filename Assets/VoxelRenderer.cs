using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

namespace Assets
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Voxel
    {
        public Vector4 Position;
        public Vector2 Size;
        public Vector3 Color;
    }

    public class VoxelRenderer : MonoBehaviour
    {
        public Material VoxelMaterial;

        public ComputeShader VoxelDataShader;
        public ComputeBuffer VoxelDataBuffer;
        public ComputeBuffer PointsBuffer;
        public ComputeBuffer PointsArgsBuffer;

        public ComputeShader RaymarchShader;
        public ComputeBuffer ColorsBuffer;

        private int[] _pointArgs = new int[4];
        private Voxel[] _voxels;
        private Vector3[] _colors;

        public Vector4 ScreenSize;

        private void Start()
        {
            Camera.onPostRender += Render;

            var voxelDataSize = new int3(8);
            var voxelCount = voxelDataSize.x * voxelDataSize.x * voxelDataSize.x;

            VoxelDataBuffer = new ComputeBuffer(voxelCount, Marshal.SizeOf<Voxel>());
            VoxelDataShader.SetBuffer(0, "voxel_data", VoxelDataBuffer);
            VoxelDataShader.Dispatch(0, voxelDataSize.x, voxelDataSize.y, voxelDataSize.z);

            _voxels = new Voxel[voxelCount];
            VoxelDataBuffer.GetData(_voxels);

            ScreenSize = new Vector4(Screen.width, Screen.height, 0, 0);
            var colorCount = (int)ScreenSize.x * (int)ScreenSize.y;

            ColorsBuffer = new ComputeBuffer(colorCount, Marshal.SizeOf<Vector3>());

            float4x4 vpMatrixInverse = Camera.main.previousViewProjectionMatrix.inverse;

            RaymarchShader.SetVector("screenSize", ScreenSize);
            RaymarchShader.SetVector("cameraPosition", Camera.main.transform.position);
            RaymarchShader.SetMatrix("vpMatrixInverse", vpMatrixInverse);

            RaymarchShader.SetBuffer(0, "colors", ColorsBuffer);
            RaymarchShader.SetBuffer(0, "voxels", VoxelDataBuffer);
            RaymarchShader.Dispatch(0, (int)ScreenSize.x / 8, (int)ScreenSize.y / 8, 1);

            _colors = new Vector3[colorCount];
            ColorsBuffer.GetData(_colors);

        }


        private int GetIndex(int3 position)
        {
            return position.y * 64 + position.z * 8 + position.x;
        }

        private int GetScreenIndex(Vector3 position)
        {
            return (int)position.y * (int)ScreenSize.x + (int)position.x;
        }

        private Color Raytrace(float3 position, float3 direction)
        {
            var travelLength = 0.0f;

            while (travelLength < 100.0f)
            {
                travelLength += 0.125f;
                position += direction * 0.125f;

                if (position.x < 1 && position.x > 0 && position.y < 1 && position.y > 0 && position.z < 1 && position.z > 0)
                {
                    var indexPosition = (int3)math.round(position * 8.0f);
                    var index = GetIndex(indexPosition);
                    if (_voxels == null || index > _voxels.Length)
                    {
                        break;
                    }

                    var colorVector = _voxels[index].Color;
                    return new Color(colorVector.x, colorVector.y, colorVector.z);
                }
            }

            return Color.black;
        }

        private void OnDrawGizmos()
        {
            for (var x = 0; x < ScreenSize.x; x++)
            {
                for (var y = 0; y < ScreenSize.y; y++)
                {
                    var position = new Vector3(x, y, 0);

                    var index = GetScreenIndex(position);

                    var colorVector = _colors[index];

                    Gizmos.color = new Color(colorVector.x, colorVector.y, colorVector.z); 
                    Gizmos.DrawCube(position, Vector3.one);
                }
            }

            //var matrix = Camera.main.previousViewProjectionMatrix;
            //float4x4 inverse = matrix.inverse;

            //var stepX = 2f / 16;
            //var stepY = 2f / 16;

            //for (var x = -1f; x < 1f; x += stepX)
            //{
            //    for (var y = -1f; y < 1f; y += stepY)
            //    {
            //        var worldDirection = math.mul(inverse, new float4(x, y, 0, 1));

            //        //worldDirection.w = 1.0f / worldDirection.w;
            //        //worldDirection.x *= worldDirection.w;
            //        //worldDirection.y *= worldDirection.w;
            //        //worldDirection.z *= worldDirection.w;

            //        Gizmos.color = Raytrace(Camera.main.transform.position, math.normalizesafe(worldDirection.xyz));
            //        Gizmos.DrawRay(Camera.main.transform.position, worldDirection.xyz);
            //    }
            //}
        }

        private void Render(Camera cam)
        {
            //VoxelMaterial.SetPass(0);
            //VoxelMaterial.SetBuffer("in_points", PointsBuffer);
            //Graphics.DrawProceduralIndirectNow(MeshTopology.Points, PointsArgsBuffer);
        }

        private void OnDestroyed()
        {
            VoxelDataBuffer.Dispose();
            PointsBuffer.Dispose();
            PointsArgsBuffer.Dispose();
        }
    }
}
