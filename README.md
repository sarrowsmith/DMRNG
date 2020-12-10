# DMRNG
The DragonMail Random Name Generator as a standalone project

## Installing
This project is designed to be cloned as a [submodule](https://git-scm.com/book/en/v2/Git-Tools-Submodules) into your Mono Godot project or Unity project Assets.

If you're not using `git` as a VCS in your project, you can just do an ordinary clone or download and unpack the .zip.

Alternatively, if you're using Unity you can download the Unity package from https://sarrowsmith.itch.io/dmrng, but note that this might not be up-to-date with the source here.

There are additional dependencies.

It will also build a command line application, but that might not work out-of-the-box for you. This depends on having `Mono.Options` available.

## Use

## Notes
It is quite likely that the algorithm will return names which are in the source list. The shorter the list, the more likely this is, and the `minSize` parameter is effectively a way of saying "I know this is going to happen for sources under a certain length, so don't even try to generate new names for those."

Processing is done in C# Unicode strings, which means it *should* work for any alphabetic script. Because the algorithm operates on trigrams, accented characters may be presented as single code points (as in the example data sets) or as combining diacritics. Similarly, I would expect a data source written in an abugida to work reasonably well. I would *not* expect abjads to produce good results, and suspect syllaberies would require very large data source to before generating anything interesting.
