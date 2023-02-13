using CSF;
using CSF.TShock;
using Foundations.Api;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Foundations.Commands
{
    [RequirePermission("foundations.staff")]
    public class StaffCommands : TSModuleBase<TSCommandContext>
    {

        [Command("sudo")]
        [RequirePermission("sudo")]
        public IResult SudoCmd(string player = "", string cmd = "")
        {
            if(player == "")
                return Error("You must specify a player!");

            if (cmd == "")
                return Error("You must specify a command!");

            var plr = TShock.Players.FirstOrDefault(x => x.Name == player);
            TShockAPI.Commands.HandleCommand(plr, cmd);
            return Success("You have sudo-ed the user!");
        }

        [Command("kickall")]
        [RequirePermission("kickall")]
        public IResult KickAll(string reason = "Kicked by staff", string staff = "")
        {
            switch (staff)
            {
                default:
                    foreach (TSPlayer plr in TShock.Players)
                    {
                        if (plr != null)
                        {
                            plr.Kick(reason, true, false);
                        }
                    }
                    return Success("You kicked everyone!");
                case "staff":
                case "-s":
                case "notstaff":
                    foreach(TSPlayer plr in TShock.Players)
                    {
                        if (!plr.HasPermission("foundations.staff"))
                        {
                            plr.Kick(reason, true, false);
                        }
                    }
                    return Success("You kicked all non-staff");
            }
        }

        [Command("freezetime")]
        [RequirePermission("freezetime")]
        public IResult FreezeTime(string time = "")
        {
            if(Foundations.core.timeFreeze.enabled == true)
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
