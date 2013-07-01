/*
 * Bloxel - CameraManager.cs
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

namespace Bloxel.Engine.Cameras
{
    public class CameraManager
    {
        private Dictionary<string, Camera> _cameras;
        private string _mainCamera;

        public string MainCameraName
        {
            get { return _mainCamera; }
            set
            {
                Contract.Assert(_cameras.ContainsKey(value));

                _mainCamera = value;
            }
        }

        public Camera MainCamera
        {
            get
            {
                if (!_cameras.ContainsKey(_mainCamera))
                    return null;

                return _cameras[_mainCamera];
            }
        }

        public CameraManager()
        {
            _cameras = new Dictionary<string, Camera>();

            _mainCamera = "";
        }

        public void AddCamera(string name, Camera c)
        {
            _cameras.Add(name, c);
        }

        public Camera Get(string name)
        {
            if (!_cameras.ContainsKey(name))
                return null;

            return _cameras[name];
        }
    }
}
