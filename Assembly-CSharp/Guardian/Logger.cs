using Guardian.Utilities;
using UnityEngine;

namespace Guardian
{
	internal class Logger
	{
		public class Entry
		{
			public string Text;

			public long Time;

			public string Timestamp;

			public Entry(string text)
			{
				Text = text;
				Time = GameHelper.CurrentTimeMillis();
				Timestamp = GameHelper.Epoch.AddMilliseconds(Time).ToLocalTime().ToString("HH:mm:ss");
			}

			public override string ToString()
			{
				return Text;
			}
		}

		public SynchronizedList<Entry> Entries = new SynchronizedList<Entry>();

		public Vector2 ScrollPosition = GameHelper.ScrollBottom;

		private void Log(string message)
		{
			message = GuardianClient.BlacklistedTagsPattern.Replace(message, string.Empty);
			if (message.Length > 0)
			{
				Entries.Add(new Entry(message));
				if (Entries.Count > GuardianClient.Properties.MaxLogLines.Value)
				{
					Entries.RemoveAt(0);
				}
				ScrollPosition = GameHelper.ScrollBottom;
			}
		}

		public void Info(string message)
		{
			Log("* ".AsColor("AAAAAA") + message);
		}

		public void Warn(string message)
		{
			Log("* ".AsColor("FFCC00") + message);
		}

		public void Error(string message)
		{
			Log("* ".AsColor("FF0000") + message);
		}

		public void Debug(string message)
		{
			Log("* ".AsColor("00FFFF") + message);
		}
	}
}
