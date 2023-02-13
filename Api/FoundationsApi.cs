using Auxiliary;
using Foundations.Models;
using SteelSeries.GameSense;
using System.Timers;
using Terraria;
using Terraria.ID;

namespace Foundations.Api
{
    public class FoundationsApi
    {
        public const string setter = "ResearchUnlocked";
        public const ushort researchCount = 5453;

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

        public TimeFreeze FreezeTimeInfo()
            => timeFreeze;

        public void StartFreezeTime(TimeFreeze.TimeOfDay time = TimeFreeze.TimeOfDay.Custom)
           => Foundations.core.timeFreeze.Start(time);

        public void StopFreezeTime()
           => Foundations.core.timeFreeze.Stop();

        public async Task<PlayerHome> GetHome(string homeName, string accountName, int worldId)
        {
            var entity = await IModel.GetAsync(GetRequest.Bson<PlayerHome>(x => x.AccountName == accountName && x.Name == homeName && x.WorldId == worldId));
            return entity;
        }

        public List<Item> Items = new List<Item>();

    }

}
