using Newtonsoft.Json;
using SQLite;
using System.ComponentModel;

namespace SoftwareShow.Contagem.MApp.Models
{
    [Table("Contagens")]
    public class ContagemModel : INotifyPropertyChanged
    {
        private string _inventario = "0";
        private string _excluido = "0";

        public ContagemModel()
        {
            Id = 0;
            Codigo = 0;
            CodigoLoja = 0;
            DataHora = DateTime.Now;
            Responsavel = string.Empty;
            Nome = string.Empty;
            Descricao = null;
            Inventario = "0";
            AtividadeId = 0;
            DataHoraEnvio = null;
            Excluido = "0";
            DataCorrigida = DateTime.Now.Date;
            VersaoContagem = null;
            SetorLoja = null;
            Itens = new List<ContagemItem>();
        }

        #region Propriedades do Banco

        [PrimaryKey, AutoIncrement]
        public int Codigo { get; set; }

        public int Id { get; set; }

        public DateTime DataHora { get; set; }

        public int CodigoLoja { get; set; }

        public string Nome { get; set; }

        public string Descricao { get; set; }

        public string Responsavel { get; set; }

        public string Inventario
        {
            get => _inventario;
            set
            {
                if (_inventario != value)
                {
                    _inventario = value;
                    OnPropertyChanged(nameof(Inventario));
                    OnPropertyChanged(nameof(IsInventario));
                }
            }
        }

        public int AtividadeId { get; set; }

        public DateTime? DataHoraEnvio { get; set; }

        public string Excluido
        {
            get => _excluido;
            set
            {
                if (_excluido != value)
                {
                    _excluido = value;
                    OnPropertyChanged(nameof(Excluido));
                    OnPropertyChanged(nameof(IsExcluido));
                }
            }
        }

        public DateTime DataCorrigida { get; set; }

        public string VersaoContagem { get; set; }

        public string SetorLoja { get; set; }

        #endregion

        #region Propriedades Relacionadas (não persistidas diretamente)

        [Ignore]
        public List<ContagemItem> Itens { get; set; }

        [Column("Itens")]
        [JsonIgnore]
        public string ItensJson
        {
            get => JsonConvert.SerializeObject(Itens);
            set => Itens = string.IsNullOrEmpty(value) ? new List<ContagemItem>() : JsonConvert.DeserializeObject<List<ContagemItem>>(value);
        }

        [Ignore]
        public Atividade Atividade { get; set; }

        [JsonIgnore]
        [Column("Atividade")]
        public string AtividadeJson
        {
            get => JsonConvert.SerializeObject(Atividade);
            set => Atividade = string.IsNullOrEmpty(value) ? null : JsonConvert.DeserializeObject<Atividade>(value);
        }

        #endregion

        #region Propriedades Computed (Regras de Domínio)

        /// <summary>
        /// Indica se a contagem foi enviada para o servidor
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public bool Enviada => DataHoraEnvio.HasValue;

        /// <summary>
        /// Indica se pode enviar a contagem
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public bool PodeEnviar => !Enviada && !IsExcluido;

        /// <summary>
        /// Converte string "0"/"1" para bool - Inventario
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public bool IsInventario => Inventario == "1";

        /// <summary>
        /// Converte string "0"/"1" para bool - Excluido
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public bool IsExcluido => Excluido == "1";

        /// <summary>
        /// Calcula o custo total dos itens da contagem
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public decimal CustoTotal
        {
            get
            {
                if (Itens == null || !Itens.Any())
                    return 0;

                return Itens.Sum(item => item.CustoTotalItem);
            }
        }

        /// <summary>
        /// Quantidade total de itens na contagem
        /// </summary>
        [Ignore]
        [JsonIgnore]
        public int QuantidadeItens => Itens?.Count ?? 0;

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Métodos de Domínio

        /// <summary>
        /// Marca a contagem como enviada
        /// </summary>
        public void MarcarComoEnviada(int idServidor)
        {
            Id = idServidor;
            DataHoraEnvio = DateTime.Now;
            OnPropertyChanged(nameof(Enviada));
            OnPropertyChanged(nameof(PodeEnviar));
        }

        /// <summary>
        /// Marca a contagem como excluída (soft delete)
        /// </summary>
        public void MarcarComoExcluida()
        {
            Excluido = "1";
        }

        /// <summary>
        /// Restaura uma contagem excluída
        /// </summary>
        public void RestaurarContagem()
        {
            Excluido = "0";
        }

        /// <summary>
        /// Define se é uma contagem de inventário
        /// </summary>
        public void DefinirComoInventario(bool isInventario)
        {
            Inventario = isInventario ? "1" : "0";
        }

        #endregion

        #region Equals e GetHashCode

        public override bool Equals(object obj)
        {
            if (obj is ContagemModel other)
            {
                return Codigo == other.Codigo;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Codigo.GetHashCode();
        }

        #endregion

        #region Propriedades para Compatibilidade (se necessário manter código existente)

        /// <summary>
        /// Propriedade para manter compatibilidade com código existente
        /// Remover quando não for mais necessária
        /// </summary>
        [Ignore]
        [JsonIgnore]
        [Obsolete("Use as propriedades específicas do banco. Esta propriedade será removida.")]
        public bool AllowMultiSelect { get; set; }

        #endregion
    }
}
