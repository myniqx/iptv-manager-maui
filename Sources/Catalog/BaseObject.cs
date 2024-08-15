namespace iptv_manager_maui.Sources.Catalog
{
	internal class BaseObject
	{
		public string Name { get; set; }

		public List<string> Logos { get; set; } = new();

		public BaseObject(string name, GroupObject? upperLevel)
		{
			Name = name;
			UpperLevel = upperLevel;
		}

		public virtual string ID => Name;

		public GroupObject? UpperLevel { get; set; }

		public bool hasMatch(string[] param)
		{
			int start = 0;
			int end = Name.Length - param.Sum(p => p.Length);

			foreach (var txt in param)
			{
				int paramLen = txt.Length;
				var paramFound = false;
				for (int i = start; i < end; i++)
				{
					var found = true;
					int j = 0;
					for (; j < paramLen; j++)
					{
						char cA = Name[i+j];
						char cB = txt[j];
						if (charMatch(cA, cB) == false)
						{
							found = false;
							break;
						}
					}
					if (found)
					{
						start = i + j;
						paramFound = true;
					}
				}
				if (paramFound == false)
					return false;
			}
			return true;
		}

		public bool hasMatch(string txt)
		{
			int lB = txt.Length;
			int lA = Name.Length-lB;

			for (int i = 0; i < lA; i++)
			{
				bool found = true;
				for (int j = 0; j < lB; j++)
				{
					char cA = Name[i+j];
					char cB = txt[j];
					if (charMatch(cA, cB) == false)
					{
						found = false;
						break;
					}
				}
				if (found)
					return true;
			}
			return false;
		}

		bool charMatch(char a, char b)
		{
			a = char.ToLower(a);
			b = char.ToLower(b);
			if (a == b)
				return true;
			a = toBasic(a);
			b = toBasic(b);
			return a == b;
		}

		char toBasic(char c)
		{
			switch (c)
			{
				case 'ş':
					return 's';
				case 'ç':
					return 'c';
				case 'ı':
					return 'i';
				case 'ğ':
					return 'g';
				case 'ü':
					return 'u';
				case 'ö':
					return 'o';
			}
			return c;
		}

	}
}
