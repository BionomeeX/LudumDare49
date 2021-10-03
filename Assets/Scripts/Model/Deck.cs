namespace Unstable.Model
{
    public class Deck
    {
        public string Name { set; get; }
        public string Description { set; get; }
        public string Author { set; get; }
        public Event[] Cards { set; get; }
    }
}
