using AppDev1.Areas.Identity.Data;
using AppDev1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AppDev1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<AppUser> _userManager;
        private readonly UserContext _context;
        private readonly int _recordsPerPage = 5;


        public HomeController(ILogger<HomeController> logger, UserContext context, IEmailSender emailSender, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _emailSender = emailSender;
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "Customer")]
        public IActionResult ForCustomerOnly()
        {
            ViewBag.message = "This is for Customer only! Hi " + _userManager.GetUserName(HttpContext.User);
            return View("Views/Home/Index.cshtml");
        }

        /*[Authorize(Roles = "Seller")]
        public IActionResult ForSellerOnly()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            Store User = _context.Store.FirstOrDefault(s => s.UserId == thisUserId);
            *//*ViewBag.message = "This is for Store Owner only!";
            return View("Views/Home/Index.cshtml");*//*
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            if (User == null)
            {
                return View("Views/Store/Create.cshtml");
            }
            else
            {
                return View("Views/Book/Index.cshtml");

            }
        }*/

        public async Task<IActionResult> Index(int id=0)
        {
            var userContext = _context.Book.Include(b => b.Store);
            int numberOfRecords = await _context.Book.CountAsync();     //Count SQL
            int numberOfPages = (int)Math.Ceiling((double)numberOfRecords / _recordsPerPage);
            ViewBag.numberOfPages = numberOfPages;
            ViewBag.currentPage = id;
            List<Book> books = await _context.Book
                .Skip(id * _recordsPerPage)  //Offset SQL
                .Take(_recordsPerPage)       //Top SQL
                .ToListAsync();

            return View(books);
            
        }
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}