using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MyChat.Models
{
    public class ChatContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public ChatContext(DbContextOptions<ChatContext> options) : base(options) { }
    }
}
