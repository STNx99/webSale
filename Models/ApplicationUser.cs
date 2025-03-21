using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebsiteBanHang.Models
{
    public class ApplicationUser
    {
        [Required]
        public string FullName { get; set; }
        public string? Address { get; set; }
        public string? Age { get; set; }

    }
}

