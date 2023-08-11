using CSF;
using CSF.TShock;
using Foundations.Models;
using MongoDB.Driver;
using Terraria;
using Terraria.ID;
using TShockAPI;

namespace Foundations.Commands
{
	[RequirePermission("foundations.util")]
	public class UtilityCommands : TSModuleBase<TSCommandContext>
	{
	
		[Command("under")]
		[Description("Places a block underneath you")]
		public IResult Under(string type = "glass")
		{
			int id;
			bool success = TileID.Search.TryGetId(type, out id);
			if (success == false)
				return Error("Invalid tile");

			WorldGen.PlaceTile((int)(Context.Player.X / 16), (int)(Context.Player.Y / 16), id, false, false);
			TSPlayer.All.SendTileSquareCentered((int)(Context.Player.X / 16), (int)(Context.Player.Y / 16), 5);
			return Success($"Placed {TileID.Search.GetName(id)} underneath you");
		}

		[Command("rain", "stoprain", "togglerain")]
		[Description("Toggles rain")]
		[RequirePermission("rain")]
		public IResult Rain()
		{
			Main.raining = !Main.raining;
			return Success(Main.raining ? "It's now raining!" : "It's no longer raining!");
		}


		[Command("ptime", "playertime")]
		[Description("Sets the time for a client player")]
		[RequirePermission("ptime")]
		private void ZAPTime(CommandArgs args)
		{
			PlayerTime temp = new PlayerTime() { Frames = -2, Day = true };

			if (args.Parameters.Count == 1)
			{
				switch (args.Parameters[0].ToLower())
				{
					case "day":
						temp.Day = true;
						temp.Frames = (int)Time.DAY;
						break;
					case "night":
						temp.Day = false;
						temp.Frames = (int)Time.NIGHT;
						break;
					case "noon":
						temp.Day = true;
						temp.Frames = (int)Time.NOON;
						break;
					case "midnight":
						temp.Day = false;
						temp.Frames = (int)Time.MIDNIGHT;
						break;
					case "off":
						temp.Day = true;
						temp.Frames = -1;
						break;
					default:
						break;
				}
			}

			if (temp.Enabled == false)
			{
				if (playertime.ContainsKey(args.Player.Index))
				{
					playertime.Remove(args.Player.Index);
					args.Player.SendSuccessMessage("Set your time to server time.");
					SendData(new SendDataEventArgs() { MsgId = PacketTypes.WorldInfo, Handled = false, remoteClient = -1 });
					return;
				}
				else
				{
					args.Player.SendErrorMessage("Your time is already the server time!");
					return;
				}
			}

			if (temp.frames == -2)
			{
				args.Player.SendErrorMessage("Invalid usage: /ptime <day/noon/night/midnight/off>");
				return;
			}

			if (playertime.ContainsKey(args.Player.Index))
				playertime[args.Player.Index] = temp;
			else
				playertime.Add(args.Player.Index, temp);

			SendData(new SendDataEventArgs() { MsgId = PacketTypes.WorldInfo, Handled = false, remoteClient = -1 });

			args.Player.SendSuccessMessage("Set your personal time to {0}.", args.Parameters[0]);
		}
#endregion

		//wip, boring & tedious, will do later
		[Command("find")]
		[Description("FILL THIS OUT LATER")]
		public async Task<IResult> FindCmd(string type = "", string search = "", int page = 1)
		{
			switch (type.ToLowerInvariant())
			{
				case "-command":
				case "command":

					List<TShockAPI.Command> cmd = new List<TShockAPI.Command>();

					await Task.Run(() => cmd = TShockAPI.Commands.ChatCommands.FindAll(x => x.Names.Contains(search) || x.Name == search));

					List<string> commands = new List<string>();

					foreach (TShockAPI.Command c in cmd)
					{
						commands.Add(String.Format("{0} (Permission: {1})", c.Name, c.Permissions.FirstOrDefault()));
					}

					PaginationTools.SendPage(Context.Player, page, commands,
							new PaginationTools.Settings
							{
								HeaderFormat = "Found Commands ({0}/{1}):",
								FooterFormat = String.Format("Type /find -command {0} {{0}} for more", search),
								NothingToDisplayString = "No commands were found."
							});

					return ExecuteResult.FromSuccess();
				case "item":
				case "-item":
					var itemList = new List<Item>();

					itemList = TShock.Utils.GetItemByIdOrName(search);

					List<string> items = new List<string>();

					foreach (Item i in itemList)
					{
						items.Add(String.Format("{0} (ID: {1})", i.Name, i.netID));
					}
					if (itemList.Count == 0)
					{
						return Error("No items were found!");
					}

					PaginationTools.SendPage(Context.Player, page, items,
						new PaginationTools.Settings
						{
							HeaderFormat = "Found Items ({0}/{1}):",
							FooterFormat = String.Format("Type /find -item {0} {{0}} for more", search),
							NothingToDisplayString = "No items were found."
						});
					return ExecuteResult.FromSuccess();

				default:
					Error("Invalid syntax! Proper syntax: {0}find <-switch> <name...> [page]",
						 TShock.Config.Settings.CommandSpecifier);
					Success("Valid {0}find switches:", TShock.Config.Settings.CommandSpecifier);
					Info("-command: Finds a command.");
					Info("-item: Finds an item.");
					Info("-npc: Finds an NPC.");
					Info("-tile: Finds a tile.");
					return Info("-wall: Finds a wall.");

			}

		}
	}
}
