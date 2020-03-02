using Mask.Blazor.Data;
using Mask.Blazor.Models;
using Mask.Blazor.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mask.Blazor.Hangfire.Jobs
{
    public class AppointmentJob
    {
        private readonly IServiceProvider serviceProvider;
        private readonly MaskDataBase maskDataBase;
        private static readonly TimeSpan endTime = new TimeSpan(21, 0, 0);
        public AppointmentJob(MaskDataBase maskDataBase, IServiceProvider serviceProvider)
        {
            this.maskDataBase = maskDataBase;
            this.serviceProvider = serviceProvider;
        }
        public void DoJob()
        {
            var users = maskDataBase.GetAllNeedAppointmentUsers().ConfigureAwait(false).GetAwaiter().GetResult();
            var taskArray = users.Select(p => MakeAppointment4User(p)).ToArray();

            Task.WaitAll(taskArray);
        }

        private Task MakeAppointment4User(UserModel userInfo)
        {
            var shopInfo = maskDataBase.GetShop(userInfo.ShopID).ConfigureAwait(false).GetAwaiter().GetResult();
            var parm = new RequestParm()
            {
                pharmacyName = shopInfo.serviceName,
                pharmcayId = shopInfo.id,
                pharmacyAddress = shopInfo.serviceAddress,
                realName = userInfo.Name.RSAEncrypt(),
                idcard = userInfo.IDCard.RSAEncrypt(),
                mobile = userInfo.Tel.RSAEncrypt()
            };
            var cts = new CancellationTokenSource();
            return Task.Run(() =>
            {
                while (DatetimeUtils.GetChinaDatetimeNow().TimeOfDay < endTime)
                {
                    Enumerable.Range(1, 10).Select(p => new Task(() =>
                    {

                        if (serviceProvider.GetService<MaskWebClient>().MakeAppointment(parm, out var result))
                        {
                            cts.Cancel();
                        }
                        else
                        {

                        }
                    }, cts.Token));
                    Thread.Sleep(10000);
                }
            }, cts.Token);
        }
    }
}
