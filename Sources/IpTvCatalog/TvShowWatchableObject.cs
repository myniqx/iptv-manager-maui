namespace IpTvParser.IpTvCatalog
{
    public class TvShowWatchableObject : WatchableObject
    {
        public int Season;
        public  int Episode;

        public override object GetIcon => MainWindow.TvShowIcon;

        public override string GetTitle => $"Episode {Episode}";

        static public TvShowWatchableObject? CreateIfTvShow(string name)
        {
            int len = name.Length;
            int s = -1;
            int e = -1;
            int remIndex = -1;
            for (int i = 0; i < len; i++)
            {
                char ch = name[i];
                if (s == -1 && ch is 'S' or 's')
                {
                    try
                    {
                        char s0 = i+1 < len ? name[i+1] : '\0';
                        char s1 = i+2 < len ? name[i+2] : '\0';
                        if (char.IsDigit(s0) && char.IsDigit(s1))
                        {
                            s = (s0 - '0') * 10 + (s1 - '0');
                        }
                        else if (char.IsDigit(s0))
                        {
                            s = (s0 - '0');
                        }
                        if (s != -1)
                        {
                            for (int j = i - 1; j >= 0; j--)
                            {
                                if (char.IsWhiteSpace(name[j]) == false)
                                {
                                    remIndex = j + 1;
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    { }
                }

                if (s != -1 && ch is 'E' or 'e')
                {
                    try
                    {
                        char e0 = i+1 < len ? name[i+1] : '\0';
                        char e1 = i+2 < len ? name[i+2] : '\0';
                        if (char.IsDigit(e0) && char.IsDigit(e1))
                        {
                            e = (e0 - '0') * 10 + (e1 - '0');
                        }
                        else if (char.IsDigit(e0))
                        {
                            e = (e0 - '0');
                        }
                    }
                    catch { }
                }
            }
            if (s == -1 || e == -1)
                return null;

            TvShowWatchableObject obj   = new();
            obj.Season = s;
            obj.Episode = e;
            obj.Name = name;
            obj.Group = remIndex > 0 ? name.Remove(remIndex) : name;
            return obj;
        }
    }
}
