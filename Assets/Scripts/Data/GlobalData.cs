using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Unstable.Model;

namespace Unstable.Data
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

                    _instance.Load();
                }
                return _instance;
            }
        }

        public void Save()
        {
            StringBuilder data = new();
            data.Append(string.Join(",", DecksAllowed));
            data.Append(";");
            data.Append(BestScore);
            data.Append(";");
            data.Append(string.Join(",", EndingsData));
            data.Append(";");
            data.Append(MuteAudio ? 1 : 0);
            data.Append(";");
            data.Append(SkipTutorial ? 1 : 0);

            File.WriteAllText("data.bin", Convert.ToBase64String(Encoding.ASCII.GetBytes(data.ToString())));
        }

        public void Load()
        {
            if (File.Exists("data.bin"))
            {

                var data = Encoding.ASCII.GetString(Convert.FromBase64String(File.ReadAllText("data.bin")));
                var s = data.Split(';');
                DecksAllowed = s[0].Split(',').ToList();
                BestScore = int.Parse(s[1]);
                EndingsData = s[2].Split(',').ToList();
                MuteAudio = s[3] == "1";
                SkipTutorial = s[4] == "1";
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

        public List<string> DecksAllowed { private set; get; } = new()
        {
            "Basic"
        };
        public List<string> EndingsData { private set; get; } = new()
        { };
        private List<Deck> _decks = new();

        public int BestScore = 0;

        public bool SkipTutorial { set; get; }
        public bool MuteAudio { set; get; }

        public List<Deck> GetAllowedDecks()
            => _decks.Where(x => DecksAllowed.Any(d => d.ToUpperInvariant() == x.Name.ToUpperInvariant())).ToList();

        public int GetCardsCount(string deck)
            => _decks.FirstOrDefault(x => x.Name.ToUpperInvariant() == deck.ToUpperInvariant()).Cards.Length;

        public string GetCardsCount()
            => $"Cards enabled: {GetAllowedDecks().Select(x => x.Cards.Length).Sum()} / {_decks.Select(x => x.Cards.Length).Sum()}";

        public void AddEndingData(string trigram)
        {
            if (!EndingsData.Contains(trigram))
            {
                EndingsData.Add(trigram);
            }
        }

        public bool DisplaySanity { set; get; }
    }
}
