namespace Rnwood.SmtpServer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ParameterProcessorMap
    {
        private readonly Dictionary<string, IParameterProcessor> processors =
            new Dictionary<string, IParameterProcessor>(StringComparer.OrdinalIgnoreCase);

        public void SetProcessor(string key, IParameterProcessor processor)
        {
            this.processors[key] = processor;
        }

        public IParameterProcessor GetProcessor(string key)
        {
            this.processors.TryGetValue(key, out IParameterProcessor result);
            return result;
        }

        public async Task ProcessAsync(IConnection connection, string[] arguments, bool throwOnUnknownParameter)
        {
            await this.ProcessAsync(connection, new ParameterParser(arguments), throwOnUnknownParameter).ConfigureAwait(false);
        }

        public Task ProcessAsync(IConnection connection, ParameterParser parameters, bool throwOnUnknownParameter)
        {
            foreach (Parameter parameter in parameters.Parameters)
            {
                IParameterProcessor parameterProcessor = this.GetProcessor(parameter.Name);

                if (parameterProcessor != null)
                {
                    parameterProcessor.SetParameter(connection, parameter.Name, parameter.Value);
                }
                else if (throwOnUnknownParameter)
                {
                    throw new SmtpServerException(
                        new SmtpResponse(
                            StandardSmtpResponseCode.SyntaxErrorInCommandArguments,
                                         "Parameter {0} is not recognised", parameter.Name));
                }
            }

            return Task.CompletedTask;
        }
    }
}