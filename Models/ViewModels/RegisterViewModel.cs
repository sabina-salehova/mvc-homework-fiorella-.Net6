using System.ComponentModel.DataAnnotations;

namespace test.Models.ViewModels
{
    public class RegisterViewModel
    {
        public string Fullname { get; set; }
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmedPassword { get; set; }
    }
}
