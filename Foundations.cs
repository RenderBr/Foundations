using CSF;
using CSF.TShock;
using Foundations.Api;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using static TShockAPI.GetDataHandlers;

namespace Foundations
{
	[ApiVersion(2, 1)]
	public class Foundations : TerrariaPlugin
	{
		#region Plugin Metadata
		public override string Name => "Foundations";
		public override string Description => "An essential plugin for TShock servers.";
		public override string Author => "Average";

		#region Version logic
		public static Version version => new Version(1, 0);
		public override Version Version => version;
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

		public async override void Initialize()
		{
			core = new FoundationsApi();

			var e = TShockAPI.Commands.ChatCommands.FirstOrDefault(x => x.Name == "home", null);
			if (e is not null)
				TShockAPI.Commands.ChatCommands.Remove(e);


			// build command modules
			await _fx.BuildModulesAsync(typeof(Foundations).Assembly);

<<<<<<< Updated upstream
			PlayerHooks.PlayerCommand += OnPlayerCommand;
			GetDataHandlers.Teleport += OnPlayerTeleport;
=======
			TShockAPI.Hooks.PlayerHooks.PlayerCommand += OnPlayerCommand;
			Teleport += OnPlayerTeleport;
>>>>>>> Stashed changes
		}

		private void OnPlayerCommand(PlayerCommandEventArgs e)
		{
			TSPlayer player = e.Player;
#if DEBUG
			Console.WriteLine($"{e.CommandText}");
#endif
			if (e.CommandText == "=")
			{
                return;
            }

            player.SetData<string>("last", e.CommandText);
		}

		private void OnPlayerTeleport(object sender, TeleportEventArgs e)
		{
			TSPlayer player = e.Player;
			if (player == null)
				return;

			core.HasTeleported(player);
		}
	}
}