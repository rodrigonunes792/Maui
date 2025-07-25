using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareShow.Contagem.MApp.Interfaces
{
    public interface ISpeechToTextService
    {
        Task<string> GetSpeechToTextAsync();
    }
}
