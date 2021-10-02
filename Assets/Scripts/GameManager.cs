using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Unstable.Model;
using Unstable.UI;

namespace Unstable
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { set; get; }

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// List of all the leaders
        /// </summary>
        private List<Leader> _leaders;
        /// <summary>
        /// List of all the events
        /// </summary>
        private List<Model.Event> _standardEvents, _crisisEvents;

        /// <summary>
        /// Get a Leader object from its trigram
        /// </summary>
        public Leader GetLeaderFromTrigram(string trigram)
        {
            if (trigram == null)
            {
                return new()
                {
                    DomainName = "Crew"
                };
            }
            return _leaders.FirstOrDefault(x => x.Trigram == trigram);
        }

        /// <summary>
        /// Number of rounds where we just got "normal" events
        /// </summary>
        private int _numberOfRoundsWithoutCrisis = 0;

        [SerializeField]
        private RectTransform _hand;
        private List<UI.Card> _cards = new();

        [SerializeField]
        private GameObject _cardPrefab;

        [SerializeField]
        private EventLoader _eventLoader;

        private void Start()
        {
            // Load JSON objects
            _leaders = JsonConvert.DeserializeObject<List<Leader>>(Resources.Load<TextAsset>("Leaders").text);
            var events = JsonConvert.DeserializeObject<Model.Event[]>(Resources.Load<TextAsset>("Events").text);

            // Make sure everything is init
            Assert.IsNotNull(_leaders, "Leaders info failed to load");
            Assert.IsNotNull(events, "Events info failed to load");
            Assert.IsTrue(_leaders.Count > 0, "No leader was found");
            Assert.IsTrue(events.Length > 0, "No event was found");

            // Split events between the standards and crisis ones
            _standardEvents = events.Where(x => !x.IsCrisis).ToList();
            _crisisEvents = events.Where(x => x.IsCrisis).ToList();

            // DEBUG
            AddCard(_leaders[0].Cards.First(x => true).Value);
            AddCard(_leaders[0].Cards.First(x => true).Value);

            NextEvent();
        }

        public void NextEvent()
        {
            var isCrisis = _numberOfRoundsWithoutCrisis > 5;

            if (isCrisis)
            {
                _eventLoader.Load(_crisisEvents[Random.Range(0, _crisisEvents.Count)]);
            }
            else
            {
                // Generate a choice between 2 units we can earn
                List<(Leader leader, (string, Model.Card) card)> allUnits = _leaders.SelectMany(leader =>
                {
                    return leader.Cards.Select(x => (leader, (x.Key, x.Value)));
                }).ToList();

                var tmp = allUnits[Random.Range(0, allUnits.Count)];
                var choice1 = CreateEventChoice(tmp.leader, tmp.card, 1);
                allUnits.Remove(tmp);
                tmp = allUnits[Random.Range(0, allUnits.Count)];
                var choice2 = CreateEventChoice(tmp.leader, tmp.card, 1);

                if (Random.value < .75f) // We get "normal" unit instead of specialized one
                {
                    var unit = CreateEventChoice(null, ("NEU", new()
                    {
                        Name = "staff members"
                    }), 10);
                    if (Random.value < .5f)
                    {
                        choice1 = unit;
                    }
                    else
                    {
                        choice2 = unit;
                    }
                }

                _eventLoader.Load(
                    new()
                    {
                        Name = "New personal available",
                        Description = "New personal is available, please choose what you want to focus on",
                        IsCrisis = false,
                        Choices = new EventChoice[] { choice1, choice2 }
                    }
                );
            }

            _numberOfRoundsWithoutCrisis++;
        }

        private EventChoice CreateEventChoice(Leader leader, (string, Model.Card) card, int count)
        {
            return new()
            {
                Description = $"Earn {count} new {card.Item2.Name}",
                TargetTrigram = leader?.Trigram,
                Cost = 0,
                Requirements = new(),
                Effects = new Function[]
                {
                    new()
                    {
                        MethodName = "ADD",
                        Argument = $"{leader?.Trigram ?? "NIL"} {card.Item1} {count}"
                    }
                }
            };
        }

        private void AddCard(Model.Card card)
        {
            var cardGo = Instantiate(_cardPrefab, _hand);
            var cardIns = cardGo.GetComponent<UI.Card>();
            cardIns.Init(card);
            _cards.Add(cardIns);

            var cardSize = 100;
            var half = _cards.Count / 2f;
            for (int i = 0; i < _cards.Count; i++)
            {
                _cards[i].SetTarget(Vector3.right * (i - half) * cardSize + Vector3.right * (cardSize / 2f));
            }
        }
    }
}
