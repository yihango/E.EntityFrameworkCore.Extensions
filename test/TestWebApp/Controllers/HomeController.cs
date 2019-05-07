using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestWebApp.Database;

namespace TestWebApp.Controllers
{
    public class HomeController : Controller
    {
        protected readonly BloggingContext _dbContext;

        public HomeController(BloggingContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var res = _dbContext.Blogs.ToList();

            return View();
        }
    }
}