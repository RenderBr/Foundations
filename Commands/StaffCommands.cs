using CSF;
using CSF.TShock;
using Foundations.Api;
using Microsoft.Xna.Framework;
using TShockAPI;

namespace Foundations.Commands
{
	[RequirePermission("foundations.staff")]
	public class StaffCommands : TSModuleBase<TSCommandContext>
	{

		[Command("sudo")]
		[RequirePermission("sudo")]
		public IResult SudoCmd(string player = "", [Remainder] string cmd = "")
		{
			if (player == "")
				return Error("You must specify a player!");

			if (cmd == "")
				return Error("You must specify a command!");

			var plr = TShock.Players.FirstOrDefault(x => x.Name == player);
			TShockAPI.Commands.HandleCommand(plr, cmd);
			return Success("You have sudo-ed the user!");
		}

		[Command("kickall")]
		[RequirePermission("kickall")]
		public IResult KickAll(string staff = "", [Remainder] string reason = "Kicked by administration.")
		{
			switch (staff)
			{
				default:
					foreach (TSPlayer plr in TShock.Players)
					{
						if (plr != null)
							plr.Kick(reason, true, false);

					}
					return Success("You kicked everyone!");
				case "staff":
				case "-s":
				case "notstaff":
					foreach (TSPlayer plr in TShock.Players)
					{
						if (!plr.HasPermission("foundations.staff"))
						{
							plr.Kick(reason, true, false);
						}
					}
					return Success("You kicked all non-staff");
			}
		}

		[Command("staffchat", "sc", "s")]
		[RequirePermission("chat")]
		public IResult StaffChat([Remainder] string message)
		{
			foreach (TSPlayer player in TShock.Players)
			{
				if (player != null && player.HasPermission("foundations.staff"))
				{
					player.SendMessage($"[StaffChat] {Context.Player.Name}: {message}", Color.LightGreen);
				}
			}
			return ExecuteResult.FromSuccess();
		}

		[Command("killall")]
		public IResult KillAll()
		{
			foreach (TSPlayer p in TShock.Players)
			{
				if (p.Name == Context.Player.Name)
					continue;
				p.KillPlayer();
			}
			return Success("You killed everyone. They do something to piss you off?");
		}

		[Command("send", "rawbc")]
		[RequirePermission("rawbc")]
		public IResult RawBroadcast([Remainder] string message) => Announce(message, Color.White);

		[Command("freezetime", "ft")]
		[RequirePermission("freezetime")]
		public IResult FreezeTime(string time = "")
		{
			if (Foundations.core.timeFreeze.enabled == true)
			{
				Foundations.core.timeFreeze.Stop();
				return Announce($"Time has been un-frozen by {Context.Name}.", Color.LightBlue);
			}

			switch (time)
			{
				default:
				case "":
					Foundations.core.timeFreeze.Start(TimeFreeze.TimeOfDay.Custom);
					break;
				case "day":
					Foundations.core.timeFreeze.Start(TimeFreeze.TimeOfDay.Day);
					break;
				case "noon":
					Foundations.core.timeFreeze.Start(TimeFreeze.TimeOfDay.Noon);
					break;
				case "night":
					Foundations.core.timeFreeze.Start(TimeFreeze.TimeOfDay.Night);
					break;
				case "midnight":
					Foundations.core.timeFreeze.Start(TimeFreeze.TimeOfDay.Midnight);
					break;
			}

			return Announce($"Time has been frozen by {Context.Name}.", Color.LightYellow);

		}
	}
}
