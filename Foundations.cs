using CSF;
using CSF.TShock;
using Terraria;
using TerrariaApi.Server;
using Foundations.Api;

namespace Foundations
{
    [ApiVersion(1,0)]
    public class Foundations : TerrariaPlugin
    {
        #region Plugin Metadata
        public override string Name => "Foundations";
        public override string Description => "An essential plugin for TShock servers.";
        public override string Author => "Average";
        public override Version Version => new Version(1, 0);
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

        public async override void Initialize()
        {
            core = new FoundationsApi();

            var e = TShockAPI.Commands.ChatCommands.FirstOrDefault(x=>x.Name=="home", null);
            if(e != null)
                TShockAPI.Commands.ChatCommands.Remove(e);


            // build command modules
            await _fx.BuildModulesAsync(typeof(Foundations).Assembly);
        }
    }
}