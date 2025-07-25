using SoftwareShow.Contagem.MApp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareShow.Contagem.MApp.Models
{
    public class UnidadeMedida
    {
        public UnidadeMedida()
        {
        }
        public UnidadeMedida(bool padrao)
        {
            Padrao = padrao;
        }
        public TiposUnidade Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public bool Padrao { get; set; }
        public static List<UnidadeMedida> Todos => new List<UnidadeMedida>(new[] {
            new UnidadeMedida { Codigo = "UN", Nome = "Unidade", Padrao = true, Id = TiposUnidade.UN },
            new UnidadeMedida { Codigo = "KG", Nome = "Kilo", Id = TiposUnidade.KG },
            new UnidadeMedida { Codigo = "CX", Nome = "Caixa", Id = TiposUnidade.CX },
            new UnidadeMedida { Codigo = "KIT", Nome= "Kit", Id = TiposUnidade.KIT } });
    }
}
