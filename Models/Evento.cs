using System;
using System.Collections.Generic;

namespace treino_api.Models
{
    public class Evento
    {
        public int Id {get; set;}
        public string Nome {get; set;} //nome do evento
        public string Endereco {get;set;} //endereco online = link da reuniao ou link do youtube etc
        public string Plataforma {get;set;} //qual a plataforma que eu vou divulgar o meu evento / youtube, teams, sei la
        public DateTime DataInicio {get; set;} //data e hora dde inicio do evento
        public DateTime DataTermino {get; set;} //data e hora do termino do aevento
        public string Tipo {get; set;}  //hackathon / curso / workshop / palestra etc
        public string Publico {get; set;} // a quem é destinado, adulato programadores
        public int Quantidade {get; set;} //quantidade maxima de pessoas que podem participar do evento
        public float Preco {get; set;} //qual é o preco do ingresso para o evento - é gratuito?
        public string Sobre {get; set;} //descricao do evento
        public virtual List<EventoOrganizador> Organizadores {get; set;} //relacionamento de muitos para muitos com organizador
        public bool Ativo {get; set;} //se o evento foi cancelado ou não, se ele tiver sido cancelado ele não é excluido do sistema
        public bool Status {get; set;} //se foi deletado ou não do sistema
        public virtual List<PessoaEvento> Pessoas {get;set;}
    }
}