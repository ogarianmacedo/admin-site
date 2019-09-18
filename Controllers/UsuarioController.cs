using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
                _contexto.Add(usuario);
                await _contexto.SaveChangesAsync();

                return RedirectToAction("Index", "Usuario");
            }

            return View(usuario);
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
                if(_contexto.Usuarios.Any(u => u.Email == login.Email && u.Senha == login.Senha && u.Ativo == true))
                {
                    int id = _contexto.Usuarios.Where(u => u.Email == login.Email && u.Senha == login.Senha).Select(u => u.UsuarioId).Single();

                    HttpContext.Session.SetInt32("UsuarioId", id);
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

        public async Task<IActionResult> Sair()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Usuario");
        }

        [Authorize]
        public async Task<IActionResult> Editar(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var usuario = await _contexto.Usuarios.FindAsync(id);
            return View(usuario);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var usuarioBanco = _contexto.Usuarios.Any(u => u.UsuarioId == usuario.UsuarioId);
                if (usuarioBanco)
                {
                    try
                    {
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
            var usuarios = _contexto.Usuarios.ToList();

            StringBuilder arquivo = new StringBuilder();

            arquivo.AppendLine("Nome;Email;Ativo");
            foreach(var item in usuarios)
            {
                arquivo.AppendLine(item.Nome + ";" + item.Email + ";" + item.Ativo);
            }

            return File(Encoding.ASCII.GetBytes(arquivo.ToString()), "text/csv", "dados-usuario.csv");
        }
    }
}