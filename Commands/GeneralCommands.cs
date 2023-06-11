﻿using CSF;
using CSF.TShock;
using Foundations.Models;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using TShockAPI;

namespace Foundations.Commands
{
	[RequirePermission("foundations.general")]
	public class GeneralCommands : TSModuleBase<TSCommandContext>
	{
		[Command("foundations", "essentials")]
		public IResult PluginInfo() => Respond("This server is running Foundations version " + Foundations.GetVersion().ToString(), Color.LightGreen);

		[Command("more")]
		[RequirePermission("more")]
		public IResult More()
		{
			Item item = Context.Player.SelectedItem;
			if (item.stack == item.maxStack) return Success("This item is already maxed out!");

			Item temp = Context.Player.SelectedItem;
			temp.stack = temp.maxStack - item.stack;
			Context.Player.GiveItem(temp.type, temp.stack, temp.prefix);
			return Success($"You have been given {temp.stack} more!");
		}

		[Command("pvp")]
		[RequirePermission("pvp")]
		public IResult PvP()
		{
			var e = Context.Player;

			e.TPlayer.hostile = !e.TPlayer.hostile;
			string hostile = Language.GetTextValue(e.TPlayer.hostile ? "LegacyMultiplayer.11" : "LegacyMultiplayer.12", e.Name);
			TSPlayer.All.SendData(PacketTypes.TogglePvp, "", e.Index);
			TSPlayer.All.SendMessage(hostile, Main.teamColor[e.Team]);
			return ExecuteResult.FromSuccess();
		}


		//shortcut for do last command
		[Command("=", "pre", "last")]
		public IResult Pre()
		{
			string LastCommand = Context.Player.GetData<string>("last");
			TShockAPI.Commands.HandleCommand(Context.Player, LastCommand);
			return Success("You repeated the last command!");
		}

		//staff online command
		[Command("staff", "staffonline", "admins", "onlinestaff", "allstaff")]
		public IResult Staff()
		{
			List<StaffMember> staff = new List<StaffMember>();

			foreach (TSPlayer player in TShock.Players)
			{
				if (player.HasPermission("foundations.staff"))
				{
					staff.Add(new StaffMember(player, player.Group));
				}
			}

			Info("Staff members online");

			foreach (StaffMember s in staff)
			{
				Respond(s.Player.Group.Prefix + " " + s.Player.Name, s.Player.Group.ChatColor);
			}
			return ExecuteResult.FromSuccess();
		}

	}
}
