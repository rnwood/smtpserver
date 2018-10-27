namespace Rnwood.SmtpServer
{
    using System;
    using System.Collections.Generic;

    public class ParameterParser
    {
        private readonly List<Parameter> parameters = new List<Parameter>();

        public ParameterParser(params string[] arguments)
        {
            this.Parse(arguments);
        }

        public Parameter[] Parameters => this.parameters.ToArray();

        private void Parse(string[] tokens)
        {
            foreach (string token in tokens)
            {
                string[] tokenParts = token.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string key = tokenParts[0];
                string value = tokenParts.Length > 1 ? tokenParts[1] : null;
                this.parameters.Add(new Parameter(key, value));
            }
        }
    }
}