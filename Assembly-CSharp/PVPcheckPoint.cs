using System.Collections;
using Photon;
using UnityEngine;

public class PVPcheckPoint : Photon.MonoBehaviour
{
	private FengGameManagerMKII fengGame;

	public int id;

	public GameObject humanCyc;

	public GameObject titanCyc;

	public CheckPointState state;

	private bool titanOn;

	private bool playerOn;

	public float humanPt;

	public float titanPt;

	public float humanPtMax = 40f;

	public float titanPtMax = 40f;

	public int normalTitanRate = 70;

	private bool annie;

	private float spawnTitanTimer;

	public float titanInterval = 30f;

	public float size = 1f;

	private float hitTestR = 15f;

	private GameObject supply;

	private float syncTimer;

	private float syncInterval = 0.6f;

	private float getPtsTimer;

	private float getPtsInterval = 20f;

	public GameObject[] chkPtNextArr;

	public GameObject[] chkPtPreviousArr;

	public static ArrayList chkPts;

	public bool hasAnnie;

	public bool isBase;

	public GameObject chkPtNext
	{
		get
		{
			if (chkPtNextArr.Length == 0)
			{
				return null;
			}
			return chkPtNextArr[Random.Range(0, chkPtNextArr.Length)];
		}
	}

	public GameObject chkPtPrevious
	{
		get
		{
			if (chkPtPreviousArr.Length == 0)
			{
				return null;
			}
			return chkPtPreviousArr[Random.Range(0, chkPtPreviousArr.Length)];
		}
	}

	private void Start()
	{
		fengGame = GameObject.Find("MultiplayerManager").GetComponent<FengGameManagerMKII>();
		if (IN_GAME_MAIN_CAMERA.Gametype == GameType.Singleplayer)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		if (IN_GAME_MAIN_CAMERA.Gamemode != GameMode.PvPCapture)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		chkPts.Add(this);
		IComparer comparer = new PVPCheckPointComparer();
		chkPts.Sort(comparer);
		if (humanPt == humanPtMax)
		{
			state = CheckPointState.Human;
			if (base.photonView.isMine && FengGameManagerMKII.Level.Map != "The City I")
			{
				Vector3 position = base.transform.position;
				Vector3 up = Vector3.up;
				supply = PhotonNetwork.Instantiate("aot_supply", position - up * (base.transform.position.y - GetHeight(base.transform.position)), base.transform.rotation, 0);
			}
		}
		else if (base.photonView.isMine && !hasAnnie)
		{
			if (Random.Range(0, 100) < 50)
			{
				int num = Random.Range(1, 2);
				for (int i = 0; i < num; i++)
				{
					NewTitan();
				}
			}
			if (isBase)
			{
				NewTitan();
			}
		}
		if (titanPt == titanPtMax)
		{
			state = CheckPointState.Titan;
		}
		hitTestR = 15f * size;
		base.transform.localScale = new Vector3(size, size, size);
	}

	private void NewTitan()
	{
		int rate = normalTitanRate;
		Vector3 position = base.transform.position;
		Vector3 up = Vector3.up;
		Vector3 position2 = base.transform.position;
		GameObject gameObject = fengGame.SpawnTitan(rate, position - up * (position2.y - GetHeight(base.transform.position)), base.transform.rotation);
		if (FengGameManagerMKII.Level.Map == "The City I")
		{
			gameObject.GetComponent<TITAN>().chaseDistance = 120f;
		}
		else
		{
			gameObject.GetComponent<TITAN>().chaseDistance = 200f;
		}
		gameObject.GetComponent<TITAN>().PVPfromCheckPt = this;
	}

	private float GetHeight(Vector3 pt)
	{
		if (Physics.Raycast(layerMask: ((LayerMask)(1 << LayerMask.NameToLayer("Ground"))).value, origin: pt, direction: -Vector3.up, hitInfo: out var hitInfo, distance: 1000f))
		{
			return hitInfo.point.y;
		}
		return 0f;
	}

	private void Update()
	{
		float num = humanPt / humanPtMax;
		float num2 = titanPt / titanPtMax;
		humanCyc.transform.localScale = new Vector3(num, num, 1f);
		titanCyc.transform.localScale = new Vector3(num2, num2, 1f);
		syncTimer += Time.deltaTime;
		if (!base.photonView.isMine)
		{
			if (syncTimer > syncInterval)
			{
				syncTimer = 0f;
				CheckIfBeingCaptured();
			}
			return;
		}
		float num3 = humanPt;
		float num4 = titanPt;
		switch (state)
		{
		case CheckPointState.None:
			if (playerOn && !titanOn)
			{
				humanGetsPoint();
				titanLosePoint();
			}
			else if (titanOn && !playerOn)
			{
				titanGetsPoint();
				humanLosePoint();
			}
			else
			{
				humanLosePoint();
				titanLosePoint();
			}
			break;
		case CheckPointState.Human:
			if (titanOn && !playerOn)
			{
				titanGetsPoint();
			}
			else
			{
				titanLosePoint();
			}
			getPtsTimer += Time.deltaTime;
			if (getPtsTimer > getPtsInterval)
			{
				getPtsTimer = 0f;
				if (!isBase)
				{
					fengGame.PVPhumanScore++;
				}
				fengGame.CheckPvPPoints();
			}
			break;
		case CheckPointState.Titan:
			if (playerOn && !titanOn)
			{
				humanGetsPoint();
			}
			else
			{
				humanLosePoint();
			}
			getPtsTimer += Time.deltaTime;
			if (getPtsTimer > getPtsInterval)
			{
				getPtsTimer = 0f;
				if (!isBase)
				{
					fengGame.PVPtitanScore++;
				}
				fengGame.CheckPvPPoints();
			}
			spawnTitanTimer += Time.deltaTime;
			if (!(spawnTitanTimer > titanInterval))
			{
				break;
			}
			spawnTitanTimer = 0f;
			if (FengGameManagerMKII.Level.Map == "The City I")
			{
				if (FengGameManagerMKII.Instance.AllTitans.Count < 12)
				{
					NewTitan();
				}
			}
			else if (FengGameManagerMKII.Instance.AllTitans.Count < 20)
			{
				NewTitan();
			}
			break;
		}
		if (syncTimer > syncInterval)
		{
			syncTimer = 0f;
			CheckIfBeingCaptured();
			if (num3 != humanPt)
			{
				base.photonView.RPC("changeHumanPt", PhotonTargets.Others, humanPt);
			}
			if (num4 != titanPt)
			{
				base.photonView.RPC("changeTitanPt", PhotonTargets.Others, titanPt);
			}
		}
	}

	private void CheckIfBeingCaptured()
	{
		playerOn = false;
		titanOn = false;
		foreach (HERO hero in FengGameManagerMKII.Instance.Heroes)
		{
			if (!(Vector3.Distance(hero.transform.position, base.transform.position) < hitTestR) || hero.hasDied)
			{
				continue;
			}
			playerOn = true;
			if (state == CheckPointState.Human && hero.photonView.isMine)
			{
				if (fengGame.checkpoint != base.gameObject)
				{
					fengGame.checkpoint = base.gameObject;
					InRoomChat.Instance.AddLine(("Respawn point changed to Point #" + id).AsColor("AAFF00"));
				}
				break;
			}
		}
		foreach (TITAN titan in FengGameManagerMKII.Instance.Titans)
		{
			if (!(Vector3.Distance(titan.transform.position, base.transform.position) < hitTestR + 5f) || titan.hasDie)
			{
				continue;
			}
			titanOn = true;
			if (state == CheckPointState.Titan && titan.photonView.isMine && titan.nonAI)
			{
				if (fengGame.checkpoint != base.gameObject)
				{
					fengGame.checkpoint = base.gameObject;
					InRoomChat.Instance.AddLine(("Respawn point changed to Point #" + id).AsColor("AAFF00"));
				}
				break;
			}
		}
	}

	private void humanGetsPoint()
	{
		if (humanPt >= humanPtMax)
		{
			humanPt = humanPtMax;
			titanPt = 0f;
			state = CheckPointState.Human;
			base.photonView.RPC("changeState", PhotonTargets.AllBuffered, 1);
			if (FengGameManagerMKII.Level.Map != "The City I")
			{
				Vector3 position = base.transform.position;
				Vector3 up = Vector3.up;
				supply = PhotonNetwork.Instantiate("aot_supply", position - up * (base.transform.position.y - GetHeight(base.transform.position)), base.transform.rotation, 0);
			}
			fengGame.PVPhumanScore += 2;
			fengGame.CheckPvPPoints();
			if (HasTeamWon(CheckPointState.Human))
			{
				fengGame.FinishGame();
			}
		}
		else
		{
			humanPt += Time.deltaTime;
		}
	}

	private void titanGetsPoint()
	{
		if (titanPt >= titanPtMax)
		{
			titanPt = titanPtMax;
			humanPt = 0f;
			if (state == CheckPointState.Human && supply != null)
			{
				PhotonNetwork.Destroy(supply);
			}
			state = CheckPointState.Titan;
			base.photonView.RPC("changeState", PhotonTargets.AllBuffered, 2);
			fengGame.PVPtitanScore += 2;
			fengGame.CheckPvPPoints();
			if (HasTeamWon(CheckPointState.Titan))
			{
				fengGame.FinishGame(isLoss: true);
			}
			if (hasAnnie)
			{
				if (!annie)
				{
					annie = true;
					Vector3 position = base.transform.position;
					Vector3 up = Vector3.up;
					PhotonNetwork.Instantiate("FEMALE_TITAN", position - up * (base.transform.position.y - GetHeight(base.transform.position)), base.transform.rotation, 0);
				}
				else
				{
					NewTitan();
				}
			}
			else
			{
				NewTitan();
			}
		}
		else
		{
			titanPt += Time.deltaTime;
		}
	}

	private void titanLosePoint()
	{
		if (!(titanPt > 0f))
		{
			return;
		}
		titanPt -= Time.deltaTime * 3f;
		if (titanPt <= 0f)
		{
			titanPt = 0f;
			if (state != CheckPointState.Human)
			{
				base.photonView.RPC("changeState", PhotonTargets.AllBuffered, 0);
			}
		}
	}

	private void humanLosePoint()
	{
		if (!(humanPt > 0f))
		{
			return;
		}
		humanPt -= Time.deltaTime * 3f;
		if (humanPt <= 0f)
		{
			humanPt = 0f;
			if (state != CheckPointState.Titan)
			{
				base.photonView.RPC("changeState", PhotonTargets.AllBuffered, 0);
			}
		}
	}

	public string GetState()
	{
		switch (state)
		{
		case CheckPointState.Human:
			return "[" + ColorSet.Human + "]H[-]";
		case CheckPointState.Titan:
			return "[" + ColorSet.TitanPlayer + "]T[-]";
		default:
			return "[FFFFFF]_[-]";
		}
	}

	private bool HasTeamWon(CheckPointState state)
	{
		foreach (PVPcheckPoint chkPt in chkPts)
		{
			if (chkPt.state != state)
			{
				return false;
			}
		}
		return true;
	}

	[RPC]
	private void changeHumanPt(float points)
	{
		humanPt = points;
	}

	[RPC]
	private void changeTitanPt(float points)
	{
		titanPt = points;
	}

	[RPC]
	private void changeState(int num)
	{
		state = (CheckPointState)num;
	}
}
