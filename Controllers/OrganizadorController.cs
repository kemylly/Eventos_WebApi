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
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] //so acessa quem tiver autorizado
    public class OrganizadorController : ControllerBase
    {
        private readonly ApplicationDbContext database;

        public OrganizadorController(ApplicationDbContext database)
        {
            this.database = database;
        }

        [HttpGet]
        public IActionResult Get() //listar todos os organizadores
        {
            var organizadores = database.Organizadores.Where(o => o.Status == true).ToList();
            
            return Ok(organizadores);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id) //listar um organizador especifico
        {
            try{
                Organizador organizador = database.Organizadores.First(o => o.Id ==id);
                
                if(organizador.Status == true){
                    return Ok(organizador);
                }
                else{
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Esse organizador foi deletado"});
                }
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Não encontrado"});
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] OrganizadorTemp oTemp)
        {
            /*validadcao*/
            if(oTemp.Nome.Length <= 1)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Nome Invalido"});
            }

            if(oTemp.Telefone.Length <= 7) //verifica o tamanho do telefone
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Telefone Invalido"});
            }

            Regex r = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
            if(r.IsMatch (oTemp.Email)) //verificar o email
            {
                
            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Email Invalido"});
            }

            if(oTemp.Cpf.Length != 11) // verificar o tamanho do cpf
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "CPF Invalido"});
            }

            if(oTemp.Rede.Length <=2)
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Rede social Invalida"});
            }
            
            // if(oTemp.Eventos == null)
            // {
            //     Response.StatusCode = 400;
            //     return new ObjectResult(new{msg = "Evento Invalida"});
            // }

            try{
                //salvar o organizador
                Organizador o = new Organizador();
                o.Nome = oTemp.Nome;
                o.Telefone = oTemp.Telefone;
                o.Email = oTemp.Email;
                o.Cpf = oTemp.Cpf;
                o.Rede = oTemp.Rede;
                o.Status = true;
                //o.Eventos = oTemp.Eventos;

               

                database.Organizadores.Add(o);
                database.SaveChanges();
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao cadastrar"});
            }

            Response.StatusCode = 201;
            return new ObjectResult("Cadastrado com sucesso");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id) //deletar um organizador
        {
            try{
                Organizador org = database.Organizadores.First(o => o.Id == id);
                
                org.Status = false;
                database.SaveChanges();

                Response.StatusCode = 201;
                return new ObjectResult("Organizador deletado");
                //return Ok();

            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao tentar deletar"});
            }
        }

        [HttpPatch]
        public IActionResult Patch([FromBody] Organizador organizador) //editar
        {
            if(organizador.Id > 0)
            {
                try{
                    var org = database.Organizadores.First(o => o.Id == organizador.Id);

                    if(org != null)
                    {
                        //editar com condiçao
                        org.Nome = organizador.Nome != null ? organizador.Nome : org.Nome;
                        org.Telefone = organizador.Telefone != null ? organizador.Telefone : org.Telefone;
                        org.Email = organizador.Email != null ? organizador.Email : org.Email;
                        org.Cpf = organizador.Cpf != null ? organizador.Cpf : org.Cpf;
                        org.Rede = organizador.Rede != null ? organizador.Rede : org.Rede;

                        database.SaveChanges();
                        Response.StatusCode = 201;
                        return new ObjectResult(new {msg = "Edição feita com sucesso"});
                        //return Ok();

                    }else{
                        Response.StatusCode = 400;
                        return new ObjectResult(new {msg = "Organizador não encontrado"});
                    }
                }catch{
                    Response.StatusCode = 400;
                    return new ObjectResult(new {msg = "Organizador não encontrado"});
                }
            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new {msg = "O Id do organizador é inválido!"});
            }
        }

        public class OrganizadorTemp
        {
            public int Id {get; set;}
            public string Nome {get; set;}  
            public string Telefone {get;set;} 
            public string Email {get;set;}
            public string Cpf {get; set;} 
            public string Rede {get; set;} 
            public bool Status {get; set;} 
            public List<EventoOrganizador> Eventos {get; set;}
        }
    }
}