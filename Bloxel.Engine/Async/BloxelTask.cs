/*
 * Bloxel - BloxelTask.cs
 * Copyright (c) 2013 Tony "untitled" Peng
 * <http://www.tonypeng.com/>
 * 
 * This file is subject to the terms and conditions defined in the
 * file 'LICENSE.txt', which is part of this source code package.
 */

using System;
using System.Diagnostics.Contracts;

namespace Bloxel.Engine.Async
{
    internal class BloxelTask
    {
        private Action _action;
        private Func<float> _priorityEvaluator;

        private float _priority;

        public Action Action
        {
            get { return _action; }
        }

        public float Priority
        {
            get { return _priority; }
            private set { _priority = value; }
        }

        internal BloxelTask(Action action, Func<float> priority)
        {
            Contract.Assert(priority != null);
            
            _action = action;
            _priorityEvaluator = priority;

            EvaluatePriority();
        }

        public float EvaluatePriority()
        {
            return (_priority = _priorityEvaluator());
        }
    }
}
