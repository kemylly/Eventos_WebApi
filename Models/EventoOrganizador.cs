namespace treino_api.Models
{
    public class EventoOrganizador
    {
        //essa classe faz relação com evento e organizador
        //um evento pode ter diversos organizadores 
        //e um organizador pode ter diversos eventos

        //public int Id {get; set;}
        public int EventoID {get; set;}
        public virtual Evento Evento {get; set;}
        public int OrganizadorID {get; set;}
        public virtual Organizador Organizador {get; set;}
    }
}