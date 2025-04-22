using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MyApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public PostController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Dodaj()
        {
            return View(new Post());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(Post post)
        {
            if (!ModelState.IsValid)
            {
                return View(post);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Forbid();
            }

            post.AppUserId = user.Id;
            post.DataDodania = DateTime.Now;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        public IActionResult Szczegoly(int id)
        {
            var post = _context.Posts
                .Include(p => p.AppUser)
                .FirstOrDefault(p => p.Id == id);


            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        public IActionResult Edytuj(int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edytuj(int id, Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Posts.Update(post);
                    _context.SaveChanges();
                }
                catch (Exception)
                {
                    return StatusCode(500, "Błąd podczas zapisywania zmian");
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        public IActionResult Usun(int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UsunPost(int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Index()
        {
            var posts = _context.Posts
                .Include(p => p.AppUser)
                .ToList();

            return View(posts);
        }
    }
}
