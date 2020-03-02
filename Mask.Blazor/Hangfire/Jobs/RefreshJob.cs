using Mask.Blazor.Data;
using Mask.Blazor.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mask.Blazor.Hangfire.Jobs
{
    public class RefreshJob
    {
        private readonly IServiceProvider serviceProvider;
        private readonly MaskDataBase maskDataBase;
        public RefreshJob(IServiceProvider serviceProvider, MaskDataBase maskDataBase)
        {
            this.serviceProvider = serviceProvider;
            this.maskDataBase = maskDataBase;
        }
        public void DoJob()
        {
            maskDataBase.UpdateShops().Wait();
            var key = serviceProvider.GetService<MaskWebClient>().GetRSAKey();
            if (string.IsNullOrWhiteSpace(key) == false)
            {
                CommonValue.PublicKey = key;
            }
        }
    }
}
