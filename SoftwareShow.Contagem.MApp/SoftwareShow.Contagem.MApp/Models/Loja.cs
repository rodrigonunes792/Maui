using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareShow.Contagem.MApp.Models
{
    public class Loja
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string LojaCompleta => $"{Codigo} - {Nome}";
    }
}
