using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjetoAdminSite.Controllers
{
    [Authorize]
    public class InscricaoExternaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}