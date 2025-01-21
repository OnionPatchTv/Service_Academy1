using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_Academy1.Models;
using System.Security.Claims;

namespace Service_Academy1.Controllers
{
    [Authorize]
    public class DiscussionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscussionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Display posts for a specific program
        public async Task<IActionResult> DiscussionForum(int programId)
        {
            if (!User.IsInRole("ProjectLeader") && !User.IsInRole("Student"))
            {
                return Forbid();
            }

            var posts = await _context.Posts
                .Include(p => p.Author) // Include author for the post
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author) // Include author for each comment
                .Where(p => p.ProgramId == programId)
                .ToListAsync();

            ViewBag.ProgramId = programId;
            return View(posts);
        }

        // Display a specific post with comments
        public async Task<IActionResult> PostPage(int postId)
        {
            if (!User.IsInRole("ProjectLeader") && !User.IsInRole("Student"))
            {
                return Forbid();
            }

            var post = await _context.Posts
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author) // Include author for each comment
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // Create a new post
        [HttpPost]
        public async Task<IActionResult> CreatePost(PostModel post, int programId)
        {
            if (!User.IsInRole("ProjectLeader") && !User.IsInRole("Student"))
            {
                return Forbid();
            }

            post.ProgramId = programId;
            post.CreatedDate = DateTime.UtcNow;
            post.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Assuming the user is logged in with their username as AuthorId

            if (ModelState.IsValid)
            {
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("DiscussionForum", new { programId = post.ProgramId });
            }
            return View(post);
        }

        [HttpGet]
        public IActionResult CreatePost(int programId)
        {
            if (!User.IsInRole("ProjectLeader") && !User.IsInRole("Student"))
            {
                return Forbid();
            }

            ViewBag.ProgramId = programId;
            return View();
        }

        // Add a comment to a post
        [HttpPost]
        public async Task<IActionResult> AddComment(CommentModel comment)
        {
            if (!User.IsInRole("ProjectLeader") && !User.IsInRole("Student"))
            {
                return Forbid();
            }

            comment.CreatedDate = DateTime.UtcNow;
            comment.AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Assuming the user is logged in with their username as AuthorId

            if (ModelState.IsValid)
            {
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("PostPage", new { postId = comment.PostId });
            }
            return View(comment);
        }
    }
}
