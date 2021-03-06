﻿namespace ServiceManager.Core.Core
{
    public class LogEntries : ObservableConcurrentDictionary<string, ObservableQueue<string>>
    {
        public void AddLog(string? name, string? content)
        {
            if(name == null || content == null) return;

            var queue = GetOrAdd(name, s => new ObservableQueue<string>());

            lock (queue)
            {
                queue.Enqueue(content);
                if (queue.Count > 100)
                    queue.Dequeue();
            }
        }
    }
}