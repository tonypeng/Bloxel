﻿/*
 * Bloxel - Camera.cs
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
    public abstract class Camera
    {
        /* TODO: make properties for all of these... */
        protected Vector3 _upVector;

        protected Vector3 _position;
        protected Vector3 _target;

        protected Matrix _view;
        protected Matrix _projection;

        protected Matrix _cameraRotation;

        protected float _yaw, _pitch, _roll;
        protected float _rotateSpeed;

        protected float fov, aspectRatio, nearPlane, farPlane;

        public Vector3 Position { get { return _position; } set { _position = value; } }
        public Vector3 Target { get { return _target; } }

        public Vector3 Forward { get { return _cameraRotation.Forward; } }
        public Vector3 Right { get { return _cameraRotation.Right; } }

        public Vector3 ForwardNoXRot { get { return Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(Yaw)); } }

        public float Yaw { get { return _yaw; } set { _yaw = value; } }
        public float Pitch { get { return _pitch; } set { _pitch = MathHelper.Clamp(value, -1f * MathHelper.PiOver2, MathHelper.PiOver2); } }

        public float RotateSpeed { get { return _rotateSpeed; } set { _rotateSpeed = value; } }

        public Matrix View { get { return _view; } protected set { _view = value; } }
        public Matrix Projection { get { return _projection; } protected set { _projection = value; } }

        public Matrix CameraRotation { get { return _cameraRotation; } protected set { _cameraRotation = value; } }

        public BoundingFrustum ViewFrustrum { get { return new BoundingFrustum(View * Projection); } }

        public Camera() { }

        public void Initialize(Vector3 up, float fov, float aspectRatio, float nearPlane, float farPlane, Vector3 position, Vector3 target)
        {
            _yaw = _pitch = _roll = 0.0f;

            // TODO: change to properties
            _rotateSpeed = 0.005f;

            _upVector = up;

            _position = position;
            _target = target;

            _view = Matrix.Identity;
            _projection = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, nearPlane, farPlane);

            this.fov = fov;
            this.aspectRatio = aspectRatio;
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
        }

        public void Update()
        {
            UpdateCamera();
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            _view = Matrix.CreateLookAt(Position, _target, CameraRotation.Up);
        }

        public virtual void Zoom(float multiplier)
        { }

        protected virtual void UpdateCamera() { }
    }
}
