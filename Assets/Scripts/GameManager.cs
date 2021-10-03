using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Unstable.Model;
using Unstable.SO;
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
        /// Effects
        /// Key: Trigram
        /// Value: Full name
        /// </summary>
        private Dictionary<string, string> _effects;

        [SerializeField]
        private Image _panelLights;

        [SerializeField]
        private FactionInfo[] _factions;
        [SerializeField]
        private FactionInfo _neutralFaction;

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

        [SerializeField]
        private GameInfo _info;

        public string GetEffect(string trigram)
            => _effects[trigram];

        private FactionInfo GetFactionInfo(string trigram)
        {
            var faction = _factions.FirstOrDefault(x => x.Trigram == trigram);
            return faction ?? _neutralFaction;
        }

        private void Start()
        {
            // Load JSON objects
            _leaders = JsonConvert.DeserializeObject<List<Leader>>(Resources.Load<TextAsset>("Leaders").text);
            var events = JsonConvert.DeserializeObject<Model.Event[]>(Resources.Load<TextAsset>("Events").text);
            _effects = JsonConvert.DeserializeObject<Dictionary<string, string>>(Resources.Load<TextAsset>("Effects").text);

            // Make sure everything is init
            Assert.IsNotNull(_leaders, "Leaders info failed to load");
            Assert.IsNotNull(events, "Events info failed to load");
            Assert.IsTrue(_leaders.Count > 0, "No leader was found");
            Assert.IsTrue(events.Length > 0, "No event was found");

            // Split events between the standards and crisis ones
            _standardEvents = events.Where(x => !x.IsCrisis).ToList();
            _crisisEvents = events.Where(x => x.IsCrisis).ToList();

            // Make sure things aren't active on game start
            _panelLights.gameObject.SetActive(false);
            _eventLoader.UnLoad();
        }

        private const float _lightOffset = .005f;
        private float _lightObjective = 0.5f;
        private void FixedUpdate()
        {
            var oldVal = _panelLights.color.a;
            _panelLights.color = new Color(
                _panelLights.color.r,
                _panelLights.color.g,
                _panelLights.color.b,
                _panelLights.color.a + (_panelLights.color.a > _lightObjective ? -_lightOffset : _lightOffset)
            );

            if ((oldVal < _lightObjective && _lightObjective < _panelLights.color.a) ||
                (oldVal > _lightObjective && _lightObjective > _panelLights.color.a))
            {
                _lightObjective = Random.Range(.5f, 1f);
            }
        }

        private Model.Card _staffCard = new()
        {
            Name = "Staff"
        };

        public void NextEvent()
        {
            var isCrisis = _numberOfRoundsWithoutCrisis > _info.MinTurnBeforeCrisis;

            if (isCrisis)
            {
                _eventLoader.Load(_crisisEvents[Random.Range(0, _crisisEvents.Count)]);
                _numberOfRoundsWithoutCrisis = 0;
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

                if (Random.value < _info.StaffMemberChance) // We get "normal" unit instead of specialized one
                {
                    var unit = CreateEventChoice(null, ("NEU", _staffCard), _info.StaffCount);
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
            _panelLights.gameObject.SetActive(true);
        }

        public void EndEvent()
        {
            _panelLights.gameObject.SetActive(false);
            _eventLoader.UnLoad();
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
                        Argument = $"{leader?.Trigram ?? "null"} {card.Item1} {count}"
                    }
                }
            };
        }

        public Model.Card GetCard(string leaderTrigram, string cardTrigram)
        {
            if (leaderTrigram == null)
            {
                return _staffCard;
            }
            return _leaders.Where(x => x.Trigram == leaderTrigram.ToUpperInvariant()).ElementAt(0).Cards[cardTrigram.ToUpperInvariant()];
        }

        public void AddCard(Model.Card card, string leaderTrigram)
        {
            var cardGo = Instantiate(_cardPrefab, _hand);
            var cardIns = cardGo.GetComponent<UI.Card>();
            cardIns.Init(card, GetFactionInfo(leaderTrigram));
            _cards.Add(cardIns);

            var cardSize = ((RectTransform)_cardPrefab.transform).sizeDelta.x / 1.5f;
            var half = _cards.Count / 2f;
            for (int i = 0; i < _cards.Count; i++)
            {
                _cards[i].SetTarget(Vector3.right * (i - half) * cardSize + Vector3.right * (cardSize / 1.5f));
            }
        }
    }
}
