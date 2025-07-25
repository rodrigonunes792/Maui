using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareShow.Contagem.MApp.Models
{
    public class Atividade
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Grupo { get; set; }
        public DateTime DataHoraInicial { get; set; }
        public DateTime DataHoraFinal { get; set; }
        public int LojaId { get; set; }
        public int TipoInventario { get; set; }
    }
}
