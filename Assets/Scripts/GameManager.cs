using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Unstable.Leaders;
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
        private List<Model.Event> _standardEvents, _crisisEvents, _tutorialEvents;
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

        [SerializeField]
        private Button _nextDayButton;

        [SerializeField]
        private LeaderSpriteInfo[] _leadersImages;

        private Dictionary<string, LeaderSanity> _leaderSanities;

        public static int CostToInt(string value)
        {
            return value.ToUpperInvariant() switch
            {
                "NONE" => 0,
                "LOW" => 2,
                "MED" => 4,
                "HIGH" => 6,
                "EX" => 8,
                _ => throw new System.ArgumentException("Invalid value", nameof(value))
            };
        }

        public static int RequirementToInt(string value)
        {
            return value.ToUpperInvariant() switch
            {
                "LOW" => 3,
                "MED" => 6,
                "HIGH" => 9,
                "EX" => 12,
                _ => throw new System.ArgumentException("Invalid value", nameof(value))
            };
        }

        /// <summary>
        /// Get a Leader object from its trigram
        /// </summary>
        public Leader GetLeaderFromTrigram(string trigram)
        {
            if (trigram == null || trigram == "NEU")
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
            var events = Resources.LoadAll<TextAsset>("Decks/");
            List<Deck> decks = new();
            foreach (var e in events)
            {
                decks.Add(JsonConvert.DeserializeObject<Deck>(e.text));
            }
            _effects = JsonConvert.DeserializeObject<Dictionary<string, string>>(Resources.Load<TextAsset>("Effects").text);
            _tutorialEvents = JsonConvert.DeserializeObject<List<Model.Event>>(Resources.Load<TextAsset>("Tutorial").text);

            // Make sure everything is init
            Assert.IsNotNull(_leaders, "Leaders info failed to load");
            Assert.IsNotNull(decks, "Events info failed to load");
            Assert.IsTrue(_leaders.Count > 0, "No leader was found");
            Assert.IsTrue(decks.Count > 0, "No event was found");

            // Split events between the standards and crisis ones
            _standardEvents = decks.SelectMany(x => x.Cards.Where(c => !c.IsCrisis)).ToList();
            _crisisEvents = decks.SelectMany(x => x.Cards.Where(c => c.IsCrisis)).ToList();

            // Make sure things aren't active on game start
            _panelLights.gameObject.SetActive(false);
            _eventLoader.UnLoad();
            _nextDayButton.gameObject.SetActive(true);

            _leaderSanities = _leadersImages.Select(x =>
            {
                return (x.Trigram, new LeaderSanity()
                {
                    Image = x.Sprite,
                    Sanity = _leaders.FirstOrDefault(f => f.Trigram == x.Trigram).MaxSanity
                });
            }).ToDictionary(x => x.Trigram, x => x.Item2);

            // Disable oxygen leader
            LowerSectorSanity("OXY", int.MaxValue);
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
            Name = "Crew Member"
        };

        public void RemoveRandomSanity(string exceptionTrigram, int cost)
        {
            var trigrams = _leaderSanities.Select(x => x.Key).ToList();
            if (exceptionTrigram != null)
            {
                trigrams.Remove(exceptionTrigram);
            }

            while (cost > 0)
            {
                LowerSectorSanity(trigrams[Random.Range(0, trigrams.Count)], 1);
            }
        }

        public void LowerSectorSanity(string trigram, int cost)
        {
            if (_leaderSanities[trigram].Sanity <= 0)
            {
                // Already dead...
                return;
            }

            _leaderSanities[trigram].Sanity -= cost;
            if (_leaderSanities[trigram].Sanity <= 0) // Out of sanity...
            {
                // Remove the object
                _leaderSanities[trigram].Image.gameObject.SetActive(false);
                _leaderSanities.Remove(trigram);
            }
        }

        public bool IsLeaderAlive(string trigram)
        {
            if (trigram == null)
            {
                return true;
            }
            return _leaderSanities.ContainsKey(trigram);
        }

        public Model.Event GetCurrentEvent()
            => _eventLoader.CurrentEvent;

        public void NextEvent()
        {
            var isCrisis = _numberOfRoundsWithoutCrisis >= _info.MinTurnBeforeCrisis;

            if (isCrisis)
            {
                if (_tutorialEvents.Count > 0)
                {
                    var ev = _tutorialEvents[0];
                    _eventLoader.Load(ev);
                    _tutorialEvents.RemoveAt(0);
                }
                else
                {
                    _eventLoader.Load(_crisisEvents[Random.Range(0, _crisisEvents.Count)]);
                }
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
                        Name = "New personnel available",
                        Description = "New personnel is available, select which one you want to add to your team.",
                        IsCrisis = false,
                        Choices = new EventChoice[] { choice1, choice2 }
                    }
                );
                _numberOfRoundsWithoutCrisis++;
            }

            _panelLights.gameObject.SetActive(true);
            _nextDayButton.gameObject.SetActive(false);
        }

        public void EndEvent()
        {
            _panelLights.gameObject.SetActive(false);
            _nextDayButton.gameObject.SetActive(true);
            _eventLoader.UnLoad();
        }

        private EventChoice CreateEventChoice(Leader leader, (string, Model.Card) card, int count)
        {
            return new()
            {
                Description = $"Enroll {count} new {card.Item2.Name}.",
                TargetTrigram = leader?.Trigram,
                Cost = "NONE",
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

        public void RemoveCard(string trigram, int count)
        {
            var avCards = _cards.Where(c =>
            {
                if (trigram == "ANY")
                {
                    return true;
                }
                return c.Trigram == trigram;
            }).ToList();
            while (count > 0 && avCards.Any())
            {
                var index = Random.Range(0, avCards.Count);

                _cards.Remove(avCards[index]);
                Destroy(avCards[index].gameObject);
                avCards.RemoveAt(index);
                count--;
            }
            UpdateCardPositions();
        }

        public void RemoveCard(UI.Card card) {
            Destroy(card.gameObject);
            _cards.Remove(card);
            UpdateCardPositions();
        }

        public void UpdateCardPositions() {
            // if <= 6 cards => classic setting
            float step;
            if(_cards.Count <= 6) {
                step = 1.5f;
            } else {
                // if 7 cards or more ??
                step = (_cards.Count - 1f) / 3f;
            }

            var cardSize = ((RectTransform)_cardPrefab.transform).sizeDelta.x / step;
            var half = _cards.Count / 2f;
            for (int i = 0; i < _cards.Count; i++)
            {
                _cards[i].SetTarget(Vector3.right * (i - half) * cardSize + Vector3.right * (cardSize / step));
            }
        }

        public void AddCard(Model.Card card, string leaderTrigram)
        {
            var cardGo = Instantiate(_cardPrefab, _hand);
            cardGo.transform.Translate(-Vector2.up * 10f);
            var cardIns = cardGo.GetComponent<UI.Card>();
            cardIns.Init(card, GetFactionInfo(leaderTrigram));
            _cards.Add(cardIns);

            UpdateCardPositions();
        }
    }
}
