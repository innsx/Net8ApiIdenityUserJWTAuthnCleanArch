using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuth.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireAtUtc { get; set; }


        public static User Create(string email, string firstName, string lastName)
        {
            User user = new User 
            { 
                FirstName = firstName, 
                LastName = lastName, 
                Email = email,
                UserName = email
            };

            return user;
        }

        //By using override, you are providing a more meaningful and human-readable representation for your specific type.
        //indicates the intention to provide a custom string representation for an object of a class or struct.
        //Overriding ToString() allows you to define how an object of your class or struct should be represented as a string.
        public override string ToString()
        {
            return FirstName + " "+ LastName;
        }
    }
}
