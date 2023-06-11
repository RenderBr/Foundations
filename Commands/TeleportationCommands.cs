using Auxiliary;
using CSF;
using CSF.TShock;
using Foundations.Extensions;
using Foundations.Models;
using MongoDB.Driver;
using Terraria;
using TShockAPI;

namespace Foundations.Commands
{
	[RequirePermission("foundations.tp")]
	public class TeleportationCommands : TSModuleBase<TSCommandContext>
	{
		[Command("tpa")]
		[Description("Requests to teleport to a player")]
		[RequirePermission("request")]
		public IResult TpaCmd([Remainder] string args)
		{
			if (string.IsNullOrEmpty(args))
			{
				Foundations.core.AcceptRequest(Context.Player);
				return ExecuteResult.FromSuccess();
			}
			var plr = TSPlayer.FindByNameOrID(args);
			if (plr.Count == 0)
				return Error("No players matched your query.");

			if (plr.Count > 1)
				return Error("More than one player matched your query.");

			var target = plr[0];
			if (target == null)
				return Error("Player not found.");

			if (target == Context.Player)
				return Error("You can't teleport to yourself!");

			if (Foundations.core.HasRequest(Context.Player))
				return Error("You already have a pending request!");

			if (Foundations.core.HasRequest(target))
				return Error("That player already has a pending request!");

			Foundations.core.RequestTeleport(Context.Player, target);
			return Success("You have sent a teleport request to {0}. Use /tpd to cancel the request.", target.Name);
		}

		[Command("tpd")]
		[Description("Denies or cancels a teleport request")]
		public IResult DenyTp()
		{
			Foundations.core.DenyRequest(Context.Player);
			return ExecuteResult.FromSuccess();
		}

		[Command("home")]
		[RequirePermission("home")]
		public async Task<IResult> HomeCmd(string name = "")
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				Error("You must specify a home name!");

				var temp = await StorageProvider.GetMongoCollection<PlayerHome>("PlayerHomes").
					Find(x => x.AccountName == Context.Player.Account.Name).
					ToListAsync();

				if (temp.Count == 0)
					return Error("You don't have any homes! Set one with /sethome");

				Success($"You have {temp.Count} homes. Please choose one of your existing homes!");

				string message = "Home List: ";

				foreach (PlayerHome ph in temp)
				{
					if (temp.IndexOf(ph) + 1 == temp.Count)
					{
						message += ph.Name;
						break;
					}
					message += ph.Name + ", ";

				}
				Info(message);
			}

			var home = await Foundations.core.GetHome(name, Context.Player.Account.Name, Main.worldID);

			if (home == null)
			{
				return Error("You do not have a home by that name!");
			}

