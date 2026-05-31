namespace Bender.Lib.NET.Objects
{
    public sealed class DBFileManager1D1D
    {
        readonly string _path;
        readonly Dictionary<double, double> _dict = new();

        public DBFileManager1D1D(string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            if (File.Exists(path))
            {
                foreach (string line in File.ReadLines(path))
                {
                    string[] split1 = line.Split(',');
                    _dict[double.Parse(split1[0])] = double.Parse(split1[1]);
                }
            }

            _path = path;
        }

        public bool Add(double x, double y)
        {
            lock (_dict)
            {
                if (_dict.ContainsKey(x))
                {
                    return false;
                }
                else
                {
                    File.AppendAllLines(_path, new string[] { x.ToString() + "," + y.ToString() });
                    _dict.Add(x, y);
                    return true;
                }
            }
        }

        public Dictionary<double, double> Snapshot()
        {
            lock (_dict)
            {
                return new Dictionary<double, double>(_dict);
            }
        }
    }
}
