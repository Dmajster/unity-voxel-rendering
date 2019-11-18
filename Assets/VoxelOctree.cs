using System.Collections.Generic;
using Unity.Mathematics;

namespace Assets
{
    struct VoxelOctreeNode
    {
        public byte ChildMask;

        public int GetChild(int nthChild)
        {
            return 0;
        }
    }

    class VoxelOctree
    {
        public float3 Position;
        public float3 Size;

        public List<VoxelOctreeNode> Nodes;
    }
}
