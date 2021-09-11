using System.Collections.Generic;

namespace Rnwood.SmtpServer.Extensions.Auth
{
	/// <summary>
	/// Authentication Mechanisms
	/// </summary>
	public class AuthMechanisms
	{
		/// <summary>
		/// Return enumerable of all valid Auth Mechanisms.
		/// </summary>
		public static IEnumerable<IAuthMechanism> All()
		{
			yield return new CramMd5Mechanism();
			yield return new PlainMechanism();
			yield return new LoginMechanism();
			yield return new AnonymousMechanism();
		}
	}
}