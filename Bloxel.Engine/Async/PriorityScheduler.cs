/*
 * Bloxel - PriorityScheduler.cs
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
using System.Threading;

namespace Bloxel.Engine.Async
{
    /// <summary>
    /// Implements a task scheduler that runs tasks with lower priority values first.
    /// </summary>
    public class PriorityScheduler : IScheduler
    {
        private List<BloxelTask> _tasks;
        private object _taskSync;
        private Thread[] _workerThreads;

        public bool AllThreadsStopped
        {
            get
            {
                for (int i = 0; i < ThreadCount; i++)
                {
                    if (_workerThreads[i].IsAlive)
                        return false;
                }

                return true;
            }
        }
        public int ThreadCount { get { return _workerThreads.Length; } }

        public PriorityScheduler(int threadCount)
        {
            Contract.Assert(threadCount > 0);

            _tasks = new List<BloxelTask>();
            _taskSync = new object();
            _workerThreads = new Thread[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                _workerThreads[i] = new Thread(Work);
                _workerThreads[i].IsBackground = true;
                _workerThreads[i].Name = "Bloxel Worker Thread #" + i;
            }
        }

        public void Start()
        {
            for (int i = 0; i < ThreadCount; i++)
                _workerThreads[i].Start();
        }

        public void Stop()
        {
            for (int i = 0; i < ThreadCount; i++)
                Schedule(null, 0.0f);
        }

        public void ForceStop()
        {
            for (int i = 0; i < ThreadCount; i++)
                _workerThreads[i].Interrupt();
        }

        public void Update()
        {
            lock (_taskSync)
            {
                for (int i = 0; i < _tasks.Count; i++)
                    _tasks[i].EvaluatePriority();
            }
        }

        public void Schedule(Action action, Func<float> priority)
        {
            Contract.Assert(priority != null);

            BloxelTask task = new BloxelTask(action, priority);

            lock (_taskSync)
            {
                _tasks.Add(task);
                Monitor.Pulse(_taskSync); // signal next worker thread that we have a task ready
            }
        }

        public void Schedule(Action action, float priority)
        {
            Schedule(action, () => priority);
        }

        private BloxelTask NextTask()
        {
            BloxelTask ret = null; // highest priority = task with lowest priority value
            int taskIndex = -1;

            _tasks.RemoveAll(t => t.Priority < 0.0f);

            // assuming the calling thread has a lock...
            for (int i = 0; i < _tasks.Count; i++)
            {
                if (ret == null || _tasks[i].Priority < ret.Priority)
                {
                    ret = _tasks[i];
                    taskIndex = i;
                }
            }

            if(taskIndex >= 0)
                _tasks.RemoveAt(taskIndex); // work like a queue; dequeue it from the list

            return ret;
        }

        private void Work()
        {
            while (true)
            {
                BloxelTask task;

                lock (_taskSync)
                {
                    try
                    {
                        if (_tasks.Count == 0) Monitor.Wait(_taskSync);
                    }
                    catch (ThreadInterruptedException)
                    { break; } // exit

                    task = NextTask();
                }

                Contract.Assert(task != null);

                if (task.Action == null) break; // null = terminate thread

                task.Action();
            }
        }
    }
}
