using System.ComponentModel.DataAnnotations;

namespace UserSystem.Services.Models.Input
{
    public class LoginInputDto
    {
        [Required, StringLength(20)]
        public string UserName { get; set; }

        [Required, StringLength(50)]
        public string Password { get; set; }
    }
}
