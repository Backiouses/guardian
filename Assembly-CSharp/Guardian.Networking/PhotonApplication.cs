namespace Guardian.Networking
{
	internal class PhotonApplication
	{
		public static PhotonApplication AoTTG2 = new PhotonApplication("AoTTG-2", string.Empty);

		public static PhotonApplication Custom = new PhotonApplication("Custom", string.Empty);

		public string Name;

		public string Id;

		public PhotonApplication(string name, string id)
		{
			Name = name;
			Id = id;
		}
	}
}
