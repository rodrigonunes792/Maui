using SoftwareShow.Contagem.MApp.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareShow.Contagem.MApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IRestService _restService;

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

        public LoginViewModel(IRestService restService)
        {
            _restService = restService;
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
