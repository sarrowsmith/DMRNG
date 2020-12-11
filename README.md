# DMRNG
The DragonMail Random Name Generator as a standalone project

## What it does
The aim of the DMRNG is to use a [Markov chain](https://en.wikipedia.org/wiki/Markov_chain) type algorithm to generate names derived from a list of seed names from a particular language/culture. The algorithm procedes character by character, basing each choice on the preceding trigram. Implicit in this is that all generated names start with the same three characters as one of the seed names.

For further details, see the comments in the code.

## Installing
This project is designed to be cloned as a [submodule](https://git-scm.com/book/en/v2/Git-Tools-Submodules) into your Mono Godot project or Unity project Assets.

If you're not using `git` as a VCS in your project, you can just do an ordinary clone or download and unpack the .zip.

Alternatively, if you're using Unity you can download the Unity package from https://sarrowsmith.itch.io/dmrng, but note that this might not be up-to-date with the source here.

There are no additional dependencies.

It will also build a command line application, but that might not work out-of-the-box for you. This depends on having `Mono.Options` available.

## Use
Briefly: To use DMRNG in your own project, create a `DMRNG.RandomNameGenerator` object and call `Next()` (_cf_ `System.Random`) on it to generate a name.

The constructor parameters are:

`int seed`: If given, seed the object's random number generator with this value. Otherwise, it will seed with a random value.

`string source`: The seed names, presented as a whitespace-separated list.

`int maxLength=20`: The maximum length of name to generate.

`int minSize=0`: If given, and the data source contains fewer seed names, just return a random name from the list rather than try to generate a new one.

`Next()` takes one optional parameter:

`maximumNameLength=0`: If given, override the `maxLength` parameter given to the constructor. The special value `-1` means "no maximum".

See `app/Program.cs` (the command line application) for an example.

To run the command line application, make sure you've got a C# development environment installed, and `Mono.Options`, then from the project folder run `dotnet run -- --help` for more details. It should be possible to build an executable which could be run from anywehere, and not require the C# environment, but I've not been able to work out how.

Example data sources can be found in `data/`.

## Notes
It is quite likely that the algorithm will return names which are in the source list. The shorter the list, the more likely this is, and the `minSize` parameter is effectively a way of saying "I know this is going to happen for sources under a certain length, so don't even try to generate new names for those."

Processing is done in C# Unicode strings, which means it *should* work for any alphabetic script. Because the algorithm operates on trigrams, accented characters may be presented as single code points (as in the Ancient Scandanavian example) or as combining diacritics. Similarly, I would expect a data source written in an abugida to work reasonably well. I would *not* expect abjads to produce good results, and suspect syllaberies would require very large data source to before generating anything interesting.

The source data is normalised by lowercasing in the current locale, names returned with an initial capital. Obviously, this has no effect on scripts with no case distinctions.

While developing and using DMRNG, I've been very aware of the problem of cultural appropriation in selecting selecting sets of seed names. I think the five I've included are "fair game" for me, as the sources are all part of my personal cultural heritage.
