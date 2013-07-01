/*
 * Bloxel - FreeCamera.cs
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

namespace Bloxel.Engine.Cameras
{
    /// <summary>
    /// Describes a camera that is free to move and rotate.
    /// </summary>
    public class FreeCamera : Camera
    {
        public FreeCamera() { }

        protected override void UpdateCamera()
        {
            _pitch = MathHelper.Clamp(_pitch, -1f * MathHelper.PiOver2, MathHelper.PiOver2);

            CameraRotation = Matrix.CreateRotationX(_pitch) * Matrix.CreateRotationY(_yaw) * Matrix.CreateRotationZ(_roll);

            _target = Position + CameraRotation.Forward;
        }

        public override void Zoom(float multiplier)
        {
            _projection = Matrix.CreatePerspectiveFieldOfView(fov / multiplier, aspectRatio, nearPlane, farPlane);
        }
    }
}
