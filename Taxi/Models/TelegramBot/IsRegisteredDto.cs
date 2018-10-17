using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Models.TelegramBot
{
    public class IsRegisteredDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
