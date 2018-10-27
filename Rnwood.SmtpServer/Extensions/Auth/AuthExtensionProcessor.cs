namespace Rnwood.SmtpServer.Extensions.Auth
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AuthExtensionProcessor : IExtensionProcessor
    {
        private readonly IConnection connection;

        public AuthExtensionProcessor(IConnection connection)
        {
            this.connection = connection;
            this.MechanismMap = new AuthMechanismMap();
            this.MechanismMap.Add(new CramMd5Mechanism());
            this.MechanismMap.Add(new PlainMechanism());
            this.MechanismMap.Add(new LoginMechanism());
            this.MechanismMap.Add(new AnonymousMechanism());
            connection.VerbMap.SetVerbProcessor("AUTH", new AuthVerb(this));
        }

        public AuthMechanismMap MechanismMap { get; private set; }

        protected async Task<IEnumerable<IAuthMechanism>> GetEnabledAuthMechanisms()
        {
            List<IAuthMechanism> result = new List<IAuthMechanism>();

            foreach (var mechanism in this.MechanismMap.GetAll())
            {
                if (await this.connection.Server.Behaviour.IsAuthMechanismEnabled(this.connection, mechanism).ConfigureAwait(false))
                {
                    result.Add(mechanism);
                }
            }

            return result;
        }

        public async Task<string[]> GetEHLOKeywords()
        {

            IEnumerable<IAuthMechanism> mechanisms = await this.GetEnabledAuthMechanisms().ConfigureAwait(false);

            if (mechanisms.Any())
            {
                string mids = string.Join(" ", mechanisms);

                return new[]
                           {
                                   "AUTH=" + mids,
                                   "AUTH " + mids
                               };
            }
            else
            {
                return Array.Empty<string>();
            }

        }

        public async Task<bool> IsMechanismEnabled(IAuthMechanism mechanism)
        {
            return await this.connection.Server.Behaviour.IsAuthMechanismEnabled(this.connection, mechanism).ConfigureAwait(false);
        }
    }
}