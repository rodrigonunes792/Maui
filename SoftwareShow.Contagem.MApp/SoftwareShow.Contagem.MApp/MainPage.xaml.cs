using SoftwareShow.Contagem.MApp.Models;
using SoftwareShow.Contagem.MApp.Interfaces;
using SoftwareShow.Contagem.MApp.Service;
using System.Text;

namespace SoftwareShow.Contagem.MApp;

public partial class MainPage : ContentPage
{
    private readonly IDatabaseService _databaseService;

    public MainPage(IDatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        LoadAtividades();
    }

    // Construtor sem parâmetros para fallback
    public MainPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
        LoadAtividades();
    }

    private async void LoadAtividades()
    {
        try
        {
            var atividades = await _databaseService.GetAllAsync<Atividade>();

            var sb = new StringBuilder();
            sb.AppendLine($"Total de atividades encontradas: {atividades.Count}\n");

            if (atividades.Count == 0)
            {
                sb.AppendLine("Nenhuma atividade encontrada no banco local.");
                sb.AppendLine("Faça uma sincronização primeiro.");
            }
            else
            {
                for (int i = 0; i < atividades.Count; i++)
                {
                    var atividade = atividades[i];
                    sb.AppendLine($"{i + 1}. ID: {atividade.Id}");
                    sb.AppendLine($"   Nome: {atividade.Nome}");
                    sb.AppendLine($"   Grupo: {atividade.Grupo}");
                    sb.AppendLine($"   Início: {atividade.DataHoraInicial:dd/MM/yyyy HH:mm}");
                    sb.AppendLine($"   Fim: {atividade.DataHoraFinal:dd/MM/yyyy HH:mm}");
                    sb.AppendLine($"   Loja ID: {atividade.LojaId}");
                    sb.AppendLine($"   Tipo Inventário: {atividade.TipoInventario}");
                    sb.AppendLine();
                }
            }

            AtividadesLabel.Text = sb.ToString();

            System.Diagnostics.Debug.WriteLine($"MainPage: Carregadas {atividades.Count} atividades");
        }
        catch (Exception ex)
        {
            AtividadesLabel.Text = $"Erro ao carregar atividades: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Erro ao carregar atividades: {ex.Message}");
        }
    }
}