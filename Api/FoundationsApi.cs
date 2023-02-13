using Auxiliary;
using Foundations.Models;
using System.Timers;
using Terraria;
using Terraria.ID;

namespace Foundations.Api
{
    public class FoundationsApi
    {
        public TimeFreeze timeFreeze = new TimeFreeze();

        public FoundationsApi()
        {
            for (var i = 0; i > ItemID.Count; i++)
            {
                Item item = new Item();
                item.SetDefaults(i);
                Items.Add(item);
            }

        }

        public async Task<PlayerHome> GetHome(string homeName, string accountName, int worldId)
        {
            var entity = await IModel.GetAsync(GetRequest.Bson<PlayerHome>(x => x.AccountName == accountName && x.Name == homeName && x.WorldId == worldId));
            return entity;
        }

        public List<Item> Items = new List<Item>();

    }

}
