using Discord;

namespace Guardian.Utilities
{
	internal class DiscordRPC
	{
		public static long StartTime;

		private static global::Discord.Discord _Discord;

		public static void Initialize()
		{
			if (_Discord != null)
			{
				return;
			}
			try
			{
				_Discord = new global::Discord.Discord(721934748825550931L, 1uL);
				_Discord.SetLogHook(LogLevel.Debug, delegate(LogLevel logLevel, string message)
				{
					switch (logLevel)
					{
					case LogLevel.Debug:
						GuardianClient.Logger.Debug(message);
						break;
					case LogLevel.Info:
						GuardianClient.Logger.Info(message);
						break;
					case LogLevel.Warn:
						GuardianClient.Logger.Warn(message);
						break;
					case LogLevel.Error:
						GuardianClient.Logger.Error(message);
						break;
					}
				});
				_Discord.GetUserManager().OnCurrentUserUpdate += delegate
				{
					GuardianClient.Logger.Debug("Connected to Discord for Rich Presence.");
				};
			}
			catch
			{
			}
		}

		public static void RunCallbacks()
		{
			if (_Discord == null)
			{
				return;
			}
			try
			{
				_Discord.RunCallbacks();
			}
			catch
			{
			}
		}

		public static void Dispose()
		{
			if (_Discord == null)
			{
				return;
			}
			try
			{
				_Discord.GetActivityManager().ClearActivity(delegate
				{
					_Discord.Dispose();
					_Discord = null;
				});
			}
			catch
			{
			}
		}

		public static void SetPresence(Activity activity)
		{
			Initialize();
			if (_Discord == null || !GuardianClient.Properties.UseRichPresence.Value)
			{
				return;
			}
			try
			{
				activity.Assets = new ActivityAssets
				{
					LargeImage = "main_icon",
					LargeText = "G-Shield by Red"
				};
				activity.Timestamps = new ActivityTimestamps
				{
					Start = StartTime
				};
				_Discord.GetActivityManager().UpdateActivity(activity, delegate
				{
				});
			}
			catch
			{
			}
		}
	}
}
