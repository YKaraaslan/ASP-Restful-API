using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Tier.Entities
{
    public class User : IdentityUser
    {
        [StringLength(30)]
        public string Name { get; set; }
        [StringLength(20)]
        public string Surname { get; set; }

        [StringLength(50)]
        public string Department { get; set; }

        [StringLength(50)]
        public string Position { get; set; }
    }
}
