namespace BlinkyVision
{
    public class Morse
    {
        public Morse(int duration, bool on)
        {
            Duration = duration;
            On = on;
        }

        public bool IsPause => !On && Duration == 2;
        public bool IsStop => !On && Duration > 5;
        public char? MorseValue => On ? Duration == 8 ? '-' : '.' : null;
        public int Duration { get; }
        public bool On { get; }
    }
}