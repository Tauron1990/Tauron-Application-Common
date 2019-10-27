namespace Tauron.Application.CQRS.Common.Server
{
    public enum EventType
    {
        Unkowen,
        Command,
        AmbientCommand,
        Event,
        AmbientEvent,
        Query,
        QueryResult
    }
}