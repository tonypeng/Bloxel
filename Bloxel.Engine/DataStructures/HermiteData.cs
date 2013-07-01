/*
 * Bloxel - HermiteData.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * This file is subject to the terms and conditions defined in the
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Bloxel.Engine.DataStructures
{
    public struct HermiteData
    {
        private List<Vector3> _intersectionPoints;
        private List<Vector3> _gradientVectors;

        public HermiteData(List<Vector3> intersectionPoints, List<Vector3> gradientVectors)
        {
            _intersectionPoints = intersectionPoints;
            _gradientVectors = gradientVectors;
        }

        public void Add(Vector3 intersectionPoint, Vector3 gradient)
        {
            _intersectionPoints.Add(intersectionPoint);
            _gradientVectors.Add(gradient);
        }

        /// <summary>
        /// Returns the list of intersection points.  The returned list should only be read and should not be modified.
        /// Use the Add method to modify the list.
        /// </summary>
        public List<Vector3> IntersectionPoints { get { return _intersectionPoints; } }
        /// <summary>
        /// Returns the list of gradient vectors.  The returned list should only be read and should not be modified.
        /// Use the Add method to modify the list.
        /// </summary>
        public List<Vector3> GradientVectors { get { return _gradientVectors; } }
    }
}
