using Guardian.Utilities;

namespace Guardian.UI.Toasts
{
	internal class Toast
	{
		public string Title;

		public string Message;

		public float TimeToLive;

		public long Time;

		public string Timestamp;

		public Toast(string title, string message, float timeToLive)
		{
			Title = title;
			Message = message;
			TimeToLive = timeToLive;
			Time = GameHelper.CurrentTimeMillis();
			Timestamp = GameHelper.Epoch.AddMilliseconds(Time).ToLocalTime().ToString("HH:mm:ss");
		}
	}
}
