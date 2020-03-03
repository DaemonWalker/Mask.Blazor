using Mask.Blazor.Data;
using Mask.Blazor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mask.Blazor.Services
{
    public class MaskService
    {
        private IMaskDatabase maskDataBase;
        public MaskService(IMaskDatabase maskDataBase)
        {
            this.maskDataBase = maskDataBase;
        }
        public async Task<List<ShopModel>> GetShops()
        {
            return await maskDataBase.GetAllShops();
        }
        public async Task<List<ShopModel>> UpdateShops()
        {
            return await maskDataBase.UpdateShops();
        }
    }
}
