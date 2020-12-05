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

        /**
         * The default maxLength and minSize parameters are experimentally
         * reasonable based on the included sample source data. Your own
         * source data, and name length requirements, may well require
         * different values.
         */
        public RandomNameGenerator(string source, int maxLength=20, int minSize=100)
        {
            _rnd = new System.Random();
            Init(source, minSize, maxLength);
        }

        public RandomNameGenerator(int seed, string source, int maxLength=20, int minSize=100)
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

        /**
         * Build the data structure used by the generator from a whitespace-separated
         * input list of sample names.
         * This table maps a trigram to a dictionary of next letter mapped to probability
         * of that letter being the next character after the trigram. The probability is
         * held as a cumulutive value in no particular order, so one of these inner
         * dictionaries could be visualised as something like:
         *      ------------------------------------------------------
         *      |    A     |          B         |         C          |
         *      +----------+--------------------+--------------------+
         *      |   0.2    |        0.6         |        1.0         |
         *      ------------------------------------------------------
         * To provide an entry point, it also maps "" (three spaces) to one of these
         * inner tables where the keys are not single letters but the initial trigrams of
         * the names in the source data.
         */
        void SetupTable(string source)
        {
            _table = new Dictionary<string, Dictionary<string, double>>();
            string[] names;
            string trigram = "   ";
            names = source.Split(null as char[]).Where( x => x != "" ).ToArray();
            if (_minSourceSize > 0 && names.Length < _minSourceSize) {
                foreach (string name in names) {
                    if (!_table.ContainsKey(trigram)) {
                       _table.Add(trigram, new Dictionary<string, double>());
                    }
                    _table[trigram][name] = 1;
                }
            }
            else {
                foreach (string name in names) {
                    int pos = 0;
                    string next = name.Substring(0, 3);
                    trigram = "   ";
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
                    /* Take a copy of the keys because we're changing the inner table in place */
                    foreach (string possibility in possibilities.Keys.ToList()) {
                        cumul += possibilities[possibility]/total;
                        possibilities[possibility] = cumul;
                    }
                }
            }
        }

        /**
         * Given a list of Ts -- written as generic, in practice an inner table as
         * described above -- and a function to map the T to a cumulative probability,
         * choose a random element weighted by the given probabilities.
         * Given an inner table looking like
         *      ------------------------------------------------------
         *      |    A     |          B         |         C          |
         *      +----------+--------------------+--------------------+
         *      |   0.2    |        0.6         |        1.0         |
         *      ------------------------------------------------------
         *                    ^dice
         * we can see how a dice value uniformly distributed between 0 and 1 is turned
         * into a non-uniform selection from the list.
         */
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

        /**
         * Call Next() to generate a random name.
         * maxNameLength exists because, experimentally, some source data sets can
         * produce ridiculously long names, and potentially get into infinite loops.
         * So if the name being generated goes over this limit, it is abandonned and
         * the generation process restarted. This could, of course, lead to another
         * inifinte loop. The reason for doing this and not just simply breaking out
         * is to ensure that the generated names always have endings which exist in
         * the source data set.
         */
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
