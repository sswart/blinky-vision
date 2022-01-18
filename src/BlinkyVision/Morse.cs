namespace BlinkyVision
{
    public class Morse
    {
        public Morse(int duration, bool on)
        {
            Duration = duration;
            On = on;
        }

        public bool IsPause => !On && Duration.IsBetween(2, 4);
        public bool IsStop => !On && Duration > 4;
        public char? MorseValue => On ? Duration.IsBetween(6, 10) ? '-' : '.' : null;
        public int Duration { get; }
        public bool On { get; }
    }
}