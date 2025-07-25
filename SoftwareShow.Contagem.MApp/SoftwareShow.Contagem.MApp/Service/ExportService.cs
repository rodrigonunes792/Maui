using SoftwareShow.Contagem.MApp.Interfaces;
using SoftwareShow.Contagem.MApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


namespace SoftwareShow.Contagem.MApp.Service
{
    public class ExportService : IExportService
    {
        public async Task<string> GerarExcelAsync(List<ContagemModel> contagens)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (contagens == null || !contagens.Any())
                        throw new ArgumentException("Nenhuma contagem fornecida para exportação");

                    // Nome do arquivo com timestamp
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var fileName = $"Contagens_Export_{timestamp}.csv";
                    var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                    // Cabeçalho do CSV
                    string cabecalho = "Cod.Loja;Contagem;Atividade;Data;Cod.Produto;Produto;Qtde;UN;Id;Custo Un;Total";
                    List<string> lines = new List<string> { cabecalho };

                    // Processar cada contagem
                    foreach (var contagem in contagens)
                    {
                        if (contagem.Itens?.Any() == true)
                        {
                            foreach (ContagemItem c in contagem.Itens)
                            {
                                foreach (InventarioItem i in c.Produtos)
                                {
                                    string line = String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10}",
                                        contagem.CodigoLoja,
                                        contagem.Codigo,
                                        contagem.Atividade?.Nome ?? "N/A",
                                        contagem.DataHora.ToString("dd/MM/yyyy"),
                                        i.CodigoProduto == null ? i.CodigoBarras : i.CodigoProduto.ToString(),
                                        i.Descricao,
                                        i.Quantidade,
                                        i.Unidade,
                                        i.CodigoProduto == null ? "N" : "S",
                                        i.CustoUnidade.ToString("F2"),
                                        i.CustoTotal.ToString("F2"));
                                    lines.Add(line);
                                }
                            }
                        }
                        else
                        {
                            // Linha para contagens sem itens
                            string line = String.Format("{0};{1};{2};{3};SEM ITENS;;;;;;;;",
                                contagem.CodigoLoja,
                                contagem.Codigo,
                                contagem.Atividade?.Nome ?? "N/A",
                                contagem.DataHora.ToString("dd/MM/yyyy"));
                            lines.Add(line);
                        }
                    }

                    // Escrever arquivo
                    File.WriteAllLines(filePath, lines);
                    return filePath;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro GerarExcelAsync: {ex.Message}");
                    throw;
                }
            });
        }

        public async Task<string> GerarPdfAsync(List<ContagemModel> contagens)
        {
            return "";
        }

        public async Task CompartilharArquivoAsync(string filePath, string title)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("Arquivo não encontrado para compartilhamento");

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = title,
                    File = new ShareFile(filePath)
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro CompartilharArquivoAsync: {ex.Message}");
                throw;
            }
        }
    }
}
