using Mask.Blazor.Data;
using Mask.Blazor.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mask.Blazor.Hangfire.Jobs
{
    public class CheckJob
    {
        private IServiceProvider serviceProvider;
        private MaskDataBase maskDataBase;
        public CheckJob(MaskDataBase maskDataBase, IServiceProvider serviceProvider)
        {
            this.maskDataBase = maskDataBase;
            this.serviceProvider = serviceProvider;
        }

        public void DoJob(bool isToday)
        {
            var users = maskDataBase.GetAllNeedAppointmentUsers().ConfigureAwait(false).GetAwaiter().GetResult();
            var appointmentDate = isToday ? DatetimeUtils.GetChinaDatetimeNow() : DatetimeUtils.GetChinaDatetimeNow().AddDays(1);
            var targetDate = appointmentDate.ToString("yyyy-MM-dd");
            Parallel.ForEach(users, user =>
            {
                if (serviceProvider.GetService<MaskWebClient>().IsSuccessed(user.IDCard, targetDate, out var code))
                {
                    user.LastAppointmentDate = targetDate;
                    user.AppointmentCode = code;
                    maskDataBase.AddOrUpdateUser(user).Wait();
                }
            });
        }
    }
}
