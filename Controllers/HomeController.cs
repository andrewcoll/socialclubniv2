using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Team()
        {
            return View();
        }

        public IActionResult Episodes()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
