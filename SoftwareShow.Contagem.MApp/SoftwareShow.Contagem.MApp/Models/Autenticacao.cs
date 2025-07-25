using Newtonsoft.Json;

namespace SoftwareShow.Contagem.MApp.Models
{
    public class Autenticacao
    {
        [JsonProperty("usuario")]
        public string Usuario { get; set; } = string.Empty;

        [JsonProperty("autorizacao")]
        public string Autorizacao { get; set; } = string.Empty;

        [JsonProperty("dataHora")]
        public DateTime DataHora { get; set; }

        [JsonProperty("autenticado")]
        public bool Autenticado { get; set; }

        [JsonProperty("mensagem")]
        public string Mensagem { get; set; } = string.Empty;

        [JsonProperty("limpar")]
        public bool Limpar { get; set; }

        [JsonProperty("lojas")]
        public IEnumerable<LojaUsuario> Lojas { get; set; } = new List<LojaUsuario>();
    }

    public class LojaUsuario
    {
        [JsonProperty("COD_LOJA")]
        public int COD_LOJA { get; set; }

        [JsonProperty("NOME_LOJA")]
        public string NOME_LOJA { get; set; } = string.Empty;
    }

}
