namespace Tauron.Application.CQRS.Common.Server
{
    public enum EventType
    {
        Unkowen,
        CommandResult,
        AmbientCommand,
        Event,
        AmbientEvent,
        Query,
        QueryResult
    }
}