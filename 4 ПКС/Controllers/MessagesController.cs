using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pks4.Models;
using System;
using System.Linq;
using YourProjectName.Data;
using YourProjectName.Models;
using System.Collections.Generic;

namespace pks4.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string from, DateTime? startDate, DateTime? endDate, string status)
        {
            var userId = _context.Users.FirstOrDefault(u => u.Login == User.Identity.Name)?.Id;
            var messagesQuery = _context.Messages
                .Include(m => m.User)
                .Where(m => m.ToUserId == userId);

            if (!string.IsNullOrEmpty(from))
            {
                messagesQuery = messagesQuery.Where(m => m.From == from);
            }

            if (startDate.HasValue)
            {
                messagesQuery = messagesQuery.Where(m => m.SentDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                messagesQuery = messagesQuery.Where(m => m.SentDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "unread")
                {
                    messagesQuery = messagesQuery.Where(m => m.Status == "new");
                }
            }

            var messages = messagesQuery.OrderByDescending(m => m.SentDate).ToList();
            var viewModel = new MessagesViewModel
            {
                Messages = messages
            };

            return View(viewModel);
        }
    }
}

namespace pks4.Models
{
    public class MessagesViewModel
    {
        public List<Message> Messages { get; set; }
    }
}
