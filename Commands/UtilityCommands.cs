using CSF;
using CSF.TShock;
using MongoDB.Driver;
using System.Linq;
using Terraria;
using TShockAPI;

namespace Foundations.Commands
{
    [RequirePermission("foundations.util")]
    public class UtilityCommands : TSModuleBase<TSCommandContext>
    {
        [Command("find")]
        [Description("FILL THIS OUT LATER")]
        public async Task<IResult> FindCmd(string type = "", string search = "", int page = 1)
        {
            switch (type.ToLowerInvariant()) {
                case "-command":
                case "command":

                    List<TShockAPI.Command> cmd = new List<TShockAPI.Command>();

                    await Task.Run(() =>
                    {
                        cmd = TShockAPI.Commands.ChatCommands.FindAll(x => x.Names.Contains(search) || x.Name == search);
                    });

                    List<string> commands = new List<string>();

                    foreach(TShockAPI.Command c in cmd)
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
                    if(itemList.Count == 0)
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
