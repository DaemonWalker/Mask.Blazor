using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mask.Blazor.Hangfire.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {

        public bool Authorize([NotNull] DashboardContext context) => 
            int.TryParse(context.GetHttpContext().AuthenticateAsync().ConfigureAwait(false).GetAwaiter().GetResult().Principal.Claims
                .FirstOrDefault(p => p.Type == ClaimTypes.Role).Value, out var level) && 
            level > 1;
    }
}
