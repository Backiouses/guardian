﻿using System.Collections.Generic;

namespace Guardian.Features.Commands.Impl.RC
{
    class CommandBanlist : Command
    {
        public CommandBanlist() : base("banlist", new string[0], string.Empty, false) { }

        public override void Execute(InRoomChat irc, string[] args)
        {
            irc.AddLine("List of banned players:".AsColor("FFCC00"));

            foreach (int id in FengGameManagerMKII.BanHash.Keys)
            {
                irc.AddLine($"#{id} ({GExtensions.AsString(FengGameManagerMKII.BanHash[id]).ColorParsed()})");
            }
        }
    }
}
