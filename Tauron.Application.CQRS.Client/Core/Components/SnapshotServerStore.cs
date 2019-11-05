using System;
using System.Buffers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Tauron.Application.CQRS.Client.Snapshotting;
using Tauron.Application.CQRS.Common.Configuration;
using Tauron.Application.CQRS.Common.Dto;
using Tauron.Application.CQRS.Common.Dto.Persistable;

namespace Tauron.Application.CQRS.Client.Core.Components
{
    public class SnapshotServerStore : ISnapshotStore
    {
        private readonly IOptions<ClientCofiguration> _confOptions;
        private readonly IDispatcherApi _dispatcherApi;

        public SnapshotServerStore(IOptions<ClientCofiguration> confOptions, IDispatcherApi dispatcherApi)
        {
            _confOptions = confOptions;
            _dispatcherApi = dispatcherApi;
        }

        public async Task Get(Guid id, ISnapshotable to)
        {
            var stade = await _dispatcherApi.Get(new ApiObjectId { Id = id.ToString(), ApiKey = _confOptions.Value.ApiKey });
            if (stade?.Data == null) return;

            Deserialize(stade.Data, to);
        }

        private void Deserialize(string data, ISnapshotable snapshotable)
        {
            var reader = new Utf8JsonReader(Convert.FromBase64String(data));
            snapshotable.ReadFrom(ref reader);
        }

        public async Task Save(ISnapshotable snapshot)
        {
            var buffer = new ArrayBufferWriter<byte>(8 * 100);
            Utf8JsonWriter writer = new Utf8JsonWriter(buffer);
            snapshot.WriteTo(writer);

            await _dispatcherApi.Put(new ApiObjectStade
                                  {
                                      ApiKey = _confOptions.Value.ApiKey,
                                      ObjectStade = new ObjectStade
                                                    {
                                                        Data = Convert.ToBase64String(buffer.WrittenSpan),
                                                        Identifer = snapshot.Id.ToString()
                                                    }
                                  });
        }
    }
}