using System.Security.Claims;

namespace Eklee.Azure.Functions.Http.Models
{
    public class Security
    {
        public Principal Principal { get; set; }
		public ClaimsPrincipal ClaimsPrincipal { get; set; }
    }
}
