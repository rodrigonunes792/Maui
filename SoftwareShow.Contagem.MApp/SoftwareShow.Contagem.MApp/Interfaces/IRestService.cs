using SoftwareShow.Contagem.MApp.Models;

namespace SoftwareShow.Contagem.MApp.Interfaces
{
    public interface IRestService
    {
        Task<Autenticacao> AutenticarAsync(string email, string senha, string versaoApp);
    }
}
