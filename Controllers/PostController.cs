using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MyApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        public IActionResult Dodaj()
        {
            return View(new Post());
        }

        [Authorize]
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

        [Authorize]
        public async Task<IActionResult> Edytuj(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            var user = await _userManager.GetUserAsync(User);

            if (post == null)
                return NotFound();

            // sprawdź, czy aktualny użytkownik to autor posta
            if (post.AppUserId != user?.Id)
                return Forbid(); // zwraca 403 - brak dostępu

            return View(post);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edytuj(int id, Post post)
        {
            var originalPost = await _context.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            var user = await _userManager.GetUserAsync(User);

            if (originalPost == null)
                return NotFound();

            if (originalPost.AppUserId != user?.Id)
                return Forbid();

            if (ModelState.IsValid)
            {
                try
                {
                    post.AppUserId = originalPost.AppUserId; // zachowaj autora
                    post.DataDodania = originalPost.DataDodania;
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return StatusCode(500, "Błąd podczas zapisywania zmian");
                }
            }

            return View(post);
        }


        [Authorize]
        public async Task<IActionResult> Usun(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            var user = await _userManager.GetUserAsync(User);

            if (post == null)
                return NotFound();

            if (post.AppUserId != user?.Id)
                return Forbid();

            return View(post);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UsunPost(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            var user = await _userManager.GetUserAsync(User);

            if (post == null)
                return NotFound();

            if (post.AppUserId != user?.Id)
                return Forbid();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

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
