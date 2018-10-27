namespace Rnwood.SmtpServer
{
    using System.Threading.Tasks;

    public interface IParameterProcessor
    {
        Task SetParameter(IConnection connection, string key, string value);
    }
}