using System;
using System.Collections;
using System.IO;
using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl.Debug
{
	internal class CommandLogProperties : Command
	{
		private string SaveDir = GuardianClient.RootDir + "\\Properties";

		public CommandLogProperties()
			: base("logpr", new string[0], "<id>", masterClient: false)
		{
		}

		public override void Execute(InRoomChat irc, string[] args)
		{
			if (args.Length < 1 || !int.TryParse(args[0], out var result))
			{
				return;
			}
			PhotonPlayer photonPlayer = PhotonPlayer.Find(result);
			if (photonPlayer == null)
			{
				return;
			}
			string text = string.Empty;
			foreach (DictionaryEntry customProperty in photonPlayer.customProperties)
			{
				text = $"{text}({customProperty.Value.GetType().Name}) {customProperty.Key}={customProperty.Value}{Environment.NewLine}";
			}
			GameHelper.TryCreateFile(SaveDir, directory: true);
			File.WriteAllText($"{SaveDir}\\Properties_{result}.txt", text);
			irc.AddLine($"Logged properties of {result} to 'Properties\\Properties_{result}.txt'.");
		}
	}
}
