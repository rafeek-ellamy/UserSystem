using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserSystem.Data.Enums;

namespace UserSystem.Data.Entities
{
    public class UserProfile : IdentityUser
    {
        [Required, MaxLength(20)]
        public string FirstName { get; set; }

        [Required, MaxLength(20)]
        public string LastName { get; set; }

        public EUserType UserTypeId { get; set; }
        //public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
