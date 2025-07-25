using Newtonsoft.Json;
using SoftwareShow.Contagem.MApp.Enums;
using SoftwareShow.Contagem.MApp.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareShow.Contagem.MApp.Models
{
    public class ContagemItem
    {
        public ContagemItem(string codigo, UnidadeMedida unidade, decimal qtde, IProduto produto = null, string descricao = null, bool identificado = true, List<Preco> precos = null)
        {
            CodigoBarras = codigo;
            Produto = produto;
            Descricao = Produto == null ? descricao ?? String.Empty : Produto.Nome;
            Quantidade = qtde;
            UnidadeMedida = unidade;
            Unidade = UnidadeMedida.Codigo;
            Identificado = identificado;
            Precos = precos;
            Produtos = SelecionarProdutos();
            DataHora = DateTime.Now;
        }
        public ContagemItem()
        {
            CodigoBarras = String.Empty;
            Produto = null;
            Descricao = String.Empty;
            Quantidade = 0;
            UnidadeMedida = UnidadeMedida.Todos.Find(u => u.Padrao);
            Unidade = UnidadeMedida.Codigo;
            Identificado = false;
            Precos = new List<Preco>();
            Produtos = new List<InventarioItem>();
            DataHora = DateTime.Now;
        }
        [Ignore]
        [JsonIgnore]
        public IProduto Produto { get; set; }
        [Ignore]
        [JsonIgnore]
        private UnidadeMedida UnidadeMedida { get; set; }
        public string CodigoBarras { get; set; }
        public bool Identificado { get; set; }
        public string Descricao { get; set; }
        public decimal Quantidade { get; set; }
        public DateTime DataHora { get; set; }
        public string Unidade { get; set; }
        [Ignore]
        public List<InventarioItem> Produtos { get; set; }
        [Column("Produtos")]
        [JsonIgnore]
        public string ProdutosJson
        {
            get { return JsonConvert.SerializeObject(Produtos); }
            set { Produtos = JsonConvert.DeserializeObject<List<InventarioItem>>(value); }
        }
        private List<InventarioItem> SelecionarProdutos()
        {
            List<InventarioItem> prods = new List<InventarioItem>();

            if (Produto != null)
            {
                //qtde comprada * quantidade por embalagem * qtde itens arvores / lote

                if (Produto is Kit)
                {
                    foreach (Componente comp in Produto.Componentes)
                    {
                        prods.Add(
                           new InventarioItem(
                               CodigoBarras,
                               Quantidade * comp.Quantidade,
                               comp.Unidade,
                               comp.Codigo,
                               comp.Nome,
                               LerPreco(comp.Codigo))
                               );
                    }
                }
                else
                {
                    //Calcular quantidade:(unidade entrada) -> quantidade:(unidade do produto)
                    decimal qtdeCalculada = Quantidade;
                    Produto prod = (Produto)Produto;
                    TiposUnidade unidadeInformada = UnidadeMedida.Id;
                    TiposUnidade unidadeProduto = Produto.TipoUnidade;
                    decimal qtdeInformada = Quantidade;
                    decimal qtdeEmbalagem = prod.QuantidadeEmbalagem;
                    decimal peso = prod.PesoUnidade;
                    decimal lote = prod.LoteComponente;

                    if (Produto.TipoUnidade != UnidadeMedida.Id)
                    {
                        //CX -> UN
                        if (unidadeInformada == TiposUnidade.CX && unidadeProduto == TiposUnidade.UN)
                            qtdeCalculada = Math.Round((qtdeInformada * qtdeEmbalagem), 0);

                        //CX -> KG
                        if (unidadeInformada == TiposUnidade.CX && unidadeProduto == TiposUnidade.KG)
                            qtdeCalculada = Math.Round(((qtdeInformada * qtdeEmbalagem) * peso), 3);

                        //KG -> UN
                        if (unidadeInformada == TiposUnidade.KG && unidadeProduto == TiposUnidade.UN)
                            qtdeCalculada = Math.Round((qtdeInformada / peso), 0);

                        //KG -> CX
                        if (unidadeInformada == TiposUnidade.KG && unidadeProduto == TiposUnidade.CX)
                            qtdeCalculada = Math.Round(((qtdeInformada / peso) / qtdeEmbalagem), 0);

                        //UN -> CX
                        if (unidadeInformada == TiposUnidade.UN && unidadeProduto == TiposUnidade.CX)
                            qtdeCalculada = Math.Round((qtdeInformada / qtdeEmbalagem), 0);

                        //UN -> KG
                        if (unidadeInformada == TiposUnidade.UN && unidadeProduto == TiposUnidade.KG)
                            qtdeCalculada = Math.Round((qtdeInformada * peso), 3);
                    }

                    //É componente?
                    if (Produto.TipoEntrada == TiposEntrada.Componente)
                    {
                        foreach (Componente comp in Produto.Componentes)
                        {
                            prods.Add(
                                new InventarioItem(
                                    CodigoBarras,
                                    (qtdeCalculada * comp.Quantidade) / lote,
                                    comp.Unidade,
                                    comp.Codigo,
                                    comp.Nome,
                                    LerPreco(comp.Codigo))
                                    );
                        }
                    }
                    else
                    {
                        prods.Add(new InventarioItem(CodigoBarras, qtdeCalculada, prod.Unidade, prod.Codigo, prod.Nome, LerPreco(prod.Codigo)));
                    }
                }
            }
            else
            {
                prods.Add(new InventarioItem(CodigoBarras, Quantidade, Unidade, null, Descricao));
            }

            return prods;
        }
        [Ignore]
        [JsonIgnore]
        private List<Preco> Precos { get; set; }
        private decimal LerPreco(int codigoProduto) => Precos?.Find(p => p.CodigoProduto == codigoProduto)?.PVF ?? 0;
        [Ignore]
        [JsonIgnore]
        public decimal CustoTotalItem
        {
            get
            {
                decimal custoTotal = 0;

                Produtos.ForEach((p) =>
                {
                    custoTotal += p.CustoTotal;
                });

                return custoTotal;
            }
        }
    }
}
