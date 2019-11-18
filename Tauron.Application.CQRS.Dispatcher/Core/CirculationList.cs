using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Tauron.Application.CQRS.Dispatcher.Core
{
    public sealed class CirculationList
    {
        private readonly ConcurrentStack<string> _buffer = new ConcurrentStack<string>();

        public CirculationList(IEnumerable<string> content) 
            => _buffer.PushRange(content.ToArray());

        public CirculationList Replace(IEnumerable<string> content)
        {
            _buffer.Clear();

            _buffer.PushRange(content.ToArray());

            return this;
        }

        public string? GetNext()
        {
            if (!_buffer.TryPop(out var element)) return null;
            
            _buffer.Push(element);

            return element;

        }
    }
}