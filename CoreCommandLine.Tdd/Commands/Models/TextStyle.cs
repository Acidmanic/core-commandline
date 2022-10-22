namespace CoreCommandLine.Tdd.Commands.Models
{
    public class TextStyle
    {
        private readonly string _name;
        private TextStyle(string name)
        {
            _name = name;
        }

        public static TextStyle Regular { get; } = new TextStyle("Regular");

        public static TextStyle Lower { get; } = new TextStyle("Lower");
        
        public static TextStyle Upper { get; } = new TextStyle("Upper");

        public override string ToString()
        {
            return _name;
        }

    }
}