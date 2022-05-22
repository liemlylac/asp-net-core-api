using System.Linq;
using System.Threading.Tasks;
using ASPNetCoreAPI.Services;
using Microsoft.AspNetCore.Http;

namespace ASPNetCoreAPI.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
        {
           
            var token = (string)context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = jwtUtils.ValidateToken(token, "access");
            if (userId != null)
            {
                context.Items["User"] = userService.GetById(userId.Value);
            }

            await _next(context);
        }
    }
}