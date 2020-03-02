using Mask.Blazor.Data;
using Mask.Blazor.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Mask.Blazor.Services
{
    public class UserService
    {
        private readonly MaskDataBase maskDataBase;
        public UserService(MaskDataBase maskDataBase)
        {
            this.maskDataBase = maskDataBase;
        }

        public async Task AddUser(UserModel userInfo)
        {
            await maskDataBase.AddOrUpdateUser(userInfo);
        }
        public async Task<ResultModel> GetAppointmentResult(string id)
        {
            var user = await maskDataBase.GetUser(id);
            if (user == null)
            {
                return null;
            }
            var shop = await maskDataBase.GetShop(user.ShopID);
            return new ResultModel()
            {
                AppointmentCode = user.AppointmentCode,
                AppointmentDate = user.LastAppointmentDate,
                ShopAddress = shop.serviceAddress,
                ShopName = shop.serviceName
            };
        }

        public async Task<AccountModel> Login(AccountModel account)
        {
            return await maskDataBase.Login(account);
           
        }
    }
}
