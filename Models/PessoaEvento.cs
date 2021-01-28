namespace treino_api.Models
{
    public class PessoaEvento
    {
        public int PessoaID {get;set;}
        public virtual Pessoa Pessoa {get; set;}
        public int EventoID {get; set;}
        public virtual Evento Evento {get; set;}
    }
}