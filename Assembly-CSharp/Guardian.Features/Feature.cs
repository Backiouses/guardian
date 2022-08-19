namespace Guardian.Features
{
	public class Feature
	{
		public string Name;

		public string[] Aliases;

		public Feature(string name, params string[] aliases)
		{
			Name = name;
			Aliases = aliases;
		}
	}
}
