using SoftwareShow.Contagem.MApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareShow.Contagem.MApp.Interfaces
{
    public interface IExportService
    {
        Task<string> GerarExcelAsync(List<ContagemModel> contagens);
        Task<string> GerarPdfAsync(List<ContagemModel> contagens);
        Task CompartilharArquivoAsync(string filePath, string title);
    }
}
