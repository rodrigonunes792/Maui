using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareShow.Contagem.MApp.Models
{
    public class Preco
    {
        public int CodigoProduto { get; set; }
        public decimal PVF { get; set; }
        public decimal Margem { get; set; }
        public decimal PVC { get; set; }
    }
}
