using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    public interface ITokenBuilder
    {
        string BuildToken(User user);
    }
}
