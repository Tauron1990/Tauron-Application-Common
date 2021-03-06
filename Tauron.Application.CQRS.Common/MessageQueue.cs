﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Tauron.Application.CQRS.Common
{
    public sealed class MessageQueue<TMessage> : IDisposable
    {
        private readonly ManualResetEvent _stopEvent = new ManualResetEvent(false);
        private readonly bool _skipExceptions;
        private readonly BlockingCollection<TMessage> _incomming;
        private readonly BlockingCollection<Task> _processorQueue;

        public event Func<Exception, Task>? OnError;

        public event Func<TMessage, Task>? OnWork;

        private readonly Task _dispatcher;
        private bool _stop;

        public MessageQueue(int maxParallel = int.MaxValue, bool skipExceptions = true)
        {
            _skipExceptions = skipExceptions;

            _incomming = new BlockingCollection<TMessage>();
            _processorQueue = new BlockingCollection<Task>(maxParallel);

            _dispatcher = new Task(async () =>
            {
                foreach (var message in _incomming.GetConsumingEnumerable())
                {
                    try
                    {
                        var worker = OnWork;
                        if (worker == null) continue;

                        _processorQueue.Add(Task.Run(async () => await worker(message)));
                    }
                    catch (Exception e)
                    {
                        await ProcessError(e);

                        if (_skipExceptions) continue;

                        throw;
                    }
                }
            });

            _dispatcher.ContinueWith(t => _processorQueue.CompleteAdding());
        }

        public void Enqueue(TMessage msg) => _incomming.Add(msg);

        public async Task Start()
        {
            if(_stop) 
                throw new InvalidOperationException("Message Queue Stoped");
            if (_dispatcher.Status == TaskStatus.Running)
                throw new InvalidOperationException("Message Queue started");
            
            _dispatcher.Start();

            foreach (var task in _processorQueue.GetConsumingEnumerable())
            {
                try
                {
                    await new SynchronizationContextRemover();
                    await task.ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    await ProcessError(e);

                    if (_skipExceptions) continue;

                    throw;
                }
            }

            _stopEvent.Set();

            OnError = null;
            OnWork = null;
        }

        public void Stop()
        {
            lock (this)
            {
                if(_stop) return;

                _stop = true;
                _incomming.CompleteAdding();
            }

            _stopEvent.WaitOne(100_000);
        }

        public void Dispose()
        {
            _incomming.Dispose();
            _processorQueue.Dispose();
            _dispatcher?.Dispose();
        }

        private async Task ProcessError(Exception e)
        {
            var invoker = OnError;

            if (invoker != null)
                await invoker(e);
        }
    }
}