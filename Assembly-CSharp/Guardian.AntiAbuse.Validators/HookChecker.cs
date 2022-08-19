namespace Guardian.AntiAbuse.Validators
{
	internal class HookChecker
	{
		public static bool IsKillObjectValid(PhotonMessageInfo info)
		{
			if (info == null)
			{
				return true;
			}
			GuardianClient.Logger.Error($"'Bullet.killObject' from #{info.sender.Id}.");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsHookMasterSetValid(Bullet hook, int viewId, PhotonMessageInfo info)
		{
			PhotonView photonView = PhotonView.Find(viewId);
			if (info != null && photonView != null && hook.photonView.ownerId == info.sender.Id && photonView.gameObject.GetComponent<HERO>() != null)
			{
				return true;
			}
			GuardianClient.Logger.Warn("'Bullet.myMasterIs' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			return false;
		}

		public static bool IsHookTieValid(Bullet hook, int viewId, PhotonMessageInfo info)
		{
			PhotonView photonView = PhotonView.Find(viewId);
			if (info != null && photonView != null && hook.photonView.ownerId == info.sender.Id)
			{
				return true;
			}
			GuardianClient.Logger.Warn("'Bullet.tieMeToOBJ' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			return false;
		}

		public static bool IsLaunchValid(PhotonMessageInfo info)
		{
			if (info == null)
			{
				return true;
			}
			GuardianClient.Logger.Error($"'Bullet.netLaunch' from #{info.sender.Id}.");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsPhaseUpdateValid(PhotonMessageInfo info)
		{
			if (info == null)
			{
				return true;
			}
			GuardianClient.Logger.Error($"'Bullet.netUpdatePhase1' from #{info.sender.Id}.");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsLeviSpiralValid(PhotonMessageInfo info)
		{
			if (info == null)
			{
				return true;
			}
			GuardianClient.Logger.Error($"'Bullet.netUpdateLeviSpiral' from #{info.sender.Id}.");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}
	}
}
