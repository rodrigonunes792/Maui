using SoftwareShow.Contagem.MApp.Enums;
using SoftwareShow.Contagem.MApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareShow.Contagem.MApp.Interfaces
{
    public interface IProduto
    {
        int Codigo { get; }
        string Nome { get; }
        string UnidadeMedida { get; }
        List<Componente> Componentes { get; }
        TiposEntrada TipoEntrada { get; }
        TiposUnidade TipoUnidade { get; }
    }
}
