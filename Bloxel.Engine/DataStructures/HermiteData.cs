/*
 * Bloxel - HermiteData.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
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
