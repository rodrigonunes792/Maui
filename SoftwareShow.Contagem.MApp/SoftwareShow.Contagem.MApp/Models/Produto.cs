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
    public class Produto : IProduto
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Unidade { get; set; }
        public decimal QuantidadeEmbalagem { get; set; }
        public decimal PesoUnidade { get; set; }
        public int TipoEntrada { get; set; }
        public string Busca { get; set; }
        public decimal LoteComponente { get; set; }
        [Ignore]
        public List<Componente> Componentes { get; set; }
        [Column("Componentes")]
        [JsonIgnore]
        public string ItensJson
        {
            get { return JsonConvert.SerializeObject(Componentes); }
            set { Componentes = JsonConvert.DeserializeObject<List<Componente>>(value); }
        }
        [JsonIgnore]
        [Ignore]
        int IProduto.Codigo { get => Codigo; }
        [JsonIgnore]
        [Ignore]
        string IProduto.Nome { get => Nome; }
        [JsonIgnore]
        [Ignore]
        string IProduto.UnidadeMedida { get => Unidade; }
        [JsonIgnore]
        [Ignore]
        List<Componente> IProduto.Componentes { get => Componentes; }
        [JsonIgnore]
        [Ignore]
        TiposEntrada IProduto.TipoEntrada
        {
            get
            {
                TiposEntrada tipo = TiposEntrada.Indefinido;
                try
                {
                    tipo = (TiposEntrada)Enum.ToObject(typeof(TiposEntrada), TipoEntrada);
                }
                catch { }

                return tipo;
            }
        }
        [JsonIgnore]
        [Ignore]
        TiposUnidade IProduto.TipoUnidade
        {
            get
            {
                bool ok = Enum.TryParse(Unidade, out TiposUnidade tipo);
                return ok ? tipo : TiposUnidade.Indefinido;
            }
        }

    }
}
