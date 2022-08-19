using Guardian.Features.Gamemodes.Impl;

namespace Guardian.Features.Gamemodes
{
	internal class GamemodeManager : FeatureManager<Gamemode>
	{
		public Gamemode DefaultMode;

		public Gamemode CurrentMode;

		public override void Load()
		{
			Add(DefaultMode = new Gamemode("Normal", new string[1] { "none" }));
			Add(new CageFight());
			Add(new LastManStanding());
			Add(new TimeBomb());
			CurrentMode = DefaultMode;
			GuardianClient.Logger.Debug($"Registered {Elements.Count} game-modes.");
		}
	}
}
