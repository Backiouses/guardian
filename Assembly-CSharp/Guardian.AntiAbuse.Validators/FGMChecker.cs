namespace Guardian.AntiAbuse.Validators
{
	internal class FGMChecker
	{
		public static bool IsPauseStateChangeValid(PhotonMessageInfo info)
		{
			if (info == null || !info.sender.isMasterClient)
			{
				GuardianClient.Logger.Error("'FengGameManagerMKII.pauseRPC' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
				if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
				{
					FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
				}
				return false;
			}
			return true;
		}

		public static bool IsStatusRequestValid(PhotonMessageInfo info)
		{
			if (info != null && !PhotonNetwork.isMasterClient)
			{
				GuardianClient.Logger.Error($"'FengGameManagerMKII.RequireStatus' from #{info.sender.Id}.");
				if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
				{
					FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
				}
				return false;
			}
			return true;
		}

		public static bool IsStatusRefreshValid(PhotonMessageInfo info)
		{
			if (info == null || !info.sender.isMasterClient)
			{
				GuardianClient.Logger.Error("'FengGameManagerMKII.refreshStatus' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
				if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
				{
					FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
				}
				return false;
			}
			return true;
		}

		public static bool IsPVPStatusRefreshValid(PhotonMessageInfo info)
		{
			if (info == null || (!info.sender.isMasterClient && FengGameManagerMKII.Level.Mode != GameMode.PvPCapture))
			{
				GuardianClient.Logger.Error("'FengGameManagerMKII.refreshPVPStatus' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
				if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
				{
					FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
				}
				return false;
			}
			return true;
		}

		public static bool IsAHSSStatusRefreshValid(PhotonMessageInfo info)
		{
			if (info == null || !info.sender.isMasterClient)
			{
				GuardianClient.Logger.Error("'FengGameManagerMKII.refreshPVPStatus_AHSS' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
				if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
				{
					FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
				}
				return false;
			}
			return true;
		}

		public static bool IsTitanKillValid(PhotonMessageInfo info)
		{
			if (info == null)
			{
				return true;
			}
			GuardianClient.Logger.Error($"'FengGameManagerMKII.titanGetKill' from #{info.sender.Id}");
			if (!FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsNetShowDamageValid(PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != 0 && (info == null || (!info.sender.isMasterClient && !info.sender.IsTitan)))
			{
				GuardianClient.Logger.Error("'FengGameManagerMKII.netShowDamage' from #" + ((info == null) ? "?" : info.sender.Id.ToString()));
				if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
				{
					FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
				}
				return false;
			}
			return true;
		}

		public static bool IsKillInfoUpdateValid(bool isKillerTitan, bool isVictimTitan, int damage, PhotonMessageInfo info)
		{
			if (info != null && (info.sender.isMasterClient || info.sender.isLocal || (isKillerTitan && damage == 0) || (isVictimTitan && (damage >= 10 || info.sender.IsTitan)) || (isKillerTitan == isVictimTitan && damage == 0)))
			{
				return true;
			}
			GuardianClient.Logger.Error("'FengGameManagerMKII.updateKillInfo' from #" + ((info == null) ? "?" : info.sender.Id.ToString()));
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsChatContentShowValid(PhotonMessageInfo info)
		{
			if (info == null)
			{
				return true;
			}
			GuardianClient.Logger.Error($"'FengGameManagerMKII.showChatContent' from #{info.sender.Id}");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}
	}
}
