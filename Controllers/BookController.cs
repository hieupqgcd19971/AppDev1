#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppDev1.Areas.Identity.Data;
using AppDev1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AppDev1.Controllers
{
    public class BookController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly int maxofpage = 10;

        private readonly int rowonpage = 3;
        public BookController(UserContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Book
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Index(int id=0,string searchString="")
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            var storeId = _context.Store.FirstOrDefault(s => s.UserId == userid);
            if(storeId == null)
            {
                return RedirectToAction("Create", "Store");
            }
            var books = from s in _context.Book
                        select s;

            books = books.Where(s =>s.Store.UserId==userid);
            ViewData["CurrentFilter"] = searchString;
            if (searchString != null)
            {
                books = books.Where(s => s.Title.Contains(searchString) || s.Category.Contains(searchString));
            }
            int numberOfRecords = await books.CountAsync();     //Count SQL
            int numberOfPages = (int)Math.Ceiling((double)numberOfRecords / rowonpage);
            ViewBag.numberOfPages = numberOfPages;
            ViewBag.currentPage = id;
            List<Book> listbookseller = await books
                .Skip(id * rowonpage)  //Offset SQL
                .Take(rowonpage)       //Top SQL
                .ToListAsync();
            return View(listbookseller);
           
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.BookIsbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [Authorize(Roles = "Seller")]
        public IActionResult ForSellerOnly()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            Store User = _context.Store.FirstOrDefault(s => s.UserId == thisUserId);
            /*ViewBag.message = "This is for Store Owner only!";
            return View("Views/Home/Index.cshtml");*/
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            if (User == null)
            {
                return View("Views/Store/Create.cshtml");
            }
            else
            {
                return RedirectToAction("index", "Book");

            }
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            var userid = _userManager.GetUserId(HttpContext.User);

            ViewData["StoreId"] = _context.Store.Where(s => s.UserId == userid).FirstOrDefault().Name;
            return View();
        }
        public async Task<IActionResult> DetailBook(string Isbn, int quantity)
        {
            ViewData["quantitytest"] = quantity;
            /*Book detailbook = _context.Book.Where(s => s.BookIsbn == Isbn).FirstOrDefault();*/
            var book = await _context.Book.FirstOrDefaultAsync(m => m.BookIsbn == Isbn);
            return View(book);
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookIsbn,Title,Pages,Author,Category,Price,Desc")] Book book, IFormFile image)
        {
            if (image != null)
            {
                string imgName = book.BookIsbn + Path.GetExtension(image.FileName);
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imgName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                book.ImgUrl = imgName;

                var thisUserId = _userManager.GetUserId(HttpContext.User);
                Store thisStore = await _context.Store.FirstOrDefaultAsync(s => s.UserId == thisUserId);
                book.StoreId = thisStore.Id;
            }
            else
            {
                return View(book);
            }
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Book");

        }

        public async Task<IActionResult> Search(int id = 0, string searchString = "")
        {
            ViewData["CurrentFilter"] = searchString;
            var books = from s in _context.Book
                        select s;
            if(searchString != null)
            {
                books = books.Where(s => s.Title.Contains(searchString) || s.Category.Contains(searchString));
            }
            int numberOfRecords = await books.CountAsync();     //Count SQL
            int numberOfPages = (int)Math.Ceiling((double)numberOfRecords / rowonpage);
            ViewBag.numberOfPages = numberOfPages;
            ViewBag.currentPage = id;
            List<Book> listbook = await books
                .Skip(id * rowonpage)  //Offset SQL
                .Take(rowonpage)       //Top SQL
                .ToListAsync();

            return View("Views/Book/Search.cshtml", listbook);
        }
        [Authorize(Roles = "Seller")]
        public IActionResult Dashboard()
        {
            return View("Views/Book/Index.cshtml");
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            var userid = _userManager.GetUserId(HttpContext.User);

            ViewData["StoreId"] = _context.Store.Where(s => s.UserId == userid).FirstOrDefault().Name;
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BookIsbn,Title,Pages,Author,Category,Price,Desc,ImgUrl,StoreId")] Book book, IFormFile image)
        {
            if (id != book.BookIsbn)
            {
                return NotFound();
            }
            if (image != null)
            {
                string imgName = book.BookIsbn + Path.GetExtension(image.FileName);
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imgName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                book.ImgUrl = imgName;
            }
            /*else
            {
                return View(book);
            }*/

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookIsbn))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            /*ViewData["StoreId"] = new SelectList(_context.Store, "Id", "Id", book.StoreId);*/
            return View(book);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.BookIsbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var book = await _context.Book.FindAsync(id);
            _context.Book.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(string id)
        {
            return _context.Book.Any(e => e.BookIsbn == id);
        }

        [Authorize(Roles = "Customer")]

        public async Task<IActionResult> AddToCart(string Isbn, int quantity)
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            


            Cart fromDb = _context.Cart.FirstOrDefault(c => c.UserId == thisUserId && c.BookIsbn == Isbn);
            //if not existing (or null), add it to cart. If already added to Cart before, ignore it.
            if (fromDb == null)
            {
                Cart myCart = new Cart();
                myCart.UserId = thisUserId;
                myCart.BookIsbn = Isbn;
                myCart.Quantity = quantity;
                _context.Add(myCart);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index","Cart");
        }
        public async Task<IActionResult> Checkout()
        {
            string thisUserId = _userManager.GetUserId(HttpContext.User);
            List<Cart> myDetailsInCart = await _context.Cart
                .Where(c => c.UserId == thisUserId)
                .Include(c => c.Book)
                .ToListAsync();
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //Step 1: create an order
                    Order myOrder = new Order();
                    myOrder.UserId = thisUserId;
                    myOrder.OrderDate = DateTime.Now;
                    myOrder.Total = myDetailsInCart.Select(c => c.Book.Price)
                        .Aggregate((c1, c2) => c1 + c2);
                    _context.Add(myOrder);
                    await _context.SaveChangesAsync();

                    //Step 2: insert all order details by var "myDetailsInCart"
                    foreach (var item in myDetailsInCart)
                    {
                        OrderDetail detail = new OrderDetail()
                        {
                            OrderId = myOrder.Id,
                            BookIsbn = item.BookIsbn,
                            Quantity = 1
                        };
                        _context.Add(detail);
                    }
                    await _context.SaveChangesAsync();

                    //Step 3: empty/delete the cart we just done for thisUser
                    _context.Cart.RemoveRange(myDetailsInCart);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (DbUpdateException ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Error occurred in Checkout" + ex);
                }
            }
            return RedirectToAction("Index", "Cart");
        }

    }
}
