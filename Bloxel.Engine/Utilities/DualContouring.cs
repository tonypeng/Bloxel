/*
 * Bloxel - DualContouring.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * This file is subject to the terms and conditions defined in the
 * file 'LICENSE.txt', which is part of this source code package.
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
        /// Length of the DualContourModification flag in the GridPoint metadata.
        /// </summary>
        public const int DUAL_CONTOUR_MODIFICATION_BIT_LENGTH = 4; // 4 bits reserved for the modification type yields 2^4=16 types of grid points

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

            // take care of this special case
            if (pointsCount == 0)
                return Vector3.Zero;

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

        public static Vector3 InterpolateIntersectionPoint(float isolevel, Vector3 p1, Vector3 p2, float value1, float value2)
        {
            float diff1 = Math.Abs(value1 - isolevel);
            float diff2 = Math.Abs(value2 - isolevel);
            float diff12 = Math.Abs(value2 - value1);

            if (diff1 < 0.001f)
                return p1;
            if (diff2 < 0.001f)
                return p2;
            if (diff12 < 0.001f)
                return p1;

            Vector3 xPoint = Vector3.Zero;
            float k = (isolevel - value1) / (value2 - value1);

            // basic linear interpolation
            xPoint.X = p1.X + k * (p2.X - p1.X);
            xPoint.Y = p1.Y + k * (p2.Y - p1.Y);
            xPoint.Z = p1.Z + k * (p2.Z - p1.Z);

            return xPoint;
        }
    }
}
