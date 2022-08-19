using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicPlayer
{
	private float micModifier = 1f;

	public bool Processing;

	private Queue<AudioClip> ClipQueue = new Queue<AudioClip>();

	private int Id = -1;

	private float Volume = 1.5f;

	public string Name;

	private bool IsMuted;

	public bool MutedYou;

	public bool changingVolume;

	public bool isMuted => IsMuted;

	public int ID => Id;

	public float volume
	{
		get
		{
			return Volume;
		}
		set
		{
			Volume = value;
		}
	}

	public MicPlayer(int id)
	{
		if (Camera.main.gameObject.GetComponent<AudioSource>() == null)
		{
			Camera.main.gameObject.AddComponent<AudioSource>();
		}
		Id = id;
		PhotonPlayer photonPlayer = PhotonPlayer.Find(id);
		if (photonPlayer.customProperties.ContainsKey("name") && photonPlayer.customProperties["name"] is string)
		{
			Name = ((string)photonPlayer.customProperties["name"]).NGUIToUnity();
		}
		MutedYou = false;
	}

	public void AddClip(AudioClip clip)
	{
		ClipQueue.Enqueue(clip);
		if (!Processing)
		{
			FengGameManagerMKII.Instance.StartCoroutine(PlayClipQueue());
		}
	}

	public IEnumerator PlayClipQueue()
	{
		if (!Processing)
		{
			Processing = true;
		}
		if (ClipQueue.Count > 0)
		{
			AudioClip audioClip = ClipQueue.Dequeue();
			Camera.main.gameObject.GetComponent<AudioSource>().PlayOneShot(audioClip, Volume * MicEF.VolumeMultiplier);
			if (micModifier == 1f && ClipQueue.Count >= 4)
			{
				micModifier = 0.98f;
			}
			else if (micModifier == 0.98f && ClipQueue.Count <= 2)
			{
				micModifier = 1f;
			}
			yield return new WaitForSeconds(audioClip.length * micModifier);
			FengGameManagerMKII.Instance.StartCoroutine(PlayClipQueue());
		}
		else
		{
			Processing = false;
		}
	}

	public void RefreshInformation()
	{
		Processing = false;
		ClipQueue = new Queue<AudioClip>();
	}

	public void Mute(bool enabled)
	{
		if (MutedYou)
		{
			return;
		}
		IsMuted = enabled;
		if (enabled)
		{
			PhotonNetwork.RaiseEvent(173, new byte[1] { 254 }, sendReliable: true, new RaiseEventOptions
			{
				TargetActors = new int[1] { Id }
			});
			MicEF.MuteList.Add(Id);
			if (MicEF.AdjustableList.Contains(Id))
			{
				MicEF.AdjustableList.Remove(Id);
				MicEF.RecompileSendList();
			}
		}
		else if (MicEF.MuteList.Contains(Id))
		{
			MicEF.MuteList.Remove(Id);
			PhotonNetwork.RaiseEvent(173, new byte[1] { 255 }, sendReliable: true, new RaiseEventOptions
			{
				TargetActors = new int[1] { Id }
			});
			if (!MicEF.AdjustableList.Contains(Id))
			{
				MicEF.AdjustableList.Add(Id);
				MicEF.RecompileSendList();
			}
		}
	}
}
