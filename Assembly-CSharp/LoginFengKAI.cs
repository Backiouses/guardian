using System.Collections;
using UnityEngine;

public class LoginFengKAI : MonoBehaviour
{
	public static string Name = string.Empty;

	public static string Guild = string.Empty;

	public static string Password = string.Empty;

	public static FengPlayer Player;

	public static LoginState LoginState;

	public string CheckUserURL = "http://fenglee.com/game/aog/login_check.php";

	public string RegisterURL = "http://fenglee.com/game/aog/signup_check.php";

	public string ForgetPasswordURL = "http://fenglee.com/game/aog/forget_password.php";

	public string GetInfoURL = "http://fenglee.com/game/aog/require_user_info.php";

	public string ChangePasswordURL = "http://fenglee.com/game/aog/change_password.php";

	public string ChangeGuildURL = "http://fenglee.com/game/aog/change_guild_name.php";

	public string formText = string.Empty;

	public GameObject output;

	public GameObject output2;

	public PanelLoginGroupManager loginGroup;

	public GameObject panelLogin;

	public GameObject panelForget;

	public GameObject panelRegister;

	public GameObject panelStatus;

	public GameObject panelChangePassword;

	public GameObject panelChangeGUILDNAME;

	private void Start()
	{
		if (Player == null)
		{
			Player = new FengPlayer();
			Player.InitAsGuest();
		}
		if (Name.Length > 0)
		{
			NGUITools.SetActive(panelLogin, state: false);
			NGUITools.SetActive(panelStatus, state: true);
			StartCoroutine(CoGetInfo());
		}
		else
		{
			output.GetComponent<UILabel>().text = "Welcome, " + Player.Name;
		}
	}

	public void Login(string name, string password)
	{
		StartCoroutine(CoLogin(name, password));
	}

	private IEnumerator CoLogin(string name, string password)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("userid", name);
		wWWForm.AddField("password", password);
		wWWForm.AddField("version", UIMainReferences.Version);
		using (WWW www = new WWW(CheckUserURL, wWWForm))
		{
			yield return www;
			ClearCookies();
			if (www.error != null)
			{
				MonoBehaviour.print(www.error);
			}
			output.GetComponent<UILabel>().text = www.text;
			formText = www.text;
		}
		if (formText.Contains("Welcome back") && formText.Contains("(^o^)/~"))
		{
			NGUITools.SetActive(panelLogin, state: false);
			NGUITools.SetActive(panelStatus, state: true);
			Name = name;
			Password = password;
			StartCoroutine(CoGetInfo());
		}
	}

	private IEnumerator CoGetInfo()
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("userid", Name);
		wWWForm.AddField("password", Password);
		using (WWW www = new WWW(GetInfoURL, wWWForm))
		{
			yield return www;
			if (www.error != null)
			{
				MonoBehaviour.print(www.error);
			}
			if (www.text.Contains("Error,please sign in again."))
			{
				NGUITools.SetActive(panelLogin, state: true);
				NGUITools.SetActive(panelStatus, state: false);
				output.GetComponent<UILabel>().text = www.text;
				Name = string.Empty;
				Password = string.Empty;
			}
			else
			{
				string[] array = www.text.Split('|');
				Guild = array[0];
				output2.GetComponent<UILabel>().text = array[1];
				Player.Name = Name;
				Player.Guild = Guild;
			}
		}
	}

	public void Register(string name, string password, string password2, string email)
	{
		StartCoroutine(CoRegister(name, password, password2, email));
	}

	private IEnumerator CoRegister(string name, string password, string password2, string email)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("userid", name);
		wWWForm.AddField("password", password);
		wWWForm.AddField("password2", password2);
		wWWForm.AddField("email", email);
		using (WWW www = new WWW(RegisterURL, wWWForm))
		{
			yield return www;
			if (www.error != null)
			{
				MonoBehaviour.print(www.error);
			}
			else
			{
				output.GetComponent<UILabel>().text = www.text;
				if (www.text.Contains("Final step,to activate your account, please click the link in the activation email"))
				{
					NGUITools.SetActive(panelRegister, state: false);
					NGUITools.SetActive(panelLogin, state: true);
				}
			}
		}
		ClearCookies();
	}

	public void ChangePassword(string oldpassword, string password, string password2)
	{
		if (Name == string.Empty)
		{
			Logout();
			NGUITools.SetActive(panelChangePassword, state: false);
			NGUITools.SetActive(panelLogin, state: true);
			output.GetComponent<UILabel>().text = "Please sign in.";
		}
		else
		{
			StartCoroutine(CoChangePassword(oldpassword, password, password2));
		}
	}

	private IEnumerator CoChangePassword(string oldpassword, string password, string password2)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("userid", Name);
		wWWForm.AddField("old_password", oldpassword);
		wWWForm.AddField("password", password);
		wWWForm.AddField("password2", password2);
		using (WWW www = new WWW(ChangePasswordURL, wWWForm))
		{
			yield return www;
			if (www.error != null)
			{
				MonoBehaviour.print(www.error);
			}
			output.GetComponent<UILabel>().text = www.text;
			if (www.text.Contains("Thanks, your password changed successfully"))
			{
				NGUITools.SetActive(panelChangePassword, state: false);
				NGUITools.SetActive(panelLogin, state: true);
			}
		}
	}

	public void ChangeGuild(string name)
	{
		if (Name == string.Empty)
		{
			Logout();
			NGUITools.SetActive(panelChangeGUILDNAME, state: false);
			NGUITools.SetActive(panelLogin, state: true);
			output.GetComponent<UILabel>().text = "Please sign in.";
		}
		else
		{
			StartCoroutine(CoChangeGuild(name));
		}
	}

	private IEnumerator CoChangeGuild(string name)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("name", Name);
		wWWForm.AddField("guildname", name);
		using (WWW www = new WWW(ChangeGuildURL, wWWForm))
		{
			yield return www;
			if (www.error != null)
			{
				MonoBehaviour.print(www.error);
			}
			output.GetComponent<UILabel>().text = www.text;
			if (www.text.Contains("Guild name set."))
			{
				NGUITools.SetActive(panelChangeGUILDNAME, state: false);
				NGUITools.SetActive(panelStatus, state: true);
				StartCoroutine(CoGetInfo());
			}
		}
	}

	public void ResetPassword(string email)
	{
		StartCoroutine(CoResetPassword(email));
	}

	private IEnumerator CoResetPassword(string email)
	{
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("email", email);
		using (WWW www = new WWW(ForgetPasswordURL, wWWForm))
		{
			yield return www;
			if (www.error != null)
			{
				MonoBehaviour.print(www.error);
			}
			else
			{
				output.GetComponent<UILabel>().text = www.text;
				NGUITools.SetActive(panelForget, state: false);
				NGUITools.SetActive(panelLogin, state: true);
			}
		}
		ClearCookies();
	}

	private void ClearCookies()
	{
		Name = string.Empty;
		Password = string.Empty;
		LoginState = LoginState.LoggedOut;
	}

	public void Logout()
	{
		ClearCookies();
		Player = new FengPlayer();
		Player.InitAsGuest();
		output.GetComponent<UILabel>().text = "Welcome, " + Player.Name;
	}
}
