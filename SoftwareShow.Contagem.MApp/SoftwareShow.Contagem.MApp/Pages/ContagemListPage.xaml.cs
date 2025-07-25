using SoftwareShow.Contagem.MApp.ViewModels;
using SoftwareShow.Contagem.MApp.Models;
using SoftwareShow.Contagem.MApp.Interfaces;
using System.ComponentModel;
using SoftwareShow.Contagem.MApp.Controls;
using CommunityToolkit.Maui.Views;

namespace SoftwareShow.Contagem.MApp.Pages
{
    public partial class ContagemListPage : ContentPage
    {
        private ContagemListViewModel _viewModel;
        private bool _isLongPressActive = false;
        private CancellationTokenSource _longPressCts;
        private readonly IOrientationService _orientationService;

        public ContagemListPage(ContagemListViewModel viewModel, IOrientationService orientationService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            // Conectar evento de compartilhamento
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            _orientationService = orientationService;
        }

        // Construtor sem par�metros para fallback
        public ContagemListPage()
        {
            InitializeComponent();
            _viewModel = new ContagemListViewModel();
            BindingContext = _viewModel;

            // Conectar evento de compartilhamento
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Escutar quando o comando Share for executado
            if (e.PropertyName == nameof(_viewModel.IsSelectionMode) && _viewModel.IsSelectionMode)
            {
                // Implementar l�gica adicional se necess�rio
            }
        }

        #region Eventos de Busca

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var newValue = e.NewTextValue ?? string.Empty;
            _viewModel.SearchText = newValue;
        }

        #endregion

        #region Eventos de Navega��o

        private async void OnBackClicked(object sender, EventArgs e)
        {
            // Se estiver em modo de sele��o, apenas limpar sele��o
            if (_viewModel.IsSelectionMode)
            {
                _viewModel.ClearSelection();
                return;
            }

            // Se estiver enviando, perguntar se quer cancelar
            if (_viewModel.IsSending)
            {
                var cancelResult = await DisplayAlert("Aten��o",
                    "Um envio est� em andamento. Deseja cancelar e sair?",
                    "Sim", "N�o");

                if (cancelResult)
                {
                    await Shell.Current.GoToAsync("//MenuPage");
                }
                return;
            }

            await Shell.Current.GoToAsync("//MenuPage");
        }

        #endregion

        #region Eventos de Toque nos Itens
        private async void OnContagemItemTapped(object sender, EventArgs e)
        {
            if (sender is Border border && border.BindingContext is ContagemModel contagem)
            {
                // Se estiver em modo de sele��o, adicionar/remover da sele��o
                if (_viewModel.IsSelectionMode)
                {
                    _viewModel.SelectContagemCommand.Execute(contagem);
                    return;
                }

                // Cancelar long press anterior
                _longPressCts?.Cancel();

                // Iniciar detec��o de long press
                _ = HandleLongPress(contagem);
            }
        }

        private async void OnContagemEnviadaItemTapped(object sender, EventArgs e)
        {
            if (sender is Border border && border.BindingContext is ContagemModel contagem)
            {
                if (_viewModel.IsSelectionMode)
                {
                    _viewModel.SelectContagemCommand.Execute(contagem);
                    return;
                }

                _longPressCts?.Cancel();
                _ = HandleLongPress(contagem);
            }
        }

        private async Task HandleLongPress(ContagemModel contagem)
        {
            // Cancelar long press anterior se existir
            _longPressCts?.Cancel();
            _longPressCts = new CancellationTokenSource();

            try
            {
                // Aguardar 800ms para detectar toque longo
                await Task.Delay(800, _longPressCts.Token);

                // Se chegou at� aqui, � long press
                _viewModel.SelectContagemCommand.Execute(contagem);

                // Feedback haptic se dispon�vel
                try
                {
                    HapticFeedback.Perform(HapticFeedbackType.LongPress);
                }
                catch { }
            }
            catch (OperationCanceledException)
            {
                // Long press foi cancelado - comportamento normal
            }
        }

        #endregion

        #region Eventos do Header

        private async void OnShareButtonClicked(object sender, EventArgs e)
        {
            //if (!_viewModel.CanShare) return;

            try
            {
                var popup = new ShareOptionsPopup();
                var result = await this.ShowPopupAsync(popup);

                if (result is string shareOption)
                {
                    await ProcessShareOption(shareOption);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao abrir op��es de compartilhamento: {ex.Message}", "OK");
            }
        }

        private async Task ProcessShareOption(string option)
        {
            try
            {
                switch (option)
                {
                    case "PDF":
                        _viewModel.SharePdfCommand.Execute(_viewModel.SelectedContagem);
                        break;

                    case "Excel":
                        _viewModel.ShareExcelCommand.Execute(_viewModel.SelectedContagem);
                        break;

                    case "Cancel":
                        // N�o fazer nada
                        break;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao compartilhar: {ex.Message}", "OK");
            }
        }

        #endregion


        #region Lifecycle Events

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var orientationService = _orientationService;
            orientationService?.ForcePortrait();
            // Limpar sele��o ao aparecer
            _viewModel?.ClearSelection();

            // Atualizar dados
            _viewModel?.RefreshCommand.Execute(null);

      
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _orientationService?.AllowAllOrientations();
            base.OnDisappearing();
            // Limpar sele��o
            _viewModel?.ClearSelection();

            // Cancelar long press pendente
            _longPressCts?.Cancel();

            // Permitir todas as orienta��es
            try
            {
                DependencyService.Get<IOrientationService>()?.AllowAllOrientations();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao permitir orienta��es: {ex.Message}");
            }
        }

        #endregion

        #region Eventos Personalizados da ViewModel

        // M�todos para intera��o com a ViewModel se necess�rio

        #endregion

        #region M�todos Auxiliares

        private void UpdateSelectionVisualState()
        {
            // Atualizar estados visuais baseados na sele��o
            if (_viewModel.IsSelectionMode)
            {
                // Pode adicionar anima��es ou outros efeitos visuais aqui
                NormalHeader.FadeTo(0, 200);
                SelectionHeader.FadeTo(1, 200);
            }
            else
            {
                SelectionHeader.FadeTo(0, 200);
                NormalHeader.FadeTo(1, 200);
            }
        }

        #endregion

        #region Eventos de Toque (para detectar fim do long press)

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);

            if (child is CollectionView collectionView)
            {
                // Adicionar gesture recognizer para detectar fim do toque
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => _isLongPressActive = false;
                collectionView.GestureRecognizers.Add(tapGesture);
            }
        }



        #endregion

        #region M�todos P�blicos

        /// <summary>
        /// M�todo para atualizar dados externamente
        /// </summary>
        public async Task RefreshDataAsync()
        {
            await _viewModel.RefreshDataAsync();
        }

        /// <summary>
        /// M�todo para limpar sele��o externamente
        /// </summary>
        public void ClearSelection()
        {
            _viewModel.ClearSelection();
        }


        #endregion
    }
}