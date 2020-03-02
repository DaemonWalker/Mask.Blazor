using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mask.Blazor.Utils
{
    public class CommonValue
    {
        public static string PublicKey { get; set; } = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDFZLucigIvl/AAliSrlP0QI8vxB11C9iAEsvvZto3A/yh9MIlCoKVFbUvqAEuLpxJxMqTDDJA4C7xoukAcyXJTEiEILeqBbqSxDlsxh+L3msaim+ZKKoUnJvxuekJyFOi9H0seZbS/WytkqKhKmATOe0w94JMHFkFFON4QyERehwIDAQAB";
        public static string ConnectionString { get; set; }
        public static string HangfireDashboardUrl { get; set; } = "/dashboard";
    }
}
