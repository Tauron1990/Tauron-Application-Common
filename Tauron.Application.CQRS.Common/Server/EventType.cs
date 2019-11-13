namespace Tauron.Application.CQRS.Common.Server
{
    public enum EventType
    {
        AmbientEvent,
        CommandResult,
        AmbientCommand,
        Command,
        Event,
        Query,
        QueryResult
    }
}