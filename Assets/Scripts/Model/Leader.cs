using System.Collections.Generic;

namespace Unstable.Model
{
    public class Leader
    {
        /// <summary>
        /// 3 letters to identify the domain
        /// </summary>
        public string Trigram { set; get; }
        /// <summary>
        /// Name of the domain (medical, engineering, etc...)
        /// </summary>
        public string DomainName { set; get; }
        /// <summary>
        /// Name of the leader representing this domain
        /// </summary>
        public string LeaderName { set; get; }

        /// <summary>
        /// Max sanity the character can have
        /// </summary>
        public int MaxSanity { set; get; }

        public string[] Endings { set; get; }

        /// <summary>
        /// Sentences used for basic conversations
        /// Key: Sanity floor
        /// Value: Sentences
        /// </summary>
        public Dictionary<int, string[]> SentencesConversation { set; get; }

        /// <summary>
        /// Sentences used for events
        /// Key: Sanity floor
        /// Value: Sentences
        /// </summary>
        public Dictionary<int, string[]> SentencesEvent { set; get; }

        /// <summary>
        /// Sentences used for crisis
        /// Key: Sanity floor
        /// Value: Sentences
        /// </summary>
        public Dictionary<int, string[]> SentencesCrisis { set; get; }

        /// <summary>
        /// Cards this class can distribute
        /// Key: Trigram to identify the card
        /// Value: The card itself
        /// </summary>
        public Dictionary<string, Card> Cards { set; get; }
    }
}
