using Dapper;
using POS.Models;
using System.Security.Claims;

namespace POS.Services
{
    public interface IServiceUser
    {
        int GetUserId();
    }

    public class ServiceUser : IServiceUser
    {
		private readonly HttpContext httpContext;

		public ServiceUser(IHttpContextAccessor httpContextAccessor)
		{
			httpContext = httpContextAccessor.HttpContext;
		}
        public int GetUserId()
        {
			if (httpContext.User.Identity.IsAuthenticated)
			{
                var idClaim = httpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();
                var id = int.Parse(idClaim.Value);
                return id;
			}
			else
			{
                throw new ApplicationException("The user is not authenticated");
			}
        }

    }
}
