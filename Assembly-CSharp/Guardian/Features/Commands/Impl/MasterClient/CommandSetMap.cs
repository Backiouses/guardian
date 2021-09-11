﻿using Guardian.Utilities;

namespace Guardian.Features.Commands.Impl.MasterClient
{
    class CommandSetMap : Command
    {
        public CommandSetMap() : base("setmap", new string[] { "map" }, "<name>", true) { }

        public override void Execute(InRoomChat irc, string[] args)
        {
            if (!Mod.IsMultiMap)
            {
                irc.AddLine("This is not a Multi-Map room!".AsColor("FF0000"));
                return;
            }
            if (args.Length > 0)
            {
                LevelInfo levelInfo = LevelInfo.GetInfo(string.Join(" ", args));
                if (levelInfo != null)
                {
                    PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
                    {
                        { "Map", levelInfo.Name }
                    });

                    FengGameManagerMKII.Instance.RestartGame();

                    GameHelper.Broadcast($"The map in play is now {levelInfo.Name}!");
                }
            }
            else
            {
                irc.AddLine("Available Maps:".AsColor("AAFF00"));

                foreach (LevelInfo level in LevelInfo.Levels)
                {
                    irc.AddLine("> ".AsColor("00FF00").AsBold() + level.Name);
                }
            }
        }
    }
}
