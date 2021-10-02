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
        /// Key: Target ability
        /// Value: Number of unit
        /// </summary>
        public Dictionary<string, int> Effects { set; get; }
    }
}
