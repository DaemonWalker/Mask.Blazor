using Hangfire;
using Hangfire.Dashboard;
using Mask.Blazor.Data;
using Mask.Blazor.Hangfire.Filters;
using Mask.Blazor.Hangfire.Jobs;
using Mask.Blazor.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Mask.Blazor.Utils
{
    public static class Extenssions
    {
        public static void AddMask(this IServiceCollection serviceDescriptors)
        {
            serviceDescriptors.AddSingleton<IMaskDatabase, MaskDatabase>();
            serviceDescriptors.AddSingleton<UserService>();
            serviceDescriptors.AddSingleton<MaskService>();
            serviceDescriptors.AddTransient<MaskWebClient>();
            serviceDescriptors.AddAuthorizationCore();
        }
        public static void SetHangfireAndSetTasks(this IApplicationBuilder app)
        {
            app.UseHangfireServer();
            app.UseHangfireDashboard(
                pathMatch: CommonValue.HangfireDashboardUrl,
                options: new DashboardOptions()
                {
                    Authorization = new List<IDashboardAuthorizationFilter>() { new HangfireAuthorizationFilter() },
                    IsReadOnlyFunc = contenxt => false
                });
            var chinaTimeZone = DatetimeUtils.ChinaTimeZone;
            RecurringJob.AddOrUpdate<CheckJob>(methodCall: job => job.DoJob(false), cronExpression: "0 0 23 * * ?", recurringJobId: "firstdaycheck", timeZone: chinaTimeZone);
            RecurringJob.AddOrUpdate<CheckJob>(methodCall: job => job.DoJob(true), cronExpression: "0 0 8 * * ?", recurringJobId: "seconddaycheck", timeZone: chinaTimeZone);
            RecurringJob.AddOrUpdate<AppointmentJob>(methodCall: job => job.DoJob(), cronExpression: "0 59 19 * * ?", recurringJobId: "makeappointment", timeZone: chinaTimeZone);
            RecurringJob.AddOrUpdate<RefreshJob>(methodCall: job => job.DoJob(), cronExpression: "0 0 4/12 * * ? ", recurringJobId: "dorefresh", timeZone: chinaTimeZone);
        }

        public static void AddBlazorAuth(this IServiceCollection services)
        {
            // ******
            // BLAZOR COOKIE Auth Code (begin)
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            // BLAZOR COOKIE Auth Code (end)
            // ******
            // ******
            // BLAZOR COOKIE Auth Code (begin)
            // From: https://github.com/aspnet/Blazor/issues/1554
            // HttpContextAccessor
            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();
            services.AddHttpClient();
            services.AddScoped<HttpClient>();
            // BLAZOR COOKIE Auth Code (end)
            // ******
        }

        public static void UseBlazorAuth(this IApplicationBuilder app)
        {
            // ******
            // BLAZOR COOKIE Auth Code (begin)
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            // BLAZOR COOKIE Auth Code (end)
            // ******
        }

        public static void UseEnvironmentValue(this IApplicationBuilder app)
        {
            var hangfireDashboardUrl = Environment.GetEnvironmentVariable(nameof(CommonValue.HangfireDashboardUrl));
            if (string.IsNullOrWhiteSpace(hangfireDashboardUrl))
            {
                throw new Exception("pls set rsa public hangfireDashboardUrl");
            }
            CommonValue.HangfireDashboardUrl = hangfireDashboardUrl;

            var connectionString = Environment.GetEnvironmentVariable(nameof(CommonValue.ConnectionString));
            if (string.IsNullOrWhiteSpace(connectionString) == false)
            {
                CommonValue.ConnectionString = connectionString;
            }

            CommonValue.ThreadNum = Environment.GetEnvironmentVariable(nameof(CommonValue.ThreadNum));
            CommonValue.SleepTime = Environment.GetEnvironmentVariable(nameof(CommonValue.SleepTime));
        }

        public static T ConvertFromRedisHash<T>(this Dictionary<string, string> dictionay) where T : new()
        {
            var type = typeof(T);
            var obj = new T();
            foreach (var kv in dictionay)
            {
                type.GetProperty(kv.Key).SetValue(obj, kv.Value);
            }
            return obj;
        }

        public static object[] ConvertToRedisHash<T>(this T t) =>
            typeof(T).GetProperties()
            .Where(p => p.GetCustomAttribute<ConvertToIgnoreAttribute>() == null || p.GetCustomAttribute<ConvertToIgnoreAttribute>().Ignore == false)
            .SelectMany(p => new object[] { p.Name, p.GetValue(t) })
            .ToArray();
    }
}
