using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mask.Blazor.Models
{
    public class ShopModel
    {
        public string serviceName { get; set; }
        public string serviceAddress { get; set; }
        public string id { get; set; }
        public override string ToString()
        {
            return $"{serviceName}";
        }
    }
}
