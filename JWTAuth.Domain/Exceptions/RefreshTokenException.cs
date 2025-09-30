using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuth.Domain.Exceptions
{
    public class RefreshTokenException(string message) : Exception(message)
    {
    }
}
