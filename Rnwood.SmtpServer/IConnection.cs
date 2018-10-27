namespace Rnwood.SmtpServer
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Rnwood.SmtpServer.Extensions;
    using Rnwood.SmtpServer.Verbs;

    public interface IConnection
    {
        IServer Server { get; }

        IExtensionProcessor[] ExtensionProcessors { get; }

        IVerbMap VerbMap { get; }

        MailVerb MailVerb { get; }

        IEditableSession Session { get; }

        IMessageBuilder CurrentMessage { get; }

        Encoding ReaderEncoding { get; }

        void SetReaderEncoding(Encoding encoding);

        Task SetReaderEncodingToDefault();

        Task CloseConnection();

        Task ApplyStreamFilter(Func<Stream, Task<Stream>> filter);

        Task WriteResponse(SmtpResponse response);

        Task<string> ReadLine();

        Task<IMessageBuilder> NewMessage();

        Task CommitMessage();

        Task AbortMessage();
    }
}