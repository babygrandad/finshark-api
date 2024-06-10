using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Account
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string Token { get; set; }
    }
}