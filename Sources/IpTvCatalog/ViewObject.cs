
namespace IpTvParser.IpTvCatalog
{
	public abstract class ViewObject : IComparable<ViewObject>
	{

		string _name = "";
		string _logo = "";
		public string Name
		{
			set
			{
				if (string.Compare(_name, value, StringComparison.OrdinalIgnoreCase) == 0)
					return;
				_name = value;
				//	PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
			}

			get
			{
				return _name;
			}
		}

		public GroupObject? UpperLevel { get; set; } = null;

		public bool isSticky = false;

		public virtual bool isWatchEnabled => false;
		public virtual bool isWatchedEnabled => false;
		public virtual bool isUnmarkEnabled => false;
		public virtual bool isUrlCopiable => false;
		public virtual bool isDownloadable => false;
		public virtual bool isSearchEnabled => false;

		public virtual string Logo
		{
			set
			{
				_logo = value;
				//	PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Logo)));
			}

			get
			{
				return _logo;
			}
		}

		[JsonIgnore]
		double _logoPercent = 0;

		[JsonIgnore]
		public virtual object GetIcon { get; set; } = MainWindow.GroupIcon;

		public virtual object GetListIcon { get; set; } = MainWindow.NullIcon;

		public virtual string GetTitle { get; set; } = "";

		public virtual DateTime AddedDate { get; set; } = DateTime.MinValue;

		public string DateDiff => AddedDate != DateTime.MinValue ? $"{DateTime.Now.Subtract((DateTime)AddedDate).Days} days ago." : "never";

		[JsonIgnore]
		public double LogoPercent
		{
			get { return _logoPercent; }
			set
			{
				Visibility old = LogoVisible;
				_logoPercent = value;
				if (old != LogoVisible)
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogoVisible)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LogoPercent)));
			}
		}

		[JsonIgnore]
		public Visibility LogoVisible
		{
			get => 0 < _logoPercent && _logoPercent < 100 ? Visibility.Visible : Visibility.Hidden;
		}

		protected void propertyChanged(string name)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public int CompareTo(ViewObject? other)
		{
			if (other is null)
				return -1;

			if (isSticky && !other.isSticky)
				return -1;
			if (!isSticky && other.isSticky)
				return 1;

			int dateComparison = -AddedDate.CompareTo(other.AddedDate);
			if (dateComparison != 0)
				return dateComparison;

			return Name.CompareTo(other.Name);
		}

		string? _logoFileName;
		public void notifyImageChanged()
		{
			propertyChanged(nameof(GetImage));
			propertyChanged(nameof(GetStretchValue));
		}

		[JsonIgnore]
		public string LogoFileName
		{
			get
			{
				if (_logoFileName == null)
				{
					using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
					{
						byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(Logo);
						byte[] hashBytes = md5.ComputeHash(inputBytes);
						_logoFileName = Convert.ToHexString(hashBytes) + ".jpg";
					}
				}
				return _logoFileName;
			}
		}

		public string FullPathOfLogo => IpTvConfig.GetImagePath(LogoFileName);

		[JsonIgnore]
		bool triedOnes = false;

		[JsonIgnore]
		public object GetImage
		{
			get
			{
				if (!string.IsNullOrEmpty(Logo))
				{
					if (File.Exists(FullPathOfLogo))
					{
						return FullPathOfLogo;
					}

					if (triedOnes == false)
					{
						MainWindow.coverDownloader.addDownload(this);
						triedOnes = true;
					}
				}
				return GetIcon;
			}
		}

		public Stretch GetStretchValue
		{
			get
			{
				return GetImage == GetIcon ? Stretch.Uniform : Stretch.UniformToFill;
			}
		}

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
