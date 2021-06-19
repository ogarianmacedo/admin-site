using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoAdminSite.Models;
using ProjetoAdminSite.ViewModels;

namespace ProjetoAdminSite.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly Contexto _contexto;

        public UsuarioController(Contexto contexto)
        {
            _contexto = contexto;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var usuarios = _contexto.Usuarios.ToListAsync();
            return View(await usuarios);
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.Session.Clear();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Entrar(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var retornoMd5 = RetornarMD5(login.Senha);

                var usuarioBanco = _contexto.Usuarios.FirstOrDefault(u => u.Email == login.Email && u.Senha == retornoMd5 && u.Ativo == true);
                
                if (usuarioBanco != null)
                {
                    HttpContext.Session.SetInt32("UsuarioId", usuarioBanco.UsuarioId);
                    HttpContext.Session.SetString("UsuarioNome", usuarioBanco.Nome);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, login.Email)
                    };

                    var userIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                    await HttpContext.SignInAsync(principal);

                    return RedirectToAction("Index", "Blog");
                }
                else
                {
                    ViewBag.AlertaLogin = "Usuário não existe ou está inativo!";
                }
            }

            return View("Login");
        }

        [Authorize]
        public async Task<IActionResult> Sair()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Usuario");
        }

        [Authorize]
        public IActionResult Cadastro()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CadastrarUsuario(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var senha = RetornarMD5(usuario.Senha);
                usuario.Senha = senha;

                _contexto.Add(usuario);
                await _contexto.SaveChangesAsync();

                return RedirectToAction("Index", "Usuario");
            }

            return View(usuario);
        }

        [Authorize]
        public async Task<IActionResult> Editar(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var usuario = await _contexto.Usuarios.FindAsync(id);
            TempData["UsuarioPass"] = usuario.Senha;
            return View(usuario);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var usuarioExiste = _contexto.Usuarios.Any(u => u.UsuarioId == usuario.UsuarioId);
                if (usuarioExiste)
                {
                    try
                    {
                        if(usuario.Senha != null)
                        {
                            var senha = RetornarMD5(usuario.Senha);
                            usuario.Senha = senha;
                        }
                        else
                        {
                            usuario.Senha = TempData["UsuarioPass"].ToString();
                        }
                     
                        _contexto.Update(usuario);
                        await _contexto.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;
                    }

                    return RedirectToAction("Index", "Usuario");
                }
                return View(usuario);
            }
            return View(usuario);
        }

        public IActionResult GerarCSV()
        {
            var usuarios = _contexto.Usuarios.Include(u => u.Blogs).ToList();

            StringBuilder arquivo = new StringBuilder();

            arquivo.AppendLine("Nome;Email;Status;");

            foreach(var item in usuarios)
            {
                var usuarioAtivo = item.Ativo == true ? "Ativo" : "Inativo";
                arquivo.AppendLine(item.Nome + ";" + item.Email + ";" + usuarioAtivo + ";");
            }

            return File(Encoding.ASCII.GetBytes(arquivo.ToString()), "text/csv", "dados-usuario.csv");
        }
             
        public string RetornarMD5(string Senha)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return RetonarHash(md5Hash, Senha);
            }
        }

        private string RetonarHash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public bool ComparaMD5(string senhabanco, string senhamd5)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                var senha = RetornarMD5(senhabanco);
                if (VerificarHash(md5Hash, senhamd5, senha))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool VerificarHash(MD5 md5Hash, string input, string hash)
        {
            StringComparer compara = StringComparer.OrdinalIgnoreCase;

            if (0 == compara.Compare(input, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}