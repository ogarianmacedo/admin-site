using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoAdminSite.Models;

namespace ProjetoAdminSite.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        private readonly Contexto _contexto;
        private readonly IHostingEnvironment _hostingEnvironment;

        public BlogController(Contexto contexto, IHostingEnvironment hostingEnvironment)
        {
            _contexto = contexto;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = _contexto.Blogs.Include(b => b.Usuario).ToListAsync();
            return View(await blogs);
        }

        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CadastrarBlog(Blog blog, IFormFile imagem)
        {
            if (ModelState.IsValid)
            {
                var linkUpload = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                if(imagem != null)
                {
                    Random rdm = new Random();
                    var nomeImagem = rdm.Next(0,5000) + "_" + imagem.FileName;

                    using (var fileStream = new FileStream(Path.Combine(linkUpload, nomeImagem), FileMode.Create))
                    {
                        await imagem.CopyToAsync(fileStream);
                        blog.Imagem = "~/uploads/" + nomeImagem;
                        blog.UsuarioId = int.Parse(HttpContext.Session.GetInt32("UsuarioId").ToString());
                    }
                }

                _contexto.Add(blog);
                await _contexto.SaveChangesAsync();

                return RedirectToAction("Index", "Blog");
            }

            return View(blog);
        }

        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _contexto.Blogs.FindAsync(id);
            TempData["Imagem"] = blog.Imagem;
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarBlog(Blog blog, IFormFile imagem)
        {
            if (ModelState.IsValid)
            {
                var blogBanco = _contexto.Blogs.Any(b => b.BlogId == blog.BlogId);
                if (blogBanco)
                {
                    try
                    {
                        if (imagem != null)
                        {
                            //upload da imagem
                            var linkUpload = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

                            Random rdm = new Random();
                            var nomeImagem = rdm.Next(0, 5000) + "_" + imagem.FileName;

                            using (var fileStream = new FileStream(Path.Combine(linkUpload, nomeImagem), FileMode.Create))
                            {
                                await imagem.CopyToAsync(fileStream);
                                blog.Imagem = "~/uploads/" + nomeImagem;
                                blog.UsuarioId = int.Parse(HttpContext.Session.GetInt32("UsuarioId").ToString());
                            }
                        }
                        else
                        {
                            blog.Imagem = TempData["Imagem"].ToString();
                            blog.UsuarioId = int.Parse(HttpContext.Session.GetInt32("UsuarioId").ToString());
                        }
                        blog.DtPublicacao = DateTime.Now;

                        _contexto.Update(blog);
                        await _contexto.SaveChangesAsync();

                        return RedirectToAction("Index", "Blog");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;
                    }
                }
            }

            return View(blog);
        }

        public async Task<IActionResult> Visualizar(int id)
        {
            var blog = await _contexto.Blogs.Include(b => b.Usuario).FirstOrDefaultAsync(b => b.BlogId == id);
            return View(blog);
        }
    }
}