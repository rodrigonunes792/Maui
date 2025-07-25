using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareShow.Contagem.MApp.Models
{
    public class InventarioItem
    {
        public InventarioItem(string codigoBarras, decimal quantidade, string unidade, int? codigoProduto = null, string nomeProduto = null, decimal custoUnidade = 0)
        {
            CodigoBarras = codigoBarras;
            Quantidade = quantidade;
            CodigoProduto = codigoProduto;
            Unidade = unidade;
            Descricao = nomeProduto;
            CustoUnidade = custoUnidade;
        }
        public string CodigoBarras { get; set; }
        public int? CodigoProduto { get; set; }
        public decimal Quantidade { get; set; }
        public string Unidade { get; set; }
        public string Descricao { get; set; }
        public decimal CustoUnidade { get; set; }
        public decimal CustoTotal { get { return Quantidade * CustoUnidade; } }
    }
}
