using System.ComponentModel.DataAnnotations;

namespace UserSystem.Services.Models.Input
{
    public class RegisterSystemUserInputDto
    {
        [Required, StringLength(20)]
        public string FirstName { get; set; }

        [Required, StringLength(20)]
        public string LastName { get; set; }

        [Required, StringLength(20)]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(50)]
        public string Password { get; set; }
    }
}
