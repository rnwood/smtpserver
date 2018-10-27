namespace Rnwood.SmtpServer
{
    using System;
    using System.Text;

    public class SmtpResponse : IEquatable<SmtpResponse>
    {
        public SmtpResponse(int code, string message, params object[] args)
        {
            this.Code = code;
            this.Message = string.Format(message, args);
        }

        public SmtpResponse(StandardSmtpResponseCode code, string message, params object[] args)
            : this((int)code, message, args)
        {
        }

        public int Code { get; private set; }

        public string Message { get; private set; }

        public bool IsError => this.Code >= 500 && this.Code <= 599;

        public bool IsSuccess => this.Code >= 200 && this.Code <= 299;

        /// <summary>
        /// Returns a <see cref="string"/> that represents the response.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the response.</returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            string[] lines = this.Message.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);

            for (int l = 0; l < lines.Length; l++)
            {
                string line = lines[l];

                if (l == lines.Length - 1)
                {
                    result.AppendLine(this.Code + " " + line);
                }
                else
                {
                    result.AppendLine(this.Code + "-" + line);
                }
            }

            return result.ToString();
        }

        public bool Equals(SmtpResponse other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.Code == this.Code && Equals(other.Message, this.Message);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(SmtpResponse))
            {
                return false;
            }

            return this.Equals((SmtpResponse)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Code * 397) ^ (this.Message != null ? this.Message.GetHashCode() : 0);
            }
        }
    }
}