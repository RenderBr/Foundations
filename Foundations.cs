using CSF;
using CSF.TShock;
using Terraria;
using TerrariaApi.Server;
using Foundations.Api;
using TShock.Hooks;
using TShockAPI.Hooks;
using TShockAPI;

namespace Foundations
{
    [ApiVersion(2,1)]
    public class Foundations : TerrariaPlugin
    {
        #region Plugin Metadata
        public override string Name => "Foundations";
        public override string Description => "An essential plugin for TShock servers.";
        public override string Author => "Average";

        #region Version logic
        public static Version version => new Version(1,0);
        public override Version Version => new Version();
        #endregion

        #endregion
        private readonly TSCommandFramework _fx;

        public static FoundationsApi core;

        public Foundations(Main game) : base(game)
        {
            _fx = new(new CommandConfiguration()
            {
                DoAsynchronousExecution = false
            });
        }
        public static Version GetVersion()
        {
            return version;
        }

        public async override void Initialize()
        {
            core = new FoundationsApi();

            var e = TShockAPI.Commands.ChatCommands.FirstOrDefault(x=>x.Name=="home", null);
            if(e != null)
                TShockAPI.Commands.ChatCommands.Remove(e);


            // build command modules
            await _fx.BuildModulesAsync(typeof(Foundations).Assembly);
            
            TShockAPI.Hooks.PlayerHooks.PlayerCommand += OnPlayerCommand;
        }

        private void OnPlayerCommand(PlayerCommandEventArgs e)
        {
            TSPlayer player = e.Player;

            player.SetData<string>("last", e.CommandText);
        }
    }
}