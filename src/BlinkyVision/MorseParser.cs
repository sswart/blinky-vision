namespace BlinkyVision
{
    public class MorseParser
    {
        public static string Translate(IEnumerable<FrameInfo> frames)
        {
            var morse = ConvertToMorse(frames);
            var morseLetters = GetLettersInMorse(morse);
            var message = morseLetters.Select(letter => FromMorse.GetValueOrDefault(letter, '_')).ToArray();
            return new string(message);
        }

        private static List<string> GetLettersInMorse(IEnumerable<Morse> morse)
        {
            var currentSet = new List<char>();
            var morseLetters = new List<string>();
            foreach (var character in morse)
            {
                if (character.IsStop)
                {
                    if (currentSet.Count > 0)
                    {
                        morseLetters.Add(new string(currentSet.ToArray()));
                    }
                    currentSet.Clear();
                }
                else if (character.MorseValue.HasValue)
                {
                    currentSet.Add(character.MorseValue.Value);
                }
                else if (!character.MorseValue.HasValue && character.On)
                {
                    break;
                }
            }

            return morseLetters;
        }

        private static IEnumerable<Morse> ConvertToMorse(IEnumerable<FrameInfo> frames)
        {
            var sets = frames!.GroupWhile((prev, next) => prev.Classification == next.Classification).ToArray();

            var possibleFrameCounts = new[] { 8, 9, 10, 11, 12 };
            var start = sets.First(set => possibleFrameCounts.Contains(set.Count()) && set.All(kvp => kvp.Classification == "off"));

            var startIndex = Array.IndexOf(sets, start);

            var parseableSets = sets.Skip(startIndex + 1);

            return parseableSets.Select(set => new Morse(set.Count(), set.First().Classification == "on"));
        }

        private static Dictionary<char, String> ToMorse => new Dictionary<char, String>()
        {
            {'A' , ".-"},
            {'B' , "-..."},
            {'C' , "-.-."},
            {'D' , "-.."},
            {'E' , "."},
            {'F' , "..-."},
            {'G' , "--."},
            {'H' , "...."},
            {'I' , ".."},
            {'J' , ".---"},
            {'K' , "-.-"},
            {'L' , ".-.."},
            {'M' , "--"},
            {'N' , "-."},
            {'O' , "---"},
            {'P' , ".--."},
            {'Q' , "--.-"},
            {'R' , ".-."},
            {'S' , "..."},
            {'T' , "-"},
            {'U' , "..-"},
            {'V' , "...-"},
            {'W' , ".--"},
            {'X' , "-..-"},
            {'Y' , "-.--"},
            {'Z' , "--.."},
            {'0' , "-----"},
            {'1' , ".----"},
            {'2' , "..---"},
            {'3' , "...--"},
            {'4' , "....-"},
            {'5' , "....."},
            {'6' , "-...."},
            {'7' , "--..."},
            {'8' , "---.."},
            {'9' , "----."},
        };
        private static Dictionary<string, char> FromMorse => ToMorse.ToDictionary(v => v.Value, v => v.Key);
    }
}
