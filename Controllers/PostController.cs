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
            var post = _context.Posts.Include(p => p.AppUser)
                .Include(p => p.Komentarze)
                .ThenInclude(k => k.AppUser)
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

            if (post.AppUserId != user?.Id)
                return Forbid();

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
                    post.AppUserId = originalPost.AppUserId;
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajKomentarz(int postId, string tresc)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(tresc))
            {
                ModelState.AddModelError("Komentarz", "Komentarz nie może być pusty.");
                return RedirectToAction("Szczegoly", new { id = postId });
            }

            var komentarz = new Komentarz
            {
                Tresc = tresc,
                PostId = postId,
                AppUserId = user.Id,
                AppUser = user,
                DataDodania = DateTime.Now
            };

            _context.Komentarze.Add(komentarz);
            await _context.SaveChangesAsync();
            return RedirectToAction("Szczegoly", new { id = postId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UsunKomentarz(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Forbid();

            var komentarz = await _context.Komentarze
                .Include(k => k.Post)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (komentarz == null) return NotFound();

            bool jestAutoremKomentarza = komentarz.AppUserId == user.Id;
            bool jestAutoremPosta = komentarz.Post.AppUserId == user.Id;

            if (!jestAutoremKomentarza && !jestAutoremPosta)
            {
                return Forbid();
            }

            _context.Komentarze.Remove(komentarz);
            await _context.SaveChangesAsync();

            return RedirectToAction("Szczegoly", new { id = komentarz.PostId });
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
