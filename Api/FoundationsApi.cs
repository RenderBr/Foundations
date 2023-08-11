using Auxiliary;
using Foundations.Models;
using Microsoft.Xna.Framework;
using MongoDB.Driver;
using Terraria;
using Terraria.ID;
using TShockAPI;
using TShockAPI.DB;

namespace Foundations.Api
{
	public class FoundationsApi
	{
		public const string setter = "ResearchUnlocked";
		public const ushort researchCount = 5453;
		public List<TeleportRequest> tpRequests = new();

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
		public Version GetVersion() => Foundations.version;

		public async Task ChangePlayerTime(string accName, bool day, int frames, bool enabled = true)
		{
			var p = await GetUser(accName);
            p.PlayerTime.Day = day;
            p.PlayerTime.Frames = frames;
            p.PlayerTime.Enabled = enabled;
        }

        public async Task<FoundationUser> GetUser(string accName) => await IModel.GetAsync(GetRequest.Bson<FoundationUser>(x => x.Account == accName), x => x.Account = accName);

        public bool RequestTeleport(TSPlayer requester, TSPlayer target)
		{
			if (tpRequests.Any(x => x.Requester.Name == requester.Name || x.Target.Name == target.Name))
			{
				return false;
			}

			tpRequests.Add(new TeleportRequest()
			{
				Requester = requester,
				Target = target,
				RequestTime = DateTime.Now,
				Accepted = false
			});
			target.SendMessage($"{requester.Account.Name} has requested to teleport to you!", Color.LightYellow);
			target.SendMessage("Type /tpa to accept the request.", Color.LightYellow);
			target.SendMessage("Type /tpd to deny the request.", Color.Red);
			return true;
		}

		public void HasTeleported(TSPlayer player)
		{
			TrackedLocation loc = new()
			{
				AccountName = player.Account.Name,
				X = player.X,
				Y = player.Y,
				Time = DateTime.Now
			};
		}

		public void TShockConfigWrite()
		{
			var configPath = Path.Combine("tshock", "config.json");
			TShock.Config.Write(configPath);
		}

		public bool CanGoBack(TSPlayer player) => StorageProvider.GetMongoCollection<TrackedLocation>("TrackedLocations").Find(x => x.Reverted == false).Any();

		public TrackedLocation? GetBack(TSPlayer player)
		{
			var e = StorageProvider.GetMongoCollection<TrackedLocation>("TrackedLocations").FindOneAndDelete(x => x.AccountName == player.Account.Name && x.Reverted == false);
			e.Reverted = true;
			return e;
		}
		
		public bool HasRequest(TSPlayer player) => tpRequests.Any(x => x.Target.Name == player.Name || x.Requester.Name == player.Name);

		public void DenyRequest(TSPlayer executor)
		{
			if (tpRequests.Any(x => x.Requester.Name == executor.Name))
			{
				tpRequests.Remove(tpRequests.Find(x => x.Requester.Name == executor.Name));
				executor.SendMessage("You have cancelled the teleport request!", Color.LightYellow);
			}
			else if (tpRequests.Any(x => x.Target.Name == executor.Name))
			{
				tpRequests.Remove(tpRequests.Find(x => x.Target.Name == executor.Name));
				executor.SendMessage("You have denied the incoming teleport request!", Color.LightYellow);
			}
			else
			{
				executor.SendMessage("You don't have any pending teleport requests!", Color.Red);
			}
		}

		public void AcceptRequest(TSPlayer executor)
		{
			if (tpRequests.Any(x => x.Target.Name == executor.Name))
			{
				var request = tpRequests.Find(x => x.Target.Name == executor.Name);
				request.Requester.Teleport(executor.TileX, executor.TileY);
				executor.SendMessage($"{request.Requester.Name} has been teleported to you!", Color.LightGreen);
				request.Requester.SendMessage($"You have been teleported to {executor.Name}!", Color.LightGreen);
				tpRequests.Remove(request);
			}
			else
			{
				executor.SendMessage("You don't have any pending teleport requests!", Color.Red);
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

		public List<PlayerHome> GetAllHomes(string accountName, int worldId) => StorageProvider.GetMongoCollection<PlayerHome>("PlayerHomes").Find(x => x.AccountName == accountName && x.WorldId == worldId).ToList();

		public async Task<bool> HomeExists(string homeName, string accountName, int worldId)
		{
			var entity = await IModel.GetAsync(GetRequest.Bson<PlayerHome>(x => x.AccountName == accountName && x.Name == homeName && x.WorldId == worldId));
			return entity != null;
		}

		public void DeleteHome(string homeName, string accountName, int worldId) => StorageProvider.GetMongoCollection<PlayerHome>("PlayerHomes").FindOneAndDelete(GetRequest.Bson<PlayerHome>(x => x.AccountName == accountName && x.Name == homeName && x.WorldId == worldId));

		public List<Item> Items = new List<Item>();

	}

}
