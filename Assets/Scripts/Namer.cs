using System;
using System.Collections.Generic;
using System.Linq;

public static class Namer
{
    private static readonly string[] humanParts = {
        "mi",
        "ke",
        "ryu",
        "si",
        "mo",
        "bu",
        "ki",
        "se",
        "sa",
        "va",
        "wo",
        "ri",
        "ra",
        "olo",
        "hec",
        "tor",
        "sam",
        "my",
        "ken",
        "wel",
        "co",
        "me",
        "to",
        "ton",
        "huy",
        "lik",
        "nat",
        "ti",
        "pop",
        "un",
        "ja",
        "hu",
        "fe",
        "isi",
        "elo",
        "son",
        "ken",
        "chi",
        "won",
        "von",
        "tis"
    };

	private static readonly string[] furfolkParts = {
		"bea",
        "bull",
        "eag",
        "rab",
        "bit",
        "le",
        "fur",
        "fuz",
        "bar",
        "kang",
        "roo",
        "ko",
        "ala",
        "sna",
        "ke",
        "dog",
        "fox",
        "wo",
        "ant",
        "cat",
        "elk",
        "ent",
        "tur",
        "tle",
        "sea",
        "psy",
        "sock",
        "ele",
        "pea",
        "nut",
        "fer"
	};

	private static readonly string[] furfolkEnds = {
        "lf",
        "zy",
        "'bar",
        "-bar",
        "der",
        "veen",
        "ween",
        "ton",
        "mass",
        "son",
        "ly",
        "tor",
        "ler",
        "l",
        "y",
        "'y",
        "'ay",
        "face",
        "stock",
        "ret"
    };

    private static readonly string[] skeletonParts = {
        "one",
        "two",
        "three",
        "four",
        "five",
        "six",
        "seven",
        "eight",
        "nine",
        "zero",
        "zoo",
        "bo",
        "ne",
        "rib",
        "sku",
        "minh",
        "bal",
        "peng",
        "puon",
        "po'dam",
        "do'drou",
        "do'poh",
        "do'ngam",
        "do'sin",
        "po'jit",
        "uno",
        "dos",
        "tres",
        "cuatro",
        "cinco",
        "seis",
        "siete",
        "ocho",
        "nueve",
        "ichi",
        "ni",
        "san",
        "yon",
        "go",
        "roku",
        "nana",
        "hachi",
        "kyuu",
        "ske"
    };

    private static readonly string[] skeletonEnds = {
        "ll",
        "rang",
        "dik",
        "nee",
        "ss",
        "pap",
        "ton"
    };

    private static readonly string[] demonParts = {
		"gra",
        "zhu",
        "wah",
        "ol",
        "ar",
        "iz",
        "muh",
        "val",
        "fha",
        "kof",
        "gru",
        "gre",
        "xur",
        "xam",
        "ize",
        "lho",
        "fax",
        "ugh",
        "fug",
        "fak",
        "um",
        "bra",
        "dom",
        "sub",
        "hell"
	};

    private static string GetRandom(Random rand, IEnumerable<string> parts)
	{
        var arr = parts.ToArray();
        return arr[rand.Next(arr.Length)];
	}

    private static string GetPart(Random rand, int race, bool last)
	{
        switch(race)
		{
            // human
			case 0:
				return GetRandom(rand, humanParts);
			// furfolk
			case 1:
				return last ? GetRandom(rand, furfolkParts.Concat(furfolkEnds)) : GetRandom(rand, furfolkParts.Concat(humanParts));
			// returned
			case 2:
                return last ? GetRandom(rand, skeletonParts.Concat(skeletonEnds)) : GetRandom(rand, skeletonParts);
			// remnant
			case 3:
				return GetRandom(rand, demonParts.Concat(humanParts));
            // subdweller
            case 4:
                return last ? GetRandom(rand, furfolkEnds) : GetRandom(rand, demonParts);
        }

        return string.Empty;
	}

	public static string GenerateName(int race = 0)
    {
        var rand = new Random();
        var parts = 2 + rand.Next(2);

        var name = "";

        for (var i = 0; i < parts; i++)
        {
            name += GetPart(rand, race, i == parts - 1);
        }

        name = name.Length > 10 ? name.Substring(0, 10) : name;

        return name.ToUpper();
    }
}
