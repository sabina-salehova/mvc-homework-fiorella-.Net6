using Microsoft.AspNetCore.Identity;

namespace test.Models.IdentityModels
{
    public class User:IdentityUser
    {
        public string Fullname { get; set; }
    }
}
