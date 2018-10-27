namespace Rnwood.SmtpServer.Verbs
{
    using System;
    using System.Collections.Generic;

    public class VerbMap : IVerbMap
    {
        private readonly Dictionary<string, IVerb> processorVerbs = new Dictionary<string, IVerb>(StringComparer.CurrentCultureIgnoreCase);

        public void SetVerbProcessor(string verb, IVerb verbProcessor)
        {
            this.processorVerbs[verb] = verbProcessor;
        }

        public IVerb GetVerbProcessor(string verb)
        {
            this.processorVerbs.TryGetValue(verb, out IVerb result);
            return result;
        }
    }
}