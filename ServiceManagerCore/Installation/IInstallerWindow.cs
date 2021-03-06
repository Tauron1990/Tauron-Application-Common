﻿using System;
using System.Threading.Tasks;

namespace ServiceManager.Core.Installation
{
    public interface IInstallWindow
    {
        event Func<Task> OnLoad;

        object DataContext { set; }

        Task InvokeAsync(Action action);

        Task<T> InvokeAsync<T>(Func<T> action);

        void SetResult(bool result);
        bool? ShowDialog();
    }
}