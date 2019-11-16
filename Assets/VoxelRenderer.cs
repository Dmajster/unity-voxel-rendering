using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets
{
    public class VoxelRenderer : MonoBehaviour
    {
        private void Start()
        {
            var meshFilter = GetComponent<MeshFilter>();

            var positions = new List<Vector3>();
            var colors = new List<Color>();
            var indices = new List<int>();
            var sizes = new List<Vector2>();

            positions.Add(new Vector3(0,0,0));
            positions.Add(new Vector3(1, 0, 0));

            colors.Add(Color.red);
            colors.Add(Color.black);

            sizes.Add(new Vector2(1.0f, 0.0f));
            sizes.Add(new Vector2(0.5f, 0.0f));
            indices.Add(0);
            indices.Add(1);
            var mesh = new Mesh {indexFormat = IndexFormat.UInt32};
            mesh.SetVertices(positions);
            mesh.SetColors(colors);
            mesh.SetUVs(0, sizes);
            mesh.SetIndices(indices, MeshTopology.Points, 0);
            meshFilter.mesh = mesh;
        }
    }
}
