using Newtonsoft.Json;
using SoftwareShow.Contagem.MApp.Interfaces;
using SoftwareShow.Contagem.MApp.Models;
using SoftwareShow.Contagem.MApp.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SoftwareShow.Contagem.MApp.ViewModels
{
    public class ContagemViewModel: INotifyPropertyChanged
    {
        private readonly IDatabaseService _databaseService;

        private ObservableCollection<Atividade> _atividades = new();
        private Atividade? _atividadeSelecionada;
        private string _complemento = string.Empty;
        private string _responsavel = string.Empty;
        private bool _isLoading = false;
        private DateTime _dataHora = DateTime.Now;
        private LojaUsuario? _lojaSelecionada;

        public ContagemViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;

            IniciarCommand = new Command(async () => await OnIniciar(), () => PodeIniciar);
            CancelarCommand = new Command(async () => await OnCancelar());

            // Configurar data/hora atual
            _dataHora = DateTime.Now;
        }

        // Construtor sem parâmetros para fallback
        public ContagemViewModel() : this(new DatabaseService()) { }

        #region Propriedades

        public ObservableCollection<Atividade> Atividades
        {
            get => _atividades;
            set
            {
                _atividades = value;
                OnPropertyChanged();
            }
        }

        public Atividade? AtividadeSelecionada
        {
            get => _atividadeSelecionada;
            set
            {
                _atividadeSelecionada = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PodeIniciar));
                ((Command)IniciarCommand).ChangeCanExecute();
            }
        }

        public string Complemento
        {
            get => _complemento;
            set
            {
                _complemento = value;
                OnPropertyChanged();
            }
        }

        public string Responsavel
        {
            get => _responsavel;
            set
            {
                _responsavel = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PodeIniciar));
                ((Command)IniciarCommand).ChangeCanExecute();
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

        public string DataTexto => _dataHora.ToString("dd/MM/yyyy");
        public string HoraTexto => _dataHora.ToString("HH:mm");

        public string LojaTexto
        {
            get
            {
                if (_lojaSelecionada != null)
                {
                    return $"{_lojaSelecionada.COD_LOJA:D4} - {_lojaSelecionada.NOME_LOJA}";
                }
                return "Loja não selecionada";
            }
        }

        public bool PodeIniciar =>
            !IsLoading &&
            AtividadeSelecionada != null &&
            !string.IsNullOrWhiteSpace(Responsavel);

        #endregion

        #region Commands

        public ICommand IniciarCommand { get; }
        public ICommand CancelarCommand { get; }

        #endregion

        #region Métodos

        public async Task CarregarDadosAsync()
        {
            try
            {
                IsLoading = true;

                // Carregar responsável do SecureStorage
                var username = await SecureStorage.GetAsync("username");
                if (!string.IsNullOrEmpty(username))
                {
                    Responsavel = username;
                }

                // Carregar loja selecionada
                var lojaJson = await SecureStorage.GetAsync("selected_store");
                if (!string.IsNullOrEmpty(lojaJson))
                {
                    _lojaSelecionada = JsonConvert.DeserializeObject<LojaUsuario>(lojaJson);
                    OnPropertyChanged(nameof(LojaTexto));
                }

                // Carregar atividades do banco local
                var atividades = await _databaseService.GetAllAsync<Atividade>();

                Atividades.Clear();
                foreach (var atividade in atividades.OrderBy(a => a.Nome))
                {
                    Atividades.Add(atividade);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar dados: {ex.Message}");
                // Aqui você pode mostrar um alert para o usuário
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task OnIniciar()
        {
            if (!PodeIniciar || _lojaSelecionada == null || AtividadeSelecionada == null)
                return;

            try
            {
                IsLoading = true;

                // Criar novo modelo de contagem
                var novaContagem = new ContagemModel
                {
                    Nome = Complemento, // Usando complemento como nome
                    Descricao = Complemento,
                    Responsavel = Responsavel,
                    AtividadeId = AtividadeSelecionada.Id,
                    CodigoLoja = _lojaSelecionada.COD_LOJA,
                    DataHora = _dataHora,
                    Atividade = _atividadeSelecionada,
                    DataCorrigida = _dataHora.Date,
                    Inventario = "0", // Padrão não é inventário
                    Excluido = "0",
                    VersaoContagem = "1.0" // Ou a versão que vocês usam
                };

                // Salvar no banco local
                await _databaseService.InsertAsync(novaContagem);

                // Navegar para a próxima tela (você define qual)
                // Exemplo: tela de itens da contagem
                await Shell.Current.GoToAsync("//ContagemListPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao criar contagem: {ex.Message}");

                // Mostrar erro para o usuário
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Erro",
                        "Erro ao criar a contagem. Tente novamente.", "OK");
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task OnCancelar()
        {
            await Shell.Current.GoToAsync("..");
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
