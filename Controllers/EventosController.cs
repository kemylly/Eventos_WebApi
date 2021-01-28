using System;
using System.Linq;
using treino_api.Data;
using treino_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace treino_api.Controllers
{
    [Route("api/v1/[Controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] //so acessa quem tiver autorizado
    public class EventosController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        //conexao com o banco
        public EventosController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult Get() //listar todos os eventos
        {
            var eventos = database.Eventos.Where(e => e.Status == true && e.Ativo == true).ToList();
            return Ok(eventos);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id) //listar um evento especifico
        {
            try{ //verificar se o meu evento existe
                Evento evento = database.Eventos.First(e => e.Id == id);
                
                if(evento.Status == true) //verificar o meu evento esta ativo
                {
                    if(evento.Ativo == true) //verificar se esse evento foi cancelado
                    {
                        return Ok(evento);
                    }else{ //esse evento foi cancelado apenas
                        return new ObjectResult(new{info = "Esse evento foi cancelado"});
                    }
                    
                }else{ //esse evento foi deletado
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Esse evento foi deletado"});
                }
            }catch{ //esse evento não existe
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Não encontrado"});
            }
        }

        [HttpGet("Cancelados")]
        public IActionResult Cancelado() //listar os eventos cancelados
        {
            var eventos = database.Eventos.Where(e => e.Status == true && e.Ativo == false).ToList();
            return Ok(eventos);
        }

        [HttpPost]
        public IActionResult Post([FromBody] EventoTemp eTemp)
        {
            //validacao
            if(eTemp.Nome.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Nome Invalido"});
            }

            if(eTemp.Endereco.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Endereço Invalido"});
            }

            if(eTemp.Plataforma.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Plataforma Invalida"});
            }

            if(eTemp.Tipo.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Tipo Invalido"});
            }

            if(eTemp.Publico.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Publico Invalido"});
            }

            if(eTemp.Quantidade <= 1) 
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Quantidade Invalida"});
            }

            if(eTemp.Preco < 0) //se for gratuito o preco é 0
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Preço Invalido"});
            }

            if(eTemp.Sobre.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Sobre Invalido"});
            }
            try{
                //salvar o evento
                Evento ev = new Evento();
                ev.Nome = eTemp.Nome;
                ev.Endereco = eTemp.Endereco;
                ev.Plataforma = eTemp.Plataforma;
                ev.DataInicio = eTemp.DataInicio;
                ev.DataTermino = eTemp.DataTermino;
                ev.Tipo = eTemp.Tipo;
                ev.Publico = eTemp.Publico;
                ev.Quantidade = eTemp.Quantidade;
                ev.Preco = eTemp.Preco;
                ev.Sobre = eTemp.Sobre;
                ev.Ativo = true;
                ev.Status = true;

                //List<Organizador> evt = new List<Organizador>();

                database.Eventos.Add(ev);
                database.SaveChanges();
                

                foreach (var org in eTemp.OrganizadoresId)
                {
                    EventoOrganizador eo = new EventoOrganizador();

                    //var organizador = database.Organizadores.First(o => o.Id == org);

                    eo.Evento = database.Eventos.First(e => e.Id == ev.Id);
                    eo.Organizador = database.Organizadores.First(e => e.Id == org);

                    database.EventosOrganizadores.Add(eo);
                    database.SaveChanges();
                }

                Response.StatusCode = 201;
                return new ObjectResult("Cadastrado com sucesso");
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult("Erro ao cadastrar");
            }
           
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try{
                Evento eve = database.Eventos.First(e => e.Id == id);

                eve.Status = false;
                database.SaveChanges();

                Response.StatusCode = 201;
                return new ObjectResult("Evento deletado");
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao tentar deletar"});
            }
        }

        [HttpPatch]
        public IActionResult Patch([FromBody] Evento evento)
        {
            if(evento.Id > 0)
            {
                try{
                    var eve = database.Eventos.First(e => e.Id == evento.Id);

                    if(eve != null)
                    {
                        eve.Nome = evento.Nome != null ? evento.Nome : eve.Nome;
                        eve.Endereco = evento.Endereco != null ? evento.Endereco : eve.Endereco;
                        eve.Plataforma = evento.Plataforma != null ? evento.Plataforma : eve.Plataforma;
                        eve.Tipo = evento.Tipo != null ? evento.Tipo : eve.Tipo;
                        eve.Publico = evento.Publico != null ? evento.Publico : eve.Publico;
                        eve.Quantidade = evento.Quantidade > 1 ? evento.Quantidade : eve.Quantidade;
                        eve.Preco = evento.Preco >= 0  ? evento.Preco : eve.Preco;
                        eve.Sobre = evento.Sobre != null ? evento.Sobre : eve.Sobre;
                        eve.Ativo = false != true ? evento.Ativo : eve.Ativo;

                        database.SaveChanges();
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Edição feita com sucesso"});
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Evento não encontrado"});
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Evento não encontrado"});
                }
            }
            else
            {
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O Id de evento é inválido!"});
            }
        }

        public class EventoTemp
        {
            public int Id {get; set;}
            public string Nome {get; set;} 
            public string Endereco {get;set;} 
            public string Plataforma {get;set;} 
            public DateTime DataInicio {get; set;} 
            public DateTime DataTermino {get; set;} 
            public string Tipo {get; set;}  
            public string Publico {get; set;} 
            public int Quantidade {get; set;} 
            public float Preco {get; set;} 
            public string Sobre {get; set;} 
            public bool Ativo {get; set;} 
            public bool Status {get; set;} 
            //public List<Organizador> Organizadors {get; set;}
            //public List<EventoOrganizador> Organizadores {get; set;}
            public List<int> OrganizadoresId {get; set;}
        }
    }
}