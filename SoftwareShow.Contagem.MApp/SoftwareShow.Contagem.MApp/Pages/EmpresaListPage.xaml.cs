using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using SoftwareShow.Contagem.MApp.Models;
using SoftwareShow.Contagem.MApp.ViewModels;

namespace SoftwareShow.Contagem.MApp.Pages;

public partial class EmpresaListPage : ContentPage
{
    private EmpresaListViewModel Model { get; set; }
    public EmpresaListPage()
        : this(new EmpresaListViewModel())
    {
    }

    public EmpresaListPage(EmpresaListViewModel viewModel)
    {
        InitializeComponent();
        Model = viewModel;
        BindingContext = Model;
    }

    private void EmpresaListPage_Loaded(object sender, EventArgs e)
    {
        EntryFiltro.Focus();
    }

    private void EntryFiltro_TextChanged(object sender, TextChangedEventArgs e)
    {
        var viewModel = (EmpresaListViewModel)BindingContext;
        CollectionViewEmpresas.ItemsSource = 
            viewModel.Empresas.Where(x => x.Busca.Contains(e.NewTextValue, StringComparison.InvariantCultureIgnoreCase)).ToList<Empresa>();
    }
}