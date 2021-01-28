using System.Collections.Generic;

namespace treino_api.Models
{
    public class Pessoa
    {
        public int Id {get; set;}
        public string Nome {get; set;}
        public string Telefone {get;set;} //telefone de conato
        public string Cpf {get; set;}
        public bool Status {get; set;} //deleta uma pessoa
        public virtual List<PessoaEvento> Eventos {get;set;}

    }
}