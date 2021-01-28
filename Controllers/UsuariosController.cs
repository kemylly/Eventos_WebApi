using treino_api.Models;
using Microsoft.AspNetCore.Mvc;
using treino_api.Data;
using System.Text;
using System.Linq;
using System;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace treino_api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        public readonly ApplicationDbContext databse;
        public UsuariosController(ApplicationDbContext database)
        {
            this.databse = database;
        }

        //registrando um usuario
        [HttpPost("registro")]
        public IActionResult Registro([FromBody] Usuario usuario)
        {
            Regex r = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");
            if(r.IsMatch (usuario.Email)) //verificar o email
            {
                //verificar se tem um email igual já cadasrado
                var email = databse.Usuarios.FirstOrDefault(u => u.Email == usuario.Email);
                if(email != null)
                {
                    Response.StatusCode = 400;
                    return new ObjectResult(new{msg = "Email já cadastrado"});
                }
            }else{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Email Invalido"});
            }

            if(usuario.Senha.Length < 6) // a senha deve coter mais de 5 caracteries
            {
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "A senha tem que ter mais de 5 caracteries"});    
            }
            
            try{ // tentando o  cadastro de usuario
                databse.Add(usuario);
                databse.SaveChanges();
                return Ok(new{msg="Usuario cadastrado com sucesso"});
            }catch{
                Response.StatusCode = 400;
                return new ObjectResult(new{msg = "Erro ao cadastrar"});
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario credencial)
        {
            //buscar um usuario por e-mail
            // verificar se a senha está correta
            //gerar um token TWT e retornar esse token para o usario
            try
            {
                Usuario usuario = databse.Usuarios.First(user => user.Email.Equals(credencial.Email));
                if(usuario != null)
                {
                    if(usuario.Senha.Equals(credencial.Senha))
                    {
                        //chave de segurança
                        string chaveDeSegurana = "kemylly_cavalcante_santos";
                        var chaveSimetrica = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveDeSegurana));
                        var credenciaisDeAcesso = new SigningCredentials(chaveSimetrica,SecurityAlgorithms.HmacSha256Signature);

                        var claims = new List<Claim>();
                        claims.Add(new Claim("id",usuario.Id.ToString())); //claim que guarda o id do usuario
                        claims.Add(new Claim("email",usuario.Email)); //pegar o email do usuario e colocar em uma claim
                        claims.Add(new Claim(ClaimTypes.Role,"Admin")); //pegar tipo do usuario

                        //criando o token e coisas necessarias
                        var JWT = new JwtSecurityToken(
                            issuer: "eventos.com",  //issuer = quem esta fornecendo o jwt ao usuario 
                            expires: DateTime.Now.AddHours(1), //quando expira
                            audience: "usuario_comum", //para quem esta destinado esse token
                            signingCredentials: credenciaisDeAcesso,  //credenciais de acesso de token
                            claims : claims
                        );

                        //return Ok();
                        return Ok(new JwtSecurityTokenHandler().WriteToken(JWT)); //gerar token
                    }
                    else{
                        //usuario errou a senha
                        Response.StatusCode = 401;
                        return new ObjectResult(new {msg = "Senha invalida"});
                    }
                }
                else{
                    Response.StatusCode = 401;  //401 = não autorizado
                    return new ObjectResult(new {msg = "Email invalido"});
                }
            }
            catch (System.Exception)
            {
                Response.StatusCode = 401;  //401 = não autorizado
                return new ObjectResult(new {msg = "Usuario invalido"});
            }
        }
    }
}