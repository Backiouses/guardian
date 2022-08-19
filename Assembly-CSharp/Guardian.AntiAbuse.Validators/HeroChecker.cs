using UnityEngine;

namespace Guardian.AntiAbuse.Validators
{
	internal class HeroChecker
	{
		public static bool IsKillObjectValid(PhotonMessageInfo info)
		{
			if (info == null)
			{
				return true;
			}
			GuardianClient.Logger.Error($"'HERO.killObject' from #{info.sender.Id}.");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsHitDamageShowValid(PhotonMessageInfo info)
		{
			if (info == null)
			{
				return true;
			}
			GuardianClient.Logger.Error($"'HERO.showHitDamage' from #{info.sender.Id}.");
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsErenTitanDeclarationValid(int viewId, PhotonMessageInfo info)
		{
			PhotonView photonView = PhotonView.Find(viewId);
			if (info != null && photonView != null && photonView.ownerId == info.sender.Id && photonView.gameObject.GetComponent<TITAN_EREN>() != null)
			{
				return true;
			}
			GuardianClient.Logger.Warn("'HERO.whoIsMyErenTitan' from #" + ((info == null) ? "?" : info.sender.Id.ToString()) + ".");
			return false;
		}

		public static bool IsCannonSetValid(int viewId, PhotonMessageInfo info)
		{
			PhotonView photonView = PhotonView.Find(viewId);
			if (info != null && photonView != null && photonView.gameObject.GetComponent<Cannon>() != null)
			{
				return true;
			}
			GuardianClient.Logger.Warn("'HERO.SetMyCannon' from #" + ((info == null) ? "?" : info.sender.Id.ToString()));
			return false;
		}

		public static bool IsSkinLoadValid(HERO hero, PhotonMessageInfo info)
		{
			if (info != null && hero.photonView.ownerId == info.sender.Id)
			{
				return true;
			}
			GuardianClient.Logger.Error("'HERO.loadskinRPC' from #" + ((info == null) ? "?" : info.sender.Id.ToString()));
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsGrabValid(int viewId, PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer)
			{
				return true;
			}
			PhotonView photonView = PhotonView.Find(viewId);
			if (info != null && photonView != null)
			{
				GameObject gameObject = photonView.gameObject;
				if (gameObject != null)
				{
					TITAN component = gameObject.GetComponent<TITAN>();
					if (component != null && component.photonView.ownerId == info.sender.Id)
					{
						return true;
					}
					FEMALE_TITAN component2 = gameObject.GetComponent<FEMALE_TITAN>();
					if (component2 != null && component2.photonView.ownerId == info.sender.Id)
					{
						return true;
					}
				}
			}
			GuardianClient.Logger.Error("'HERO.netGrabbed' from #" + ((info == null) ? "?" : info.sender.Id.ToString()));
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsBlowAwayValid(PhotonMessageInfo info)
		{
			if (IN_GAME_MAIN_CAMERA.Gametype != GameType.Multiplayer || info == null || info.sender.isMasterClient || info.sender.isLocal || info.sender.IsTitan)
			{
				return true;
			}
			GuardianClient.Logger.Error("'HERO.blowAway' from #" + ((info == null) ? "?" : info.sender.Id.ToString()));
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsAnimationPlayValid(HERO hero, PhotonMessageInfo info)
		{
			if (info == null || hero.photonView.ownerId == info.sender.Id || info.sender.isMasterClient || info.sender.IsTitan)
			{
				return true;
			}
			GuardianClient.Logger.Error("'HERO.netPlayAnimation' from #" + ((info == null) ? "?" : info.sender.Id.ToString()));
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsAnimationSeekedPlayValid(HERO hero, PhotonMessageInfo info)
		{
			if (info != null && hero.photonView.ownerId == info.sender.Id)
			{
				return true;
			}
			GuardianClient.Logger.Error("'HERO.netPlayAnimationAt' from #" + ((info == null) ? "?" : info.sender.Id.ToString()));
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsCrossFadeValid(HERO hero, PhotonMessageInfo info)
		{
			if (info != null && hero.photonView.ownerId == info.sender.Id)
			{
				return true;
			}
			GuardianClient.Logger.Error("'HERO.netCrossFade' from #" + ((info == null) ? "?" : info.sender.Id.ToString()));
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsAnimationPauseValid(HERO hero, PhotonMessageInfo info)
		{
			if (info != null && hero.photonView.ownerId == info.sender.Id)
			{
				return true;
			}
			GuardianClient.Logger.Error("'HERO.netPauseAnimation' from #" + ((info == null) ? "?" : info.sender.Id.ToString()));
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}

		public static bool IsAnimationResumeValid(HERO hero, PhotonMessageInfo info)
		{
			if (info != null && hero.photonView.ownerId == info.sender.Id)
			{
				return true;
			}
			GuardianClient.Logger.Error("'HERO.netContinueAnimation' from #" + ((info == null) ? "?" : info.sender.Id.ToString()));
			if (info.sender != null && !FengGameManagerMKII.IgnoreList.Contains(info.sender.Id))
			{
				FengGameManagerMKII.IgnoreList.Add(info.sender.Id);
			}
			return false;
		}
	}
}
