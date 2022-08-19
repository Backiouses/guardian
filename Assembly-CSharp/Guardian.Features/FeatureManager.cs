using System;
using System.Collections.Generic;

namespace Guardian.Features
{
	internal class FeatureManager<T> where T : Feature
	{
		public List<T> Elements = new List<T>();

		public virtual void Load()
		{
		}

		public virtual void Save()
		{
		}

		public void Add(T element)
		{
			Elements.Add(element);
		}

		public T Find(string name)
		{
			foreach (T element in Elements)
			{
				if (element.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return element;
				}
				string[] aliases = element.Aliases;
				for (int i = 0; i < aliases.Length; i++)
				{
					if (aliases[i].Equals(name, StringComparison.OrdinalIgnoreCase))
					{
						return element;
					}
				}
			}
			return null;
		}
	}
}
