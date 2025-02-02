using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace UserSystem.Services.Models.Input
{
    public class UpdateSystemUserInputDto
    {
        [Required]
        public string UserId { get; set; }

        [Required, StringLength(20)]
        public string FirstName { get; set; }

        [Required, StringLength(20)]
        public string LastName { get; set; }

        [Required, StringLength(20)]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [StringLength(50)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[a-zA-Z\\d@$!%*?&]{8,16}$",
            ErrorMessage = "add at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
        public string? Password { get; set; }

        [Required]
        public List<string> Roles { get; set; }


        //action by
        [SwaggerIgnore]
        public string? CurrentUserId { get; set; }
    }
}
