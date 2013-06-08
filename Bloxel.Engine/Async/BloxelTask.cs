/*
 * Bloxel - BloxelTask.cs
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
