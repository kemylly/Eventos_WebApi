using System;
using System.Linq;
using treino_api.Data;
using treino_api.Models;
using treino_api.HATEOAS;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace treino_api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] //so acessa quem tiver autorizado
    public class PessoaController : ControllerBase
    {
        private readonly ApplicationDbContext database;
        private HATEOAS.Hateoas hateoas;

        public PessoaController(ApplicationDbContext database)
        {
            this.database = database;
            hateoas = new HATEOAS.Hateoas("localhost:5001/api/v1/Pessoa");
            hateoas.AddAction("GET_INFO","GET");
            hateoas.AddAction("DELETE_PESSOA","DELETE");
            hateoas.AddAction("EDIT_PESSOA","PATCH");
        }

        [HttpGet]
        public IActionResult Get()  //listar todas as pessoas
        {
            var pessoas = database.Pessoas.Where(p => p.Status == true).ToList();
            //var pessoas = database.Pessoas.Include(e => e.EventoId).Where(p => p.Status == true).ToList();
            
            // List<PessoaContainer> pessoasHATEOAS = new List<PessoaContainer>();
            
            // foreach (var pec in pessoas)
            // {
            //     PessoaContainer pessoaHATEOAS = new PessoaContainer();
            //     pessoaHATEOAS.pessoa = pec;
            //     pessoaHATEOAS.links = hateoas.GetActions(pec.Id.ToString());
            //     pessoasHATEOAS.Add(pessoaHATEOAS);
            // }
            
            //return Ok(pessoasHATEOAS);
            return Ok(pessoas);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)  //listar uma pessoa especifica
        {
            try{
                Pessoa pessoa = database.Pessoas.First(p => p.Id == id);
                
                //ViewBag.Eventos = database.Eventos.Where(p => p.Status == true).ToList();
                if(pessoa.Status == true)
                {
                    
                    return Ok(pessoa);
                }else{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Essa pessoa foi deletada"});
                }
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Não encontrado"});
            }
        }

        // [HttpGet("PessoaEventos, {id}")] //deve-se passar o id do evento
        // public IActionResult PessoaEventos(int id) //achar pessoas que estao em eventos em comum
        // {
        //     List<Evento> evento = new List<Evento>();
        //     int IDEvento;
        //     foreach (var eve in evento)
        //     {
        //         if(eve.Id == id)
        //         {
        //            IDEvento  = eve.Id;
        //         }
        //     }

        //     var pessoas = database.Pessoas.Include(e => e.EventoId).Where(e => e.Status == true && e.EventoId == IDEvento).ToList();
        //     return Ok(pessoas);
        // }

        [HttpPost]
        public IActionResult Post([FromBody] PessoaTemp pTemp)
        {
            //validação
            if(pTemp.Nome.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Nome Invalido"});
            }

            if(pTemp.Telefone.Length <= 7)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Telefone Invalido"});
            }

            if(pTemp.Cpf.Length != 11)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Cpf Invalido"});
            }

            try{
                Pessoa p = new Pessoa();
                p.Nome = pTemp.Nome;
                p.Telefone = pTemp.Telefone;
                p.Cpf = pTemp.Cpf;
                p.Status = true;
               
                database.Pessoas.Add(p);
                database.SaveChanges();
                
               foreach (var eve in pTemp.EventosId)
               {
                   PessoaEvento pe = new PessoaEvento();

                   pe.Pessoa = database.Pessoas.First(e => e.Id == p.Id);
                   pe.Evento = database.Eventos.First(p => p.Id == eve);

                   database.PessoasEventos.Add(pe);
                   database.SaveChanges();
               }

                Response.StatusCode = 201;
                return new ObjectResult("Cadastrado com sucesso");
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao cadastrar"});
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)  //deletar uma pessoa
        {
            try{
                Pessoa p = database.Pessoas.First(p => p.Id == id);
                
                p.Status = false;
                //database.Pessoas.Remove(p);
                database.SaveChanges();

                Response.StatusCode = 201;
                return new ObjectResult("Pessoa deletada");
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao tentar deletar"});
            }
        }

        [HttpPatch]
        public IActionResult Patch([FromBody] Pessoa pessoa) //editar uma pessoa
        {
            if(pessoa.Id > 0)
            {
                try{
                    var pes = database.Pessoas.First(p => p.Id == pessoa.Id);

                    if(pes != null)
                    {
                        //editar com codicao
                        pes.Nome = pessoa.Nome != null ? pessoa.Nome : pes.Nome;
                        pes.Telefone = pessoa.Telefone != null ? pessoa.Telefone : pes.Telefone;
                        pes.Cpf = pessoa.Cpf != null ? pessoa.Cpf : pes.Cpf;
                        //pes.EventoId = pessoa.EventoId != null ? pessoa.EventoId : pes.EventoId;
                        //pes.UsuarioId = pessoa.UsuarioId != null ? pessoa.UsuarioId : pes.UsuarioId;

                        database.SaveChanges();
                        Response.StatusCode = 201;
                        return new ObjectResult(new {msg = "Edição feita com sucesso"});
                    }else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Pessoa não encontrada"});
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Pessoa não encontrada"});
                }
            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O Id do pessoa é inválido!"});
            }
        }

        public class PessoaTemp
        {
            public int Id {get; set;}
            public string Nome {get; set;}
            public string Telefone {get;set;} //telefone de conato
            public string Cpf {get; set;}
            public bool Status {get; set;} //deleta uma pessoa
            //public int UsuarioId {get; set;} //pegar o id do usuario
            //public int EventoId {get; set;} //pegar o id de evento
            public List<int> EventosId {get;set;}
        }
        
        public class PessoaContainer
        {
            public Pessoa pessoa;
            public Link[] links;
        }
    }
}