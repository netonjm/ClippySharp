using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClippySharp
{
    public class QueueProcessor
    {
        const int QueueProcessorDelay = 500;
        public readonly Queue<Action> queue;
        CancellationTokenSource source;

        public QueueProcessor()
        {
            queue = new Queue<Action>();
            Start();
        }

        public void Start ()
        {
            Stop();

            source = new CancellationTokenSource();
            Task.Run(() =>
            {
                while (!source.IsCancellationRequested)
                {
                    if (queue.Count > 0)
                    {
                        queue.Dequeue()?.Invoke();
                    } else
                    {
                        Thread.Sleep(QueueProcessorDelay);
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
