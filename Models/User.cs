using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyChat.Models
{
    public class User : IdentityUser<int>
    {
        public string? Avatar { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }
        [JsonIgnore]
        public List<Message> Messages { get; set; }
        public bool IsBlocked()
        {
            return LockoutEnabled && LockoutEnd > DateTimeOffset.UtcNow;
        }
    }
}
