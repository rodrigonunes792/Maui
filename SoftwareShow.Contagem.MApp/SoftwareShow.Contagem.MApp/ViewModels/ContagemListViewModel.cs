using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SoftwareShow.Contagem.MApp.Models;
using SoftwareShow.Contagem.MApp.Interfaces;
using SoftwareShow.Contagem.MApp.Service;
using System.Linq;
using System.IO;
using SoftwareShow.Contagem.MApp.Pages;


namespace SoftwareShow.Contagem.MApp.ViewModels
{
    public class ContagemListViewModel : INotifyPropertyChanged
    {
        private readonly IDatabaseService _databaseService;
        private readonly IRestService _restService;
        private readonly IExportService _exportService;

        private List<ContagemModel> _allContagens = new();
        private ObservableCollection<ContagemModel> _contagensEmAndamento = new();
        private ObservableCollection<ContagemModel> _contagensEnviadas = new();
        private ContagemModel _selectedContagem = null;

        private string _searchText = string.Empty;
        private bool _isEmAndamentoTabSelected = true;
        private bool _isLoading = false;
        private bool _isSending = false;
        private string _statusMessage = string.Empty;

        #region Propriedades Públicas

        public ObservableCollection<ContagemModel> ContagensEmAndamento
        {
            get => _contagensEmAndamento;
            set
            {
                _contagensEmAndamento = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ContagemModel> ContagensEnviadas
        {
            get => _contagensEnviadas;
            set
            {
                _contagensEnviadas = value;
                OnPropertyChanged();
            }
        }

        public ContagemModel SelectedContagem
        {
            get => _selectedContagem;
            set
            {
                _selectedContagem = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSelectionMode));
                OnPropertyChanged(nameof(SelectedContagemInfo));
                OnPropertyChanged(nameof(CanShare));
                OnPropertyChanged(nameof(CanSend));
            }
        }

        public bool IsSelectionMode => _selectedContagem != null;

        public string SelectedContagemInfo
        {
            get
            {
                if (_selectedContagem != null)
                    return $"#{_selectedContagem.Codigo} - {_selectedContagem.Nome}";
                return string.Empty;
            }
        }

        public bool CanShare => _selectedContagem != null && _selectedContagem.QuantidadeItens > 0;

