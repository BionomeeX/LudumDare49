using System.Collections.Generic;

namespace Unstable.Model
{
    public class Card
    {
        /// <summary>
        /// Name of the card
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// Description of the card
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// Effects of the card
        /// Key: Function to call
        /// Value: Parameter given to the function
        /// </summary>
        public Dictionary<string, string> Effets { set; get; }
    }
}
