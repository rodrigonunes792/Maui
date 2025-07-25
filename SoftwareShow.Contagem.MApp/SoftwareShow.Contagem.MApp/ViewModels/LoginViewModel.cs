using SoftwareShow.Contagem.MApp.Interfaces;
using SoftwareShow.Contagem.MApp.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SoftwareShow.Contagem.MApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IRestService _restService;
        private readonly IDatabaseService _databaseService;
        private string _usuario = string.Empty;
        private string _senha = string.Empty;
        private bool _isLoading = false;
        private bool _isPasswordVisible = false;

        public string Usuario
        {
            get => _usuario;
            set
            {
                _usuario = value;
                OnPropertyChanged();
            }
        }

        public string Senha
        {
            get => _senha;
            set
            {
                _senha = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged();
            }
        }

        public LoginViewModel(IRestService restService, IDatabaseService databaseService)
        {
            _restService = restService;
            _databaseService = databaseService;
        }

        public async Task<(bool Success, string Message)> LoginAsync()
        {
            // Validação básica
            if (string.IsNullOrWhiteSpace(Usuario))
            {
                return (false, "Informe o usuário");
            }

            if (string.IsNullOrWhiteSpace(Senha))
            {
                return (false, "Informe a senha");
            }

            // Verificar se API configurada
            var apiUrl = await SecureStorage.GetAsync("api_url");
            if (string.IsNullOrEmpty(apiUrl))
            {
                return (false, "Configure o endereço da API primeiro");
            }

            IsLoading = true;

            try
            {
                var autenticacao = await _restService.AutenticarAsync(Usuario, Senha, "1.0.0");

                if (autenticacao.Autenticado)
                {
                    // Salvar dados essenciais
                    await SecureStorage.SetAsync("auth_token", autenticacao.Autorizacao ?? "");
                    await SecureStorage.SetAsync("username", autenticacao.Usuario ?? "");
                    await SecureStorage.SetAsync("auth_datetime", autenticacao.DataHora.ToString("yyyy-MM-dd HH:mm:ss"));

                    // Salvar lojas se houver
                    if (autenticacao.Lojas?.Any() == true)
                    {
                        var lojasJson = Newtonsoft.Json.JsonConvert.SerializeObject(autenticacao.Lojas);
                        await SecureStorage.SetAsync("user_lojas", lojasJson);
                        await SecureStorage.SetAsync("total_lojas", autenticacao.Lojas.Count().ToString());
                    }
                    await GetDatabase(autenticacao.Limpar);
                    return (true, "Login realizado com sucesso!");
                }
                else
                {
                    var mensagem = string.IsNullOrEmpty(autenticacao.Mensagem) ?
                        "Usuário ou senha incorretos" : autenticacao.Mensagem;
                    return (false, mensagem);
                }
            }
            catch (Exception)
            {
                return (false, "Erro de conexão. Verifique a URL da API.");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task GetDatabase(bool clear)
        {
            var db = await _databaseService.GetConnectionAsync();
            if (clear)
            {
                

                await db.DropTableAsync<ContagemModel>();
                await db.DropTableAsync<CodigoBarras>();
                await db.DropTableAsync<Produto>();
                await db.DropTableAsync<Kit>();
                await db.DropTableAsync<Preco>();
                await db.DropTableAsync<Atividade>();
                //db.DropTableAsync<ProdutosInventario>();

            }
            await db.CreateTableAsync<ContagemModel>();
            await db.CreateTableAsync<CodigoBarras>();
            await db.CreateTableAsync<Produto>();
            await db.CreateTableAsync<Kit>();
            await db.CreateTableAsync<Preco>();
            await db.CreateTableAsync<Atividade>();
        }

        public void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
