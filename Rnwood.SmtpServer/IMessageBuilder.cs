namespace Rnwood.SmtpServer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public interface IMessageBuilder
    {
        Task<Stream> WriteData();

        ISession Session { get; set; }

        DateTime ReceivedDate { get; set; }

        string From { get; set; }

        ICollection<string> To { get; }

        bool SecureConnection { get; set; }

        bool EightBitTransport { get; set; }

        long? DeclaredMessageSize { get; set; }

        Task<IMessage> ToMessage();

        Task<Stream> GetData();
    }
}