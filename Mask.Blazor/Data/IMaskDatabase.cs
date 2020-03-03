using Mask.Blazor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mask.Blazor.Data
{
    public interface IMaskDatabase
    {
        Task<ShopModel> GetShop(string shopID);
        Task<UserModel> GetUser(string id);
        Task<List<ShopModel>> GetAllShops();
        Task<List<UserModel>> GetUsers();
        Task<List<UserModel>> GetAllNeedAppointmentUsers();
        Task AddOrUpdateUser(UserModel userInfo);
        Task<List<ShopModel>> UpdateShops();
        Task<AccountModel> Login(AccountModel accountModel);
        Task<List<AccountModel>> GetAccounts();
    }
}
