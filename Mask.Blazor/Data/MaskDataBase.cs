using CSRedis;
using Mask.Blazor.Models;
using Mask.Blazor.Utils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mask.Blazor.Data
{
    public class MaskDatabase : IMaskDatabase
    {
        const string ShopIDListKey = "ShopIDs";
        const string ShopIDKey = "Shop";
        const string IDListKey = "IDs";
        const string IDKey = "ID";
        const string AccountKey = "Account";
        const string AccountListKey = "Accounts";
        private readonly CSRedisClient database;
        private readonly IServiceProvider serviceProvider;
        public MaskDatabase(IServiceProvider serviceProvider)
        {
            database = new CSRedisClient(CommonValue.ConnectionString);
            RedisHelper.Initialization(database);
            this.serviceProvider = serviceProvider;
        }
        public async Task<ShopModel> GetShop(string shopID)
        {
            var shopHash = await database.HGetAllAsync(GetShopIDKey(shopID));
            var shopInfo = shopHash.ConvertFromRedisHash<ShopModel>();
            return shopInfo;
        }

        public async Task<UserModel> GetUser(string id)
        {
            var idHash = await database.HGetAllAsync(GetIDKey(id));
            if (idHash == null || idHash.Count == 0)
            {
                return null;
            }

            var userInfo = idHash.ConvertFromRedisHash<UserModel>();
            return userInfo;
        }
        public async Task<List<ShopModel>> GetAllShops() => (await database.SMembersAsync(ShopIDListKey)).Select(p => GetShop(p).ConfigureAwait(false).GetAwaiter().GetResult()).ToList();
        public async Task<List<UserModel>> GetUsers() => (await database.SMembersAsync(IDListKey)).Select(p => GetUser(p).ConfigureAwait(false).GetAwaiter().GetResult()).ToList();
        public async Task<List<UserModel>> GetAllNeedAppointmentUsers() => (await GetUsers()).Where(p => DateTime.TryParse(p.LastAppointmentDate, out DateTime dt) == false || (DatetimeUtils.GetChinaDatetimeNow() - dt).TotalDays > 5).ToList();
        public async Task AddOrUpdateUser(UserModel userInfo)
        {
            var entries = userInfo.ConvertToRedisHash();
            await database.SAddAsync(IDListKey, userInfo.IDCard);
            await database.HMSetAsync(GetIDKey(userInfo.IDCard), entries);
        }
        public async Task<List<ShopModel>> UpdateShops()
        {
            try
            {
                var list = await serviceProvider.GetService<MaskWebClient>().GetShopList();
                foreach (var shop in list)
                {
                    await database.SAddAsync(ShopIDListKey, shop.id);
                    await database.HMSetAsync(GetShopIDKey(shop.id), shop.ConvertToRedisHash());
                }
                return list;
            }
            catch
            {
                return new List<ShopModel>();
            }
        }
        public async Task<AccountModel> Login(AccountModel accountModel)
        {
            var account = await database.HGetAllAsync(GetAccountKey(accountModel.Account));
            return account.ConvertFromRedisHash<AccountModel>();
        }

        public async Task<List<AccountModel>> GetAccounts() => 
            (await database.SMembersAsync(AccountListKey))
            .Select(accont => database.HGetAllAsync(GetAccountKey(accont)).ConfigureAwait(false).GetAwaiter().GetResult().ConvertFromRedisHash<AccountModel>())
            .ToList();

        private static string GetShopIDKey(string shopID) => $"{ShopIDKey}{shopID}";
        private static string GetIDKey(string id) => $"{IDKey}{id}";
        private static string GetAccountKey(string account) => $"{AccountKey}{account}";

    }
}
