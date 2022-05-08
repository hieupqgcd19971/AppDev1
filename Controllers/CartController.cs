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
using Microsoft.AspNetCore.Identity;

namespace AppDev1.Controllers
{
    public class CartController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly UserContext _context;

        public CartController(UserContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Cart
        public async Task<IActionResult> Index()
        {
            var userContext = _context.Cart.Include(c => c.Book).Include(c => c.User);
            return View(await userContext.ToListAsync());
        }

        // GET: Cart/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Book)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Cart/Create
        public IActionResult Create()
        {
            ViewData["BookIsbn"] = new SelectList(_context.Book, "BookIsbn", "BookIsbn");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Cart/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,BookIsbn")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookIsbn"] = new SelectList(_context.Book, "BookIsbn", "BookIsbn", cart.BookIsbn);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cart.UserId);
            return View(cart);
        }

        // GET: Cart/Edit/5
        public async Task<IActionResult> Edit(string uid,string isbn)
        {
            if (uid == null || isbn == null)
            {
                return NotFound();
            }

            var cart =await _context.Cart.FirstOrDefaultAsync(c => c.UserId == uid && c.BookIsbn == isbn);
            if (cart == null)
            { 
                return NotFound();
            }
            
            return View(cart);
        }

        // POST: Cart/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,BookIsbn,Quantity")] Cart cart)
        {
            /* if (id != cart.UserId)
             {
                 return NotFound();
             }

             if (ModelState.IsValid)
             {
                 try
                 {
                     _context.Update(cart);
                     await _context.SaveChangesAsync();
                 }
                 catch (DbUpdateConcurrencyException)
                 {
                     if (!CartExists(cart.UserId))
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
             ViewData["BookIsbn"] = new SelectList(_context.Book, "BookIsbn", "BookIsbn", cart.BookIsbn);
             ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cart.UserId);
             return View(cart);*/
            try
            {
                _context.Cart.Update(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction("Edit", new { uid = cart.UserId, bid = cart.BookIsbn });
            }
        }

        // GET: Cart/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Book)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cart = await _context.Cart.FindAsync(id);
            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(string id)
        {
            return _context.Cart.Any(e => e.UserId == id);
        }
        public async Task<IActionResult> DeleteItem(string Isbn)
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            var cart = await _context.Cart.Where(s => s.UserId == userid && s.BookIsbn == Isbn).FirstOrDefaultAsync();
            _context.Cart.Remove(cart);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
