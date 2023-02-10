using Auxiliary;
using CSF;
using CSF.TShock;
using Foundations.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Foundations.Commands
{
    [RequirePermission("foundations.tp")]
    public class TeleportationCommands : TSModuleBase<TSCommandContext>
    {
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
                    x.X = (int)(Context.Player.X / 16);
                    x.Y = (int)(Context.Player.Y / 16);
                    x.WorldId = Main.worldID;
                }));

            }
            else
            {
                home.X = (int)(Context.Player.X / 16);
                home.Y = (int)(Context.Player.Y / 16);
                home.WorldId = Main.worldID;
            }

            return Success("Your home has been set!");
        }

    }
}
