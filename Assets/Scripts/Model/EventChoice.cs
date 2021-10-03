using System.Collections.Generic;

namespace Unstable.Model
{
    public class EventChoice
    {
        /// <summary>
        /// Description of the choice (how the event is resolved with this)
        /// </summary>
        public string Description { set; get; }

        /// <summary>
        /// Trigram of the domain associated with this choice
        /// </summary>
        public string TargetTrigram { set; get; }

        /// <summary>
        /// How many sanity point you loose if you don't take this choice
        /// </summary>
        public string Cost { set; get; }

        /// <summary>
        /// What happens if this choice is selected
        /// </summary>
        public Function[] Effects { set; get; }

        /// <summary>
        /// Requirements
        /// Key: ID of the card to use (DomainTrigram_CardTrigram)
        /// Value: Number of card required
        /// </summary>
        public Dictionary<string, string> Requirements { set; get; }
    }
}
