using SoftwareShow.Contagem.MApp.Models;

namespace SoftwareShow.Contagem.MApp.Interfaces
{
    public interface IRestService
    {
        Task<Autenticacao> AutenticarAsync(string email, string senha, string versaoApp);
        //Task<List<Kit>> BaixarKit(int codigoLoja);
        //Task<List<Preco>> BaixarPreco(int codigoLoja);
        //Task<List<Atividade>> ConsultarAtividade();
        //Task<List<CodigoBarras>> BaixarCodigoBarras();
        //Task<List<Produto>> BaixarProduto();
        //Task<List<CodigoBarras>> ConsultarCodigoBarras(string codigo);
        //Task<List<Loja>> ConsultarLoja(int codigo);
        //Task<bool> FazerLogoff(string autorizacao);


        // Métodos com suporte a CancellationToken para sincronização
        Task<List<Kit>> BaixarKitAsync(int codigoLoja, CancellationToken cancellationToken = default);
        Task<List<Preco>> BaixarPrecoAsync(int codigoLoja, CancellationToken cancellationToken = default);
        Task<List<Atividade>> ConsultarAtividadeAsync(CancellationToken cancellationToken = default);
        Task<List<CodigoBarras>> BaixarCodigoBarrasAsync(CancellationToken cancellationToken = default);
        Task<List<Produto>> BaixarProdutoAsync(CancellationToken cancellationToken = default);
        Task<List<ProdutosInventario>> ListarProdutosInventarioPorAtividadeIdAsync(int atividadeId);
        Task<int> EnviarContagemAsync(ContagemModel obj);
    }
}
