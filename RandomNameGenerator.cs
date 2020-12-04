using System;
using System.Collections.Generic;
using System.Linq;

namespace DMRNG
{
    public class RandomNameGenerator
    {
        int _minSourceSize;
        int _maxNameLength;
        System.Random _rnd;

        Dictionary<string, Dictionary<string, double>> _table;

        public RandomNameGenerator(string source, int minSize=100, int maxLength=20)
        {
            _rnd = new System.Random();
            Init(source, minSize, maxLength);
        }

        public RandomNameGenerator(int seed, string source, int minSize=100, int maxLength=20)
        {
            _rnd = new System.Random(seed);
            Init(source, minSize, maxLength);
        }

        void Init(string source, int minSize, int maxLength)
        {
            _minSourceSize = minSize;
            _maxNameLength = maxLength;
            SetupTable(source);
        }

        void SetupTable(string source)
        {
            _table = new Dictionary<string, Dictionary<string, double>>();
            string[] names;
            names = source.Split(null as char[]).Where( x => x != "" ).ToArray();
            if (_minSourceSize > 0 && names.Length < _minSourceSize) {
                foreach (string name in names) {
                    _table["   "][name] = 1;
                }
            }
            else {
                foreach (string name in names) {
                    int pos = 0;
                    string trigram = "   ";
                    string next = name.Substring(0, 3);
                    while(true) {
                        if (!_table.ContainsKey(trigram)) {
                            _table.Add(trigram, new Dictionary<string, double>());
                        }
                        if (_table[trigram].ContainsKey(next)) {
                            _table[trigram][next]++;
                        }
                        else {
                            _table[trigram][next] = 1;
                        }
                        if (pos + 4 > name.Length)
                            break;
                        trigram = name.Substring(pos, 3);
                        next = name.Substring(pos+3, 1);
                        pos++;
                    }

                }
            }
            foreach (Dictionary<string, double> possibilities in _table.Values) {
                if (possibilities.Count > 1) {
                    double total = possibilities.Values.Sum();
                    double cumul = 0;
                    foreach (string possibility in possibilities.Keys) {
                        cumul += possibilities[possibility]/total;
                        possibilities[possibility] = cumul;
                    }
                }
            }
        }

        T Choose<T>(IEnumerable<T> possibilities, Func<T, double> value, T fallback)
        {
            double dice = _rnd.NextDouble();
            foreach (T possibility in possibilities) {
                if (value(possibility) > dice) {
                    return possibility;
                }
            }
            return fallback;
        }

        public string Next(int maxNameLength=0)
        {
            if (maxNameLength == 0)
                maxNameLength = _maxNameLength;
            Dictionary<string, double> possibilities;
            string trigram = "   ";
            string name = "";
            while (_table.TryGetValue(trigram, out possibilities)) {
                string next = Choose(possibilities, kvp => kvp.Value, new KeyValuePair<string, double>()).Key;
                if (name.Length > maxNameLength || next == "") {
                    name = "";
                    trigram = "   ";
                }
                else {
                    name += next;
                    trigram = name.Substring(name.Length - 3, 3);
                }
            }
            return name[0]+name.Substring(1).ToLower();
        }
    }
}
