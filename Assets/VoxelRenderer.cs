using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

namespace Assets
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Voxel
    {
        public Vector4 Position;
        public Vector2 Size;
        public Color Color;
    }

    public class VoxelRenderer : MonoBehaviour
    {
        public Material VoxelMaterial;

        public ComputeShader ComputeShader;
        public ComputeBuffer VoxelDataBuffer;
        public ComputeBuffer PointsBuffer;
        public ComputeBuffer PointsArgsBuffer;

        public int[] PointArgs; 

        private void Start()
        {
            Camera.onPostRender += Render;

            var voxelDataSize = new int3(32, 32, 32);
            var voxelCount = voxelDataSize.x * voxelDataSize.y * voxelDataSize.z;

            VoxelDataBuffer = new ComputeBuffer(voxelCount, sizeof(int)); // 1m^3

            PointsBuffer = new ComputeBuffer(voxelCount, Marshal.SizeOf<Voxel>(), ComputeBufferType.Append);
            PointsArgsBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);

            ComputeShader.SetBuffer(0, "in_voxel_data", VoxelDataBuffer);
            ComputeShader.SetBuffer(0, "out_points", PointsBuffer);
            ComputeShader.Dispatch(0, voxelDataSize.x / 8, voxelDataSize.y / 8, voxelDataSize.z / 8);
            ComputeBuffer.CopyCount(PointsBuffer, PointsArgsBuffer, sizeof(int));

            PointArgs = new int[4];
            PointsArgsBuffer.SetData(
            new []{
                1,
                32768,
                0,
                0
            });

            VoxelMaterial.SetPass(0);
            VoxelMaterial.SetBuffer("in_points", PointsBuffer);
        }

        private void Render(Camera cam)
        {
            VoxelMaterial.SetPass(0);
            Graphics.DrawProceduralIndirectNow(MeshTopology.Points, PointsArgsBuffer);
        }

        private void OnDestroyed()
        {
            VoxelDataBuffer.Dispose();
            PointsBuffer.Dispose();
            PointsArgsBuffer.Dispose();
        }
    }
}
