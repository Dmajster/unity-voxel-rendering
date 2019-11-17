using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets
{
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

        public ComputeShader ComputeShader;
        public ComputeBuffer VoxelBuffer;
        public ComputeBuffer VoxelArgsBuffer;

        private void Start()
        {
            Camera.onPostRender += Render;

            VoxelBuffer = new ComputeBuffer(2, Marshal.SizeOf<Voxel>());
            VoxelArgsBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);

            var voxelData = new[]
            {
                new Voxel
                {
                    Position = new Vector4(0, 0, 0, 1),
                    Size = new Vector2(1.0f, 0.0f),
                    Color = new Vector3(255, 0, 0)
                },
                new Voxel
                {
                    Position = new Vector4(1, 0, 0, 1),
                    Size = new Vector2(1f, 0.0f),
                    Color = new Vector3(0, 1.0f, 0)
                },
            };
            VoxelBuffer.SetData(voxelData);

            var voxelArgsData = new[]
            {
                1,
                2,
                0,
                0,
            };
            VoxelArgsBuffer.SetData(voxelArgsData);
        }

        private void Render(Camera cam)
        {
            VoxelMaterial.SetPass(0);
            VoxelMaterial.SetBuffer("_VoxelBuffer", VoxelBuffer);
            Graphics.DrawProceduralIndirectNow(MeshTopology.Points, VoxelArgsBuffer);
        }

        private void OnDestroyed()
        {
            VoxelBuffer.Dispose();
            VoxelArgsBuffer.Dispose();
        }
    }
}
