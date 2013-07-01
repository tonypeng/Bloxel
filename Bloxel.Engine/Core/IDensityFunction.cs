/*
 * Bloxel - IDensityFunction.cs
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

namespace Bloxel.Engine.Core
{
    /// <summary>
    /// Provides an interface for a density function.
    /// </summary>
    public interface IDensityFunction
    {
        /// <summary>
        /// Evaluates the implicit function at the specified position.
        /// </summary>
        /// <param name="x">The x-component of the position.</param>
        /// <param name="y">The y-component of the position.</param>
        /// <param name="z">The z-component of the position.</param>
        /// <returns>The result of evaluating the implicit function at the specified position.</returns>
        float f(float x, float y, float z);
    }
}
