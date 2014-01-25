using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Bloxel.Engine.Core
{
    public class MeshBuildEventArgs : EventArgs
    {
        private Vector3[] _vertices;
        private int[] _indices;

        public MeshBuildEventArgs(Vector3[] vertices, int[] indices)
        {
            _vertices = vertices;
            _indices = indices;
        }

        public Vector3[] Vertices
        {
            get { return _vertices; }
        }

        public int[] Indices
        {
            get { return _indices; }
        }
    }
}
