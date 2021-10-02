namespace Unstable.Model
{
    public class Event
    {
        /// <summary>
        /// Name of the event
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// Description of the event
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// Is the event a crisis or just a normal event
        /// </summary>
        public bool IsCrisis { set; get; }

        /// <summary>
        /// All the choices of the event
        /// </summary>
        public EventChoice[] Choices { set; get; }
    }
}
