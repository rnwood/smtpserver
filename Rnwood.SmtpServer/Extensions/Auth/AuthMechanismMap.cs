namespace Rnwood.SmtpServer.Extensions.Auth
{
    using System;
    using System.Collections.Generic;

    public class AuthMechanismMap
    {
        private readonly Dictionary<string, IAuthMechanism> map = new Dictionary<string, IAuthMechanism>(StringComparer.OrdinalIgnoreCase);

        public void Add(IAuthMechanism mechanism)
        {
            this.map[mechanism.Identifier] = mechanism;
        }

        public IAuthMechanism Get(string identifier)
        {
            this.map.TryGetValue(identifier, out IAuthMechanism result);

            return result;
        }

        public IEnumerable<IAuthMechanism> GetAll()
        {
            return this.map.Values;
        }
    }
}