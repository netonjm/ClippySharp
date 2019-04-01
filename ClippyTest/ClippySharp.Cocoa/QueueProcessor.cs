using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClippyTest
{
    public class QueueProcessor
    {
        public readonly Queue<Action> queue;
        System.Threading.CancellationTokenSource source;
        public QueueProcessor()
        {
            queue = new Queue<Action>();
            Start();
        }

        public void Start ()
        {
            Stop();

            source = new System.Threading.CancellationTokenSource();
            Task.Run(() =>
            {
                while (!source.IsCancellationRequested)
                {
                    if (queue.Count > 0)
                    {
                        queue.Dequeue()?.Invoke();
                    } else
                    {
                        Thread.Sleep(500);
                    }

                }
            }, source.Token);
        }

        public void Stop ()
        {
            if (source != null)
            {
                source.Cancel();
            }
        }

        public void Enqueue(Action p)
        {
            queue.Enqueue(p);
        }

        public void Dequeue()
        {
            queue.Dequeue();
        }

        public void Clear()
        {
            queue.Clear();
        }
    }
}
