namespace Rnwood.SmtpServer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using Rnwood.SmtpServer.Extensions.Auth;

    public abstract class AbstractSession : IEditableSession
    {
        public AbstractSession(IPAddress clientAddress, DateTime startDate)
        {
            this.messages = new List<IMessage>();
            this.ClientAddress = clientAddress;
            this.StartDate = startDate;
        }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public IPAddress ClientAddress { get; set; }

        public string ClientName { get; set; }

        public bool SecureConnection { get; set; }

        public abstract TextReader GetLog();

        public abstract void Dispose();

        public IMessage[] GetMessages()
        {
            return this.messages.ToArray();
        }

        public void AddMessage(IMessage message)
        {
            this.messages.Add(message);
        }

        private List<IMessage> messages;

        public bool CompletedNormally { get; set; }

        public bool Authenticated { get; set; }

        public IAuthenticationCredentials AuthenticationCredentials { get; set; }

        public Exception SessionError { get; set; }

        public SessionErrorType SessionErrorType { get; set; }

        public abstract void AppendToLog(string text);
    }
}