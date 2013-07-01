/*
 * Bloxel - IScheduler.cs
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

namespace Bloxel.Engine.Async
{
    /// <summary>
    /// Provides an interface for a task scheduler.
    /// </summary>
    public interface IScheduler
    {
        bool AllThreadsStopped { get; }
        int ThreadCount { get; }

        void Start();
        void Stop();
        void ForceStop();

        void Update();

        void Schedule(Action action, Func<float> priority);
        void Schedule(Action action, float priority);
    }
}
