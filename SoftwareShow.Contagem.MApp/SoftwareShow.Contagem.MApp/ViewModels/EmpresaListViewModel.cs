using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SoftwareShow.Contagem.MApp.Models;

namespace SoftwareShow.Contagem.MApp.ViewModels
{
    public class EmpresaListViewModel : ViewModelBase
    {
        private Empresa? _empresaSelecionada;
        private bool _podeAvancar;
        public EmpresaListViewModel() { }

        public ObservableCollection<Empresa> Empresas { get; set; } =
            [
                new Empresa
                {
                    Identificador = "9900",
                    Nome = "SP SANTO ANDRE SHOPPINHO"
                },
                new Empresa
                {
                    Identificador = "9000",
                    Nome = "SP ITAPEVI HIPER"
                },
                        new Empresa
            {
                Identificador = "9000",
                Nome = "SP ITAPEVI HIPER"
            },
                        new Empresa
            {
                Identificador = "9000",
                Nome = "SP ITAPEVI HIPER"
            },
                                    new Empresa
            {
                Identificador = "9000",
                Nome = "SP ITAPEVI HIPER"
            },
                                    new Empresa
            {
                Identificador = "9000",
                Nome = "SP ITAPEVI HIPER"
            },
                                    new Empresa
            {
                Identificador = "9000",
                Nome = "SP ITAPEVI HIPER"
            },
                                    new Empresa
            {
                Identificador = "9000",
                Nome = "SP ITAPEVI HIPER"
            },
                                    new Empresa
            {
                Identificador = "9000",
                Nome = "SP ITAPEVI HIPER"
            },
                                    new Empresa
            {
                Identificador = "9000",
                Nome = "SP ITAPEVI HIPER"
            },
                                    new Empresa
            {
                Identificador = "9000",
                Nome = "SP ITAPEVI HIPER"
            },
                                    new Empresa
            {
                Identificador = "9000",
                Nome = "SP ITAPEVI HIPER"
            }

            ];

        public bool PodeAvancar 
        {
            get { return _podeAvancar; }
            set { _podeAvancar = value;  }
        }

        public Empresa? EmpresaSelecionada         
        {
            get => _empresaSelecionada;
            set 
            {
                SetProperty(ref _empresaSelecionada, value);
                SetProperty(ref _podeAvancar, true);
            }
        }


        //public string Name
        //{
        //    get => _name;
        //    set => SetProperty(ref _name, value);
        //}
        //private string _name;
    }
}
