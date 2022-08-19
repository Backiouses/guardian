using System.Collections;
using UnityEngine;

public class PanelMultiJoin : MonoBehaviour
{
	public GameObject[] items;

	private int currentPage = 1;

	private int totalPage = 1;

	private float elapsedTime = 2f;

	private ArrayList filterRoom;

	private string filter = string.Empty;

	private UILabel pageLabel;

	private void Start()
	{
		for (int i = 0; i < 10; i++)
		{
			items[i].SetActive(value: true);
			items[i].GetComponentInChildren<UILabel>().text = string.Empty;
			items[i].SetActive(value: false);
		}
	}

	private void ShowList()
	{
		if (filter.Length == 0)
		{
			RoomInfo[] roomList = PhotonNetwork.GetRoomList();
			if (roomList.Length != 0)
			{
				totalPage = (roomList.Length - 1) / 10 + 1;
			}
			else
			{
				totalPage = 1;
			}
		}
		else
		{
			UpdateFilteredRooms();
			if (filterRoom.Count > 0)
			{
				totalPage = (filterRoom.Count - 1) / 10 + 1;
			}
			else
			{
				totalPage = 1;
			}
		}
		if (currentPage < 1)
		{
			currentPage = totalPage;
		}
		if (currentPage > totalPage)
		{
			currentPage = 1;
		}
		ShowServerList();
	}

	private string GetServerDataString(RoomInfo room)
	{
		string[] array = room.name.Split('`');
		if (array.Length < 7)
		{
			return "[FF0000]Invalid Room.";
		}
		string text;
		switch (array[2].ToLower())
		{
		case "normal":
			text = "[00FF00]Normal[-]";
			break;
		case "hard":
			text = "[FFFF00]Hard[-]";
			break;
		case "abnormal":
			text = "[FF0000]Abnormal[-]";
			break;
		default:
			text = array[2];
			break;
		}
		string text2 = text;
		string text3;
		switch (array[4].ToLower())
		{
		case "day":
			text3 = "[FFFF00]Day[-]";
			break;
		case "dawn":
			text3 = "[FF6600]Dawn[-]";
			break;
		case "night":
			text3 = "[000000]Night[-]";
			break;
		default:
			text3 = array[4];
			break;
		}
		string text4 = text3;
		string text5 = string.Empty;
		if (!room.open || (room.maxPlayers > 0 && room.playerCount >= room.maxPlayers))
		{
			text5 = "[FF0000]";
		}
		text5 += $"({room.playerCount}/{room.maxPlayers})";
		string text6 = ((array[5].Length == 0) ? string.Empty : "[FF0000](Pwd)[-] ");
		return text6 + array[0] + "[-] [AAAAAA]:: [FFFFFF]" + array[1] + "[AAAAAA] / " + text2 + " / " + text4 + "[-] " + text5;
	}

	private void ShowServerList()
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		if (roomList.Length != 0)
		{
			if (filter.Length == 0)
			{
				for (int i = 0; i < 10; i++)
				{
					int num = 10 * (currentPage - 1) + i;
					if (num < roomList.Length)
					{
						RoomInfo roomInfo = roomList[num];
						items[i].SetActive(value: true);
						items[i].GetComponentInChildren<UILabel>().text = GetServerDataString(roomInfo);
						items[i].GetComponentInChildren<BTN_Connect_To_Server_On_List>().roomName = roomInfo.name;
					}
					else
					{
						items[i].SetActive(value: false);
					}
				}
			}
			else
			{
				for (int j = 0; j < 10; j++)
				{
					int num2 = 10 * (currentPage - 1) + j;
					if (num2 < filterRoom.Count)
					{
						RoomInfo roomInfo2 = (RoomInfo)filterRoom[num2];
						items[j].SetActive(value: true);
						items[j].GetComponentInChildren<UILabel>().text = GetServerDataString(roomInfo2);
						items[j].GetComponentInChildren<BTN_Connect_To_Server_On_List>().roomName = roomInfo2.name;
					}
					else
					{
						items[j].SetActive(value: false);
					}
				}
			}
		}
		else
		{
			for (int k = 0; k < items.Length; k++)
			{
				items[k].SetActive(value: false);
			}
		}
		if (pageLabel == null)
		{
			pageLabel = GameObject.Find("LabelServerListPage").GetComponent<UILabel>();
		}
		pageLabel.text = currentPage + " / " + totalPage;
	}

	public void PageUp()
	{
		currentPage--;
		if (currentPage < 1)
		{
			currentPage = totalPage;
		}
		ShowServerList();
	}

	public void PageDown()
	{
		currentPage++;
		if (currentPage > totalPage)
		{
			currentPage = 1;
		}
		ShowServerList();
	}

	private void OnEnable()
	{
		currentPage = 1;
		totalPage = 0;
		Refresh();
	}

	public void Refresh()
	{
		ShowList();
	}

	public void ConnectToIndex(int index, string roomName)
	{
		for (int i = 0; i < 10; i++)
		{
			items[i].SetActive(value: false);
		}
		string[] array = roomName.Split('`');
		if (array.Length > 6)
		{
			if (array[5].Length > 0)
			{
				PanelMultiJoinPWD.Password = array[5];
				PanelMultiJoinPWD.RoomName = roomName;
				UIMainReferences component = GameObject.Find("UIRefer").GetComponent<UIMainReferences>();
				NGUITools.SetActive(component.PanelMultiPWD, state: true);
				NGUITools.SetActive(component.panelMultiROOM, state: false);
			}
			else
			{
				PhotonNetwork.JoinRoom(roomName);
			}
		}
	}

	private void OnFilterSubmit(string content)
	{
		filter = content;
		UpdateFilteredRooms();
		ShowList();
	}

	private void UpdateFilteredRooms()
	{
		filterRoom = new ArrayList();
		if (filter.Length <= 0)
		{
			return;
		}
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		foreach (RoomInfo roomInfo in roomList)
		{
			if (roomInfo.name.ToUpper().Contains(filter.ToUpper()))
			{
				filterRoom.Add(roomInfo);
			}
		}
	}

	private void Update()
	{
		elapsedTime += Time.deltaTime;
		if (elapsedTime > 1f)
		{
			elapsedTime = 0f;
			ShowList();
		}
	}
}
