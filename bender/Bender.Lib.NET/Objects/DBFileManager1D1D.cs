using System.Globalization;

namespace Bender.Lib.NET
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

                    if (split1.Length != 2)
                    {
                        throw new NotSupportedException("split1.Length != 2");
                    }

                    if (!double.TryParse(split1[0], CultureInfo.InvariantCulture, out double x))
                    {
                        throw new NotSupportedException("x did not parse");
                    }

                    if (!double.TryParse(split1[1], CultureInfo.InvariantCulture, out double y))
                    {
                        throw new NotSupportedException("y did not parse");
                    }

                    _dict[x] = y;
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
                    File.AppendAllLines(_path, new string[] { x.ToString(CultureInfo.InvariantCulture) + "," + y.ToString(CultureInfo.InvariantCulture) });
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

        public bool ContainsKey(double x)
        {
            lock (_dict)
            {
                return _dict.ContainsKey(x);
            }
        }
    }
}
