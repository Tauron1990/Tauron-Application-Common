using System;
using System.Threading;
using System.Threading.Tasks;
using Tauron.Application.CQRS.Extensions.ServiceControl;

namespace ServiceManager.Core.Core
{
    public class ServiceStopWaiter: IDisposable
    {
        private readonly ManualResetEvent _manualReset = new ManualResetEvent(false);
        private readonly string _targetService;

        public ServiceStopWaiter(string targetService)
        {
            _targetService = targetService;
            ServiceStopHandler.ServiceStoppedEvent += ServiceStopHandlerOnServiceStoppedEvent;
        }

        private Task ServiceStopHandlerOnServiceStoppedEvent(ServiceStoppedEvent arg)
        {
            if(arg.ServiceName == _targetService) _manualReset.Set();

            return Task.CompletedTask;
        }

        public Task Wait(int timeout)
        {
            _manualReset.WaitOne(timeout);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _manualReset.Dispose();
            ServiceStopHandler.ServiceStoppedEvent -= ServiceStopHandlerOnServiceStoppedEvent;
        }
    }
}