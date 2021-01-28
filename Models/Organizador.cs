using System.Collections.Generic;

namespace treino_api.Models
{
    public class Organizador
    {
        //os organizadores vao ser pessoas fisica a principio
        public int Id {get; set;}
        public string Nome {get; set;}  //nome da pessoa
        public string Telefone {get;set;} //telefone de conato
        public string Email {get;set;}
        public string Cpf {get; set;} 
        public string Rede {get; set;} //guarda o nome de usuario de alguma rede social de preferencia do organizador
        public virtual List<EventoOrganizador> Eventos {get; set;} //relacionamento de muitos para muitos com
        public bool Status {get; set;} //deletar um organizador
    }
}