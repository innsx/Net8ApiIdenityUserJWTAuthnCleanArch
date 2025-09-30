using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuth.Domain.Exceptions
{
    public class RegisterationFailedException(IEnumerable<string> errorDescription) 
        : Exception($"Registration failed with following errors: {string.Join(Environment.NewLine, errorDescription)}")
    {
    }
}
