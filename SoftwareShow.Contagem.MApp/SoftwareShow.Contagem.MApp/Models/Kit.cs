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
    public class Kit : IProduto
    {
        public int Codigo { get; set; }
        public int CodigoLoja { get; set; }
        public string Unidade { get; set; }
        public string Nome { get; set; }
        public string Busca { get; set; }
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
        TiposEntrada IProduto.TipoEntrada { get => TiposEntrada.Componente; }
        [JsonIgnore]
        [Ignore]
        TiposUnidade IProduto.TipoUnidade { get => TiposUnidade.KIT; }
    }
}
