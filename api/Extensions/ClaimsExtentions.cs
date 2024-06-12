using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Extensions
{
    public static class ClaimsExtentions
    {
        public static string GetUsername(this ClaimsPrincipal user){
            //this link might have been deprecated so if it does not work, ask chat gpt to give you an update.
            return user.Claims.SingleOrDefault(x => x.Type.Equals("http://schemas.xmlsoap.org.ws/2005/05/identity/claims/givenname")).Value;
         }
    }
}