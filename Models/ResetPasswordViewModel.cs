using System.ComponentModel.DataAnnotations;

namespace test.Models
{
    public class ResetPasswordViewModel
    {
        public string Email{ get; set; }
        public string Token { get; set; }
        public string Password{ get; set; }

        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