        public bool CanSend => IsEmAndamentoTabSelected && _selectedContagem != null &&
                              _selectedContagem.QuantidadeItens > 0 && _selectedContagem.PodeEnviar;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterContagens();
            }
        }

        public bool IsEmAndamentoTabSelected
        {
            get => _isEmAndamentoTabSelected;
            set
            {
                _isEmAndamentoTabSelected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEnviadasTabSelected));
                OnPropertyChanged(nameof(CanSend));
                ClearSelection();
                FilterContagens();
            }
        }

        public bool IsEnviadasTabSelected => !_isEmAndamentoTabSelected;

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool IsSending
        {
            get => _isSending;
            set
            {
                _isSending = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public bool ShowEmptyView
        {
            get
            {
                if (IsEmAndamentoTabSelected)
                    return ContagensEmAndamento.Count == 0;
                else
                    return ContagensEnviadas.Count == 0;
            }
        }

        #endregion

        #region Commands

        public ICommand SearchCommand { get; }
        public ICommand EmAndamentoTabCommand { get; }
        public ICommand EnviadasTabCommand { get; }
        public ICommand SelectContagemCommand { get; }
        public ICommand EditContagemCommand { get; }
        public ICommand DeleteContagemCommand { get; }
        public ICommand ShareExcelCommand { get; }
        public ICommand SharePdfCommand { get; }
        public ICommand SendContagemCommand { get; }
        public ICommand CreateNewContagemCommand { get; }
        public ICommand ClearSelectionCommand { get; }
        public ICommand RefreshCommand { get; }

        #endregion

        public ContagemListViewModel(IDatabaseService databaseService, IRestService restService, IExportService exportService)
        {
            _databaseService = databaseService;
            _restService = restService;
            _exportService = exportService;

            // Inicializar Commands
            SearchCommand = new Command<string>(OnSearchTextChanged);
            EmAndamentoTabCommand = new Command(() => IsEmAndamentoTabSelected = true);
            EnviadasTabCommand = new Command(() => IsEmAndamentoTabSelected = false);
            SelectContagemCommand = new Command<ContagemModel>(OnSelectContagem);
            EditContagemCommand = new Command<ContagemModel>(OnEditContagem, CanExecuteEdit);
            DeleteContagemCommand = new Command<ContagemModel>(OnDeleteContagem, CanExecuteDelete);
            ShareExcelCommand = new Command<ContagemModel>(OnShareExcel, CanExecuteShare);
            SharePdfCommand = new Command<ContagemModel>(OnSharePdf, CanExecuteShare);
            SendContagemCommand = new Command<ContagemModel>(OnSendContagem, CanExecuteSend);
            CreateNewContagemCommand = new Command(OnCreateNewContagem);
            ClearSelectionCommand = new Command(ClearSelection);
            RefreshCommand = new Command(async () => await LoadContagensAsync());

            LoadContagensAsync();
        }

        // Construtor sem parâmetros para fallback
        public ContagemListViewModel() : this(new DatabaseService(), null, new ExportService()) { }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Métodos Privados

        private async Task LoadContagensAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Carregando contagens...";

                _allContagens = await _databaseService.GetAllAsync<ContagemModel>();

                // Carregar atividades relacionadas se necessário
                foreach (var contagem in _allContagens)
                {
                    if (contagem.AtividadeId > 0 && contagem.Atividade == null)
                    {
                        //contagem.Atividade = await _databaseService.GetByIdAsync<Atividade>(contagem.AtividadeId);
                    }
                }

                FilterContagens();
                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao carregar contagens: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Erro LoadContagensAsync: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterContagens()
        {
            List<ContagemModel> filteredEmAndamento;
            List<ContagemModel> filteredEnviadas;

            if (string.IsNullOrWhiteSpace(_searchText))
            {
                filteredEmAndamento = _allContagens.Where(c => !c.Enviada && !c.IsExcluido).ToList();
                filteredEnviadas = _allContagens.Where(c => c.Enviada && !c.IsExcluido).ToList();
            }
            else
            {
                var searchLower = _searchText.ToLowerInvariant();

                filteredEmAndamento = _allContagens
                    .Where(c => !c.Enviada && !c.IsExcluido && MatchesSearch(c, searchLower))
                    .ToList();

                filteredEnviadas = _allContagens
                    .Where(c => c.Enviada && !c.IsExcluido && MatchesSearch(c, searchLower))
                    .ToList();
            }

            // Atualizar coleções
            ContagensEmAndamento.Clear();
            foreach (var contagem in filteredEmAndamento.OrderByDescending(c => c.DataHora))
            {
                ContagensEmAndamento.Add(contagem);
            }

            ContagensEnviadas.Clear();
            foreach (var contagem in filteredEnviadas.OrderByDescending(c => c.DataHoraEnvio ?? c.DataHora))
            {
                ContagensEnviadas.Add(contagem);
            }

            OnPropertyChanged(nameof(ShowEmptyView));
        }

        private bool MatchesSearch(ContagemModel contagem, string searchLower)
        {
            return contagem.Codigo.ToString().Contains(searchLower) ||
                   contagem.Id.ToString().Contains(searchLower) ||
                   contagem.Nome?.ToLowerInvariant().Contains(searchLower) == true ||
                   contagem.Atividade?.Nome?.ToLowerInvariant().Contains(searchLower) == true ||
                   contagem.DataHora.ToString("dd/MM/yyyy").Contains(searchLower);
        }

        #endregion

        #region Command Handlers

        private void OnSearchTextChanged(string searchText)
        {
            SearchText = searchText ?? string.Empty;
        }

        private void OnSelectContagem(ContagemModel contagem)
        {
            if (contagem == null) return;

            // Se for a mesma contagem, desseleciona
            if (_selectedContagem == contagem)
            {
                SelectedContagem = null;
                System.Diagnostics.Debug.WriteLine($"Contagem {contagem.Codigo} desmarcada");
            }
            else
            {
                // Seleciona apenas uma contagem
                SelectedContagem = contagem;
                System.Diagnostics.Debug.WriteLine($"Contagem {contagem.Codigo} selecionada");
            }
        }

        public void ClearSelection()
        {
            SelectedContagem = null;
        }

        private bool CanExecuteEdit(ContagemModel contagem)
        {
            return contagem != null && !contagem.Enviada && !contagem.IsExcluido;
        }

        private async void OnEditContagem(ContagemModel contagem)
        {
            if (contagem == null) return;

            try
            {
                StatusMessage = "Abrindo editor...";
                await Task.Delay(500); // Feedback visual
                ContagemItemPage.ContagemParaEditar = contagem;
                await Shell.Current.GoToAsync("//ContagemItemPage");
                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao editar contagem: {ex.Message}";
                await Task.Delay(3000);
                StatusMessage = string.Empty;
            }
        }

        private bool CanExecuteDelete(ContagemModel contagem)
        {
            return contagem != null && !contagem.Enviada && !contagem.IsExcluido;
        }

        private async void OnDeleteContagem(ContagemModel contagem)
        {
            if (contagem == null) return;

            try
            {
                var result = await Shell.Current.DisplayAlert(
                    "Confirmar Exclusão",
                    $"Deseja excluir a contagem {contagem.Codigo}?\n\nEsta ação não pode ser desfeita.",
                    "Excluir", "Cancelar");

                if (result)
                {
                    StatusMessage = "Excluindo contagem...";

                    contagem.MarcarComoExcluida();
                    await _databaseService.UpdateAsync(contagem);
                    await LoadContagensAsync();

                    StatusMessage = "Contagem excluída com sucesso";
                    await Task.Delay(2000);
                    StatusMessage = string.Empty;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao excluir contagem: {ex.Message}";
                await Task.Delay(3000);
                StatusMessage = string.Empty;
            }
        }

        private bool CanExecuteShare(ContagemModel contagem)
        {
            return contagem != null && contagem.QuantidadeItens > 0;
        }

        private async void OnShareExcel(ContagemModel contagem = null)
        {
            var contagemParaCompartilhar = contagem ?? _selectedContagem;
            if (!CanExecuteShare(contagemParaCompartilhar) || _exportService == null) return;

            try
            {
                StatusMessage = "Gerando arquivo Excel...";
                System.Diagnostics.Debug.WriteLine($"📊 Gerando Excel para contagem: {contagemParaCompartilhar.Codigo}");

                // Gerar arquivo Excel para uma contagem
                var filePath = await _exportService.GerarExcelAsync(new List<ContagemModel> { contagemParaCompartilhar });

                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    StatusMessage = "Compartilhando arquivo Excel...";

                    // Compartilhar arquivo
                    await _exportService.CompartilharArquivoAsync(filePath, $"Contagem {contagemParaCompartilhar.Codigo} - Excel");

                    StatusMessage = "✅ Arquivo Excel compartilhado com sucesso!";
                    System.Diagnostics.Debug.WriteLine($"✅ Excel gerado e compartilhado: {filePath}");

                    // Se foi do header, limpar seleção
                    if (contagem == null) ClearSelection();
                }
                else
                {
                    throw new Exception("Falha na geração do arquivo Excel");
                }

                await Task.Delay(2000);
                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Erro ao gerar Excel: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"❌ Erro OnShareExcel: {ex}");

                await Task.Delay(3000);
                StatusMessage = string.Empty;
            }
        }

        private async void OnSharePdf(ContagemModel contagem = null)
        {
            var contagemParaCompartilhar = contagem ?? _selectedContagem;
            if (!CanExecuteShare(contagemParaCompartilhar) || _exportService == null) return;

            try
            {
                StatusMessage = "Gerando arquivo PDF...";
                System.Diagnostics.Debug.WriteLine($"📄 Gerando PDF para contagem: {contagemParaCompartilhar.Codigo}");

                // Gerar arquivo PDF para uma contagem
                var filePath = await _exportService.GerarPdfAsync(new List<ContagemModel> { contagemParaCompartilhar });

                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    StatusMessage = "Compartilhando arquivo PDF...";

                    // Compartilhar arquivo
                    await _exportService.CompartilharArquivoAsync(filePath, $"Contagem {contagemParaCompartilhar.Codigo} - PDF");

                    StatusMessage = "✅ Arquivo PDF compartilhado com sucesso!";
                    System.Diagnostics.Debug.WriteLine($"✅ PDF gerado e compartilhado: {filePath}");

                    // Se foi do header, limpar seleção
                    if (contagem == null) ClearSelection();
                }
                else
                {
                    throw new Exception("Falha na geração do arquivo PDF");
                }

                await Task.Delay(2000);
                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Erro ao gerar PDF: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"❌ Erro OnSharePdf: {ex}");

                await Task.Delay(3000);
                StatusMessage = string.Empty;
            }
        }

        private bool CanExecuteSend(ContagemModel contagem)
        {
            return contagem != null && contagem.QuantidadeItens > 0 && contagem.PodeEnviar;
        }

        private async void OnSendContagem(ContagemModel contagem = null)
        {
            var contagemParaEnviar = contagem ?? _selectedContagem;
            if (!CanExecuteSend(contagemParaEnviar) || _restService == null) return;

            try
            {
                IsSending = true;
                StatusMessage = $"Enviando contagem {contagemParaEnviar.Codigo}...";

                var resultado = await _restService.EnviarContagemAsync(contagemParaEnviar);

                if (resultado > 0)
                {
                    contagemParaEnviar.MarcarComoEnviada(resultado);
                    await _databaseService.UpdateAsync(contagemParaEnviar);

                    StatusMessage = $"✅ Contagem {contagemParaEnviar.Codigo} enviada com sucesso!";
                    System.Diagnostics.Debug.WriteLine($"✅ Contagem {contagemParaEnviar.Codigo} enviada. ID: {resultado}");

                    // Limpar seleção e recarregar
                    ClearSelection();
                    await LoadContagensAsync(); // Recarregar para mover para aba "Enviadas"
                }
                else
                {
                    throw new Exception("Falha no envio - servidor retornou ID inválido");
                }

                await Task.Delay(2000);
                StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ Erro ao enviar contagem: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"❌ Erro OnSendContagem: {ex}");

                await Task.Delay(3000);
                StatusMessage = string.Empty;
            }
            finally
            {
                IsSending = false;
            }
        }

        private async void OnCreateNewContagem()
        {
            try
            {
                await Shell.Current.GoToAsync("//ContagemPage");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Erro ao navegar: {ex.Message}";
            }
        }

        #endregion

        #region Métodos Públicos

        public async Task RefreshDataAsync()
        {
            await LoadContagensAsync();
        }

        /// <summary>
        /// Método para atualizar estados dos comandos
        /// </summary>
        public void UpdateCommandStates()
        {
            ((Command)EditContagemCommand).ChangeCanExecute();
            ((Command)DeleteContagemCommand).ChangeCanExecute();
            ((Command)ShareExcelCommand).ChangeCanExecute();
            ((Command)SharePdfCommand).ChangeCanExecute();
            ((Command)SendContagemCommand).ChangeCanExecute();
        }

        /// <summary>
        /// Método para obter contagem selecionada
        /// </summary>
        public ContagemModel GetSelectedContagem() => _selectedContagem;

        /// <summary>
        /// Método para verificar se há contagens carregadas
        /// </summary>
        public bool HasContagens() => _allContagens.Any();

        /// <summary>
        /// Método para obter estatísticas
        /// </summary>
        public (int emAndamento, int enviadas, int total) GetEstatisticas()
        {
            var emAndamento = _allContagens.Count(c => !c.Enviada && !c.IsExcluido);
            var enviadas = _allContagens.Count(c => c.Enviada && !c.IsExcluido);
            var total = _allContagens.Count(c => !c.IsExcluido);

            return (emAndamento, enviadas, total);
        }

        #endregion

        #region Métodos de Debug/Teste

        /// <summary>
        /// Método para criar dados de teste (apenas para desenvolvimento)
        /// </summary>
        public async Task CreateTestDataAsync()
        {
            if (!_allContagens.Any())
            {
                var testContagens = new List<ContagemModel>
                {
                    new ContagemModel
                    {
                        Codigo = 1,
                        Id = 0,
                        Nome = "Teste Contagem 1",
                        DataHora = DateTime.Now.AddDays(-1),
                        CodigoLoja = 24,
                        Responsavel = "Usuario Teste",
                        Atividade = new Atividade { Nome = "Inventário Geral" },
                        Itens = new List<ContagemItem>
                        {
                            new ContagemItem
                            {
                                Produtos = new List<InventarioItem>
                                {
                                    new InventarioItem("7890123456789",2,"Produto Teste A", 10,"UN", 15.50m)
                                }
                            }
                        }
                    },
                    new ContagemModel
                    {
                        Codigo = 2,
                        Id = 0,
                        Nome = "Teste Contagem 2",
                        DataHora = DateTime.Now.AddHours(-6),
                        CodigoLoja = 24,
                        Responsavel = "Usuario Teste",
                        Atividade = new Atividade { Nome = "Contagem Sazonal" },
                        Itens = new List<ContagemItem>()
                    }
                };

                foreach (var contagem in testContagens)
                {
                    await _databaseService.InsertAsync(contagem);
                }

                System.Diagnostics.Debug.WriteLine("🧪 Dados de teste criados!");
                await LoadContagensAsync();
            }
        }

        #endregion
    }
}