namespace Rnwood.SmtpServer
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    public class SmtpCommand : IEquatable<SmtpCommand>
    {
        private readonly static Regex COMMANDREGEX = new Regex("(?'verb'[^ :]+)[ :]*(?'arguments'.*)");

        public SmtpCommand(string text)
        {
            this.Text = text;

            this.IsValid = false;
            this.IsEmpty = true;

            if (!string.IsNullOrEmpty(text))
            {
                Match match = COMMANDREGEX.Match(text);

                if (match.Success)
                {
                    this.Verb = match.Groups["verb"].Value;
                    this.ArgumentsText = match.Groups["arguments"].Value ?? "";
                    this.IsValid = true;
                }
            }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; private set; }

        public string ArgumentsText { get; private set; }

        public string Verb { get; private set; }

        public bool IsValid { get; private set; }

        public bool IsEmpty { get; private set; }

        public bool Equals(SmtpCommand other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.Text, this.Text);
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

            if (obj.GetType() != typeof(SmtpCommand))
            {
                return false;
            }

            return this.Equals((SmtpCommand)obj);
        }

        public override int GetHashCode()
        {
            return (this.Text != null ? this.Text.GetHashCode() : 0);
        }
    }
}