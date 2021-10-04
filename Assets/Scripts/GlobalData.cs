using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unstable.Model;

namespace Unstable
{
    public class GlobalData : MonoBehaviour
    {
        private static GlobalData _instance;
        public static GlobalData Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("GlobalData", typeof(GlobalData));
                    DontDestroyOnLoad(go);
                    _instance = go.GetComponent<GlobalData>();

                    var events = Resources.LoadAll<TextAsset>("Decks/");
                    foreach (var e in events)
                    {
                        _instance._decks.Add(JsonConvert.DeserializeObject<Deck>(e.text));
                    }
                }
                return _instance;
            }
        }

        public bool ToggleDeck(string name)
        {
            if (DecksAllowed.Contains(name))
            {
                DecksAllowed.Remove(name);
                return false;
            }
            DecksAllowed.Add(name);
            return true;
        }

        private List<string> DecksAllowed = new()
        {
            "Basic"
        };
        private List<Deck> _decks = new();

        public List<Deck> GetAllowedDecks()
            => _decks.Where(x => DecksAllowed.Any(d => d.ToUpperInvariant() == x.Name.ToUpperInvariant())).ToList();

        public int GetCardsCount(string deck)
            => _decks.FirstOrDefault(x => x.Name.ToUpperInvariant() == deck.ToUpperInvariant()).Cards.Length;

        public string GetCardsCount()
            => $"Cards enabled: {GetAllowedDecks().Select(x => x.Cards.Length).Sum()} / {_decks.Select(x => x.Cards.Length).Sum()}";
    }
}
