using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoAdminSite.Models;

namespace ProjetoAdminSite.Controllers
{
    [Authorize]
    public class InscricaoExternaController : Controller
    {
        private readonly Contexto _contexto;

        public InscricaoExternaController(Contexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<IActionResult> Index()
        {
            var inscricoes = _contexto.InscricoesExterna.ToListAsync();
            return View(await inscricoes);
        }

        public IActionResult GerarCSV()
        {
            var inscricoes = _contexto.InscricoesExterna.ToList();

            StringBuilder arquivo = new StringBuilder();

            arquivo.AppendLine("Nome;Email;Telefone;Mensagem;Data Inscrição");
            foreach(var item in inscricoes)
            {
                arquivo.AppendLine(item.Nome + ";" + item.Email + ";" + item.Telefone + ";" + item.Mensagem + ";" + item.DtInscricao);
            }

            return File(Encoding.ASCII.GetBytes(arquivo.ToString()), "text/csv", "inscricoes.csv");
        }
    }
}