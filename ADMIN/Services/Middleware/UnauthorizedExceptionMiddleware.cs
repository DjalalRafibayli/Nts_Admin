using ADMIN.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ADMIN.Services.Middleware
{
    public class UnauthorizedExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UnauthorizedExceptionMiddleware> _logger;
        //private readonly ResourceManager resourceManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UnauthorizedExceptionMiddleware(RequestDelegate next, ILogger<UnauthorizedExceptionMiddleware> logger, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            //this.resourceManager = new
            //ResourceManager("EmployeeException.API.Resources.Resource",
            //typeof(UnauthorizedExceptionMiddleware).Assembly);
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await this._next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                await this.HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            if (ex.InnerException is UnauthorizedException)
            {
                httpContext.Response.Redirect("/login/logout");
            }
            else
            {
                throw ex;
            }
        }
    }
}
