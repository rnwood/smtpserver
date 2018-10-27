namespace Rnwood.SmtpServer
{
    using System;

    public interface IServer : IDisposable
    {
        IServerBehaviour Behaviour { get; }
    }
}