using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzArena_AdminPanel.API.Login
{
    public class LoginResponse
    {
        public UserResult Result { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
    }
}