			if (Context.Player.Teleport(home.X, home.Y))
			{
				return Success("You have been teleported to your home!");
			}
			else
			{
				return Error("You cannot teleport to your home at this time!");
			}
		}

		[Command("back")]
		[RequirePermission("back")]
		public IResult Back()
		{
			if (Foundations.core.CanGoBack(Context.Player))
			{
				var last = Foundations.core.GetBack(Context.Player);
				if (last is null)
					return Error("You can't go back!");
				Context.Player.Teleport(last.X, last.Y);
				return Success("You've gone back.");
			}
			else return Error("You can't go back!");
		}

		[Command("sethome")]
		[RequirePermission("home")]
		public async Task<IResult> SetHomeCmd(string name = "")
		{
			if (String.IsNullOrWhiteSpace(name))
				return Error("You must specify a home name!");

			var home = await Foundations.core.GetHome(name, Context.Player.Account.Name, Main.worldID);

			if (home == null)
			{

				await IModel.CreateAsync(CreateRequest.Bson<PlayerHome>(x =>
				{
					x.Name = name;
					x.AccountName = Context.Player.Account.Name;
					x.X = (int)(Context.Player.X * 16);
					x.Y = (int)(Context.Player.Y * 16);
					x.WorldId = Main.worldID;
				}));

			}
			else
			{
				home.X = (int)(Context.Player.X * 16);
				home.Y = (int)(Context.Player.Y * 16);
				home.WorldId = Main.worldID;
			}

			return Success("Your home has been set!");
		}

		[Command("homes")]
		[Description("Displays a list of all your homes.")]
		public IResult ListHomes()
		{
			var list = Foundations.core.GetAllHomes(Context.Player.Account.Name, Main.worldID);
			if (list.Count <= 0)
				return Error("You don't have any homes!");

			var msg = "You currently own the following homes: ";
			foreach (PlayerHome home in list)
			{
				msg += home.Name + ", ";
			}

			msg = msg.TrimEnd(',', ' '); // Remove the trailing comma and space

			return Success(msg);
		}

		[Command("down")]
		[Description("Teleports the user down x levels")]
		[RequirePermission("direct")]
		public async Task<IResult> Down(int levels = 1)
		{
			TSPlayer player = Context.Player;
			int currentLevel = 0;
			bool empty = false;
			int x = Math.Max(0, Math.Min(player.TileX, Main.maxTilesX - 2));
			int y = Math.Max(0, Math.Min(player.TileY + 3, Main.maxTilesY - 3));

			await Task.Run(() =>
			{
				for (int j = y; currentLevel < levels && j < Main.maxTilesY - 2; j++)
				{
					if (Main.tile[x, j].IsEmpty() && Main.tile[x + 1, j].IsEmpty() &&
						Main.tile[x, j + 1].IsEmpty() && Main.tile[x + 1, j + 1].IsEmpty() &&
						Main.tile[x, j + 2].IsEmpty() && Main.tile[x + 1, j + 2].IsEmpty())
					{
						empty = true;
					}
					else if (empty)
					{
						empty = false;
						currentLevel++;
						y = j;
					}
				}
			});

			if (currentLevel == 0)
				return Error("Could not teleport down!");
			else
			{
				player.Teleport(16 * x, 16 * y - 10);
				return Success("Teleported down {0} level{1}.", currentLevel, currentLevel == 1 ? "" : "s");
			}
		}

		[Command("left")]
		[Description("Teleports the user left x times")]
		[RequirePermission("direct")]
		public async Task<IResult> Left(int times = 1)
		{
			TSPlayer player = Context.Player;
			int currentTimes = 0;
			bool empty = false;
			int x = Math.Max(0, Math.Min(player.TileX - 3, Main.maxTilesX - 3));
			int y = Math.Max(0, Math.Min(player.TileY, Main.maxTilesY - 2));

			await Task.Run(() =>
			{
				for (int i = x; currentTimes < times && i >= 0; i--)
				{
					if (Main.tile[i, y].IsEmpty() && Main.tile[i, y + 1].IsEmpty() &&
						Main.tile[i + 1, y].IsEmpty() && Main.tile[i + 1, y + 1].IsEmpty() &&
						Main.tile[i + 2, y].IsEmpty() && Main.tile[i + 2, y + 1].IsEmpty())
					{
						empty = true;
					}
					else if (empty)
					{
						empty = false;
						currentTimes++;
						x = i;
					}
				}
			});

			if (currentTimes == 0)
				return Error("Could not teleport left!");
			else
			{
				player.Teleport(16 * x - 10, 16 * y);
				return Success("Teleported left {0} time{1}.", currentTimes, currentTimes == 1 ? "" : "s");
			}
		}

		[Command("up")]
		[Description("Teleports the user up x levels")]
		[RequirePermission("direct")]
		public async Task<IResult> Up(int levels = 1)
		{
			TSPlayer player = Context.Player;
			int currentLevel = 0;
			bool empty = false;
			int x = Math.Max(0, Math.Min(player.TileX, Main.maxTilesX - 2));
			int y = Math.Max(0, Math.Min(player.TileY - 3, Main.maxTilesY - 3));

			await Task.Run(() =>
			{
				for (int j = y; currentLevel < levels && j >= 0; j--)
				{
					if (Main.tile[x, j].IsEmpty() && Main.tile[x + 1, j].IsEmpty() &&
						Main.tile[x, j + 1].IsEmpty() && Main.tile[x + 1, j + 1].IsEmpty() &&
						Main.tile[x, j + 2].IsEmpty() && Main.tile[x + 1, j + 2].IsEmpty())
					{
						empty = true;
					}
					else if (empty)
					{
						empty = false;
						currentLevel++;
						y = j;
					}
				}
			});

			if (currentLevel == 0)
				return Error("Could not teleport up!");
			else
			{
				player.Teleport(16 * x, 16 * y + 10);
				return Success("Teleported up {0} level{1}.", currentLevel, currentLevel == 1 ? "" : "s");
			}
		}

		[Command("right")]
		[Description("Teleports the user right x times")]
		[RequirePermission("direct")]
		public async Task<IResult> Right(int times = 1)
		{
			TSPlayer player = Context.Player;
			int currentTimes = 0;
			bool empty = false;
			int x = Math.Max(0, Math.Min(player.TileX + 3, Main.maxTilesX - 3));
			int y = Math.Max(0, Math.Min(player.TileY, Main.maxTilesY - 2));

			await Task.Run(() =>
			{
				for (int i = x; currentTimes < times && i < Main.maxTilesX - 2; i++)
				{
					if (Main.tile[i, y].IsEmpty() && Main.tile[i, y + 1].IsEmpty() &&
						Main.tile[i + 1, y].IsEmpty() && Main.tile[i + 1, y + 1].IsEmpty() &&
						Main.tile[i + 2, y].IsEmpty() && Main.tile[i + 2, y + 1].IsEmpty())
					{
						empty = true;
					}
					else if (empty)
					{
						empty = false;
						currentTimes++;
						x = i;
					}
				}
			});

			if (currentTimes == 0)
				return Error("Could not teleport right!");
			else
			{
				player.Teleport(16 * x + 10, 16 * y);
				return Success("Teleported right {0} time{1}.", currentTimes, currentTimes == 1 ? "" : "s");
			}
		}



		[Command("delhome")]
		[Description("Deletes a home.")]
		public async Task<IResult> DeleteHome(string args)
		{
			if (!String.IsNullOrEmpty(args))
			{
				if (await Foundations.core.HomeExists(args, Context.Player.Account.Name, Main.worldID))
				{
					Foundations.core.DeleteHome(args, Context.Player.Account.Name, Main.worldID);
					return Success($"You deleted {args} successfully!");
				}
				else return Error($"{args} does not exist!");
			}
			else return Error("Please enter a valid home name! Use /homes to see your homes.");
		}

	}
}
