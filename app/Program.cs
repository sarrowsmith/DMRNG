#if UNITY_EDITOR
#else
#if UNITY_STANDALONE
#else
using System;
using System.IO;
using System.Collections.Generic;
using Mono.Options;
using DMRNG;

class Program
{
    static void Main(string[] args)
    {
        int number = 1;
        string source = "";
        int maxLength = 20;
        string seed = null;
        bool showHelp = false;

        var p = new OptionSet ()
        {
            { "n|number=", "the {NUMBER} of names to generate.",
            (int v) => number = v },
            { "s|seed=", "use {SEED} to seed the randomness.",
            v => seed = v },
            { "m|max-length=", "the {MAXIMUM_LENGTH} of name to generate.",
            (int v) => maxLength = v },
            { "h|help",  "show this message and exit",
            v => showHelp = v != null },
        };

        try
        {
            List<string> extra;
            extra = p.Parse (args);
            if (extra.Count > 0)
            {
                source = extra[extra.Count - 1];
            }
        }
        catch (OptionException e)
        {
            Console.WriteLine(e.Message);
            showHelp = true;
        }

        if (showHelp)
        {
            ShowHelp (p);
            return;
        }

        StreamReader reader;
        if ((source == "" || source == "-") && Console.IsInputRedirected)
        {
            reader = new StreamReader(Console.OpenStandardInput(), Console.InputEncoding);
        }
        else
        {
            reader = new StreamReader(source);
        }
        using (reader) {
            source = reader.ReadToEnd();
        }

        RandomNameGenerator rng = seed == null ?
            new RandomNameGenerator(source, maxLength) :
            new RandomNameGenerator(Utils.Hash(seed), source, maxLength);

        for (; number > 0; --number)
        {
            Console.WriteLine(rng.Next());
        }
    }

    static void ShowHelp (OptionSet p)
    {
        string myName = Utils.GetName();
        Console.WriteLine($"Usage: {myName} [OPTIONS]+ <SOURCE|->");
        Console.WriteLine("Generate NUMBER names (default 1) sourced from SOURCE.");
        Console.WriteLine("Give SOURCE as - to use stdin.");
        Console.WriteLine("Options:");
        p.WriteOptionDescriptions(Console.Out);
    }

}
#endif
#endif
