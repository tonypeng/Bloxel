/*
 * Bloxel - DualContouringUtil.cs
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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Bloxel.Engine.DataStructures;

namespace Bloxel.Engine.Utilities
{
    public static class DualContouring
    {
        /// <summary>
        /// Default maximum number of iterations for the SchmitzVertexFromHermiteData method.
        /// </summary>
        public const int MAX_ITERATIONS = 50;

        /// <summary>
        /// Calculates an approximated vertex for a block.
        /// The method is based off of the algorithm described in Leonardo Augusto Schmitz's paper on "Analysis and Acceleration of High Quality Isosurface Contouring" from October 2009.
        /// </summary>
        /// <param name="hermite">The hermite data for some arbitrary block.</param>
        /// <param name="threshold">The value that a force at or below will cause the method to return the approximated position.</param>
        /// <returns>The approximated vertex for the block with the given hermite data.</returns>
        public static Vector3 SchmitzVertexFromHermiteData(HermiteData hermite, float threshold)
        {
            return SchmitzVertexFromHermiteData(hermite, threshold, MAX_ITERATIONS);
        }

        /// <summary>
        /// Calculates an approximated vertex for a block.
        /// The method is based off of the algorithm described in Leonardo Augusto Schmitz's paper on "Analysis and Acceleration of High Quality Isosurface Contouring" from October 2009.
        /// </summary>
        /// <param name="hermite">The hermite data for some arbitrary block.</param>
        /// <param name="threshold">The value that a force at or below will cause the method to return the approximated position.</param>
        /// <param name="maxIterations">The maximum number of iterations of force to step the particle through.</param>
        /// <returns>The approximated vertex for the block with the given hermite data.</returns>
        public static Vector3 SchmitzVertexFromHermiteData(HermiteData hermite, float threshold, int maxIterations)
        {
            threshold *= threshold; // square it so we don't have to use sqrt later

            // copy these so I don't have to type as much...
            List<Vector3> xPoints = hermite.IntersectionPoints;
            List<Vector3> grads = hermite.GradientVectors;
            int pointsCount = xPoints.Count;

            // The two lists should be synchronized, or we have an isssue...
            Contract.Assert(xPoints.Count == grads.Count);

            // start at mass point C, which is calculated by taking the mean of all the
            // intersection points.
            Vector3 c = Vector3.Zero;

            // average it out
            for (int i = 0; i < pointsCount; i++) c += xPoints[i];
            c /= pointsCount; // basic arithmetic mean

            for (int i = 0; i < maxIterations; i++)
            {
                // create a force that acts on the mass
                Vector3 force = Vector3.Zero;

                // loop through each intersection point
                for (int j = 0; j < pointsCount; j++)
                {
                    Vector3 xPoint = xPoints[j];
                    Vector3 xNormal = grads[j];

                    force += xNormal * -1f * (Vector3.Dot(xNormal, c - xPoint));
                }

                // dampen the force
                float damping = 1f - (float)i / maxIterations;
                c += force * damping / pointsCount; // average over all the points

                // if the force is negligible according to the threshold, we're done
                if (force.LengthSquared() < threshold)
                    break;
            }

            return c; // return the approximated position
        }
    }
}
