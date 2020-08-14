﻿using System;

namespace Guardian.Features.Commands.Impl.RC
{
    class CommandResetKD : Command
    {
        public CommandResetKD() : base("resetkd", new string[0], "[all]", false) { }

        public override void Execute(InRoomChat irc, string[] args)
        {
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable()
            {
                { PhotonPlayerProperty.Kills, 0 },
                { PhotonPlayerProperty.Deaths, 0 },
                { PhotonPlayerProperty.MaxDamage, 0 },
                { PhotonPlayerProperty.TotalDamage, 0 }
            };
            if (args.Length > 0)
            {
                if (PhotonNetwork.isMasterClient)
                {
                    if (args[0].Equals("all", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (PhotonPlayer player in PhotonNetwork.playerList)
                        {
                            player.SetCustomProperties(properties);
                        }
                        FengGameManagerMKII.Instance.photonView.RPC("Chat", PhotonTargets.All, "All stats have been reset.".WithColor("ffcc00"), string.Empty);
                    }
                }
                else
                {
                    irc.AddLine("Command requires master client.".WithColor("ff0000"));
                }
            }
            else
            {
                PhotonNetwork.player.SetCustomProperties(properties);
                irc.AddLine("Your stats have been reset.".WithColor("ffcc00"));
            }
        }
    }
}
