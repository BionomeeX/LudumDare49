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
using TMPro;

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

        public int Score = 0;

        public static int CostToInt(string value)
        {
            return value.ToUpperInvariant() switch
            {
                "NONE" => 0,
                "LOW" => 2,
                "MED" => 4,
                "HIGH" => 6,
                "EX" => 8,
                _ => throw new System.ArgumentException("Invalid value " + value, nameof(value))
            };
        }

        public static int RequirementToInt(string value)
        {
            return value.ToUpperInvariant() switch
            {
                "ONE" => 1,
                "LOW" => 3,
                "MED" => 6,
                "HIGH" => 9,
                "EX" => 12,
                _ => throw new System.ArgumentException("Invalid value " + value, nameof(value))
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

        [SerializeField]
        private Image _leaderText;

        public string GetEffect(string trigram)
            => _effects[trigram];

        private FactionInfo GetFactionInfo(string trigram)
        {
            var faction = _factions.FirstOrDefault(x => x.Trigram == trigram);
            return faction ?? _neutralFaction;
        }

        private void Start()
        {
            _leaderText.gameObject.SetActive(false);

            // Load JSON objects
            _leaders = JsonConvert.DeserializeObject<List<Leader>>(Resources.Load<TextAsset>("Leaders").text);

            _effects = JsonConvert.DeserializeObject<Dictionary<string, string>>(Resources.Load<TextAsset>("Effects").text);
            _tutorialEvents = JsonConvert.DeserializeObject<List<Model.Event>>(Resources.Load<TextAsset>("Tutorial").text);

            var decks = GlobalData.Instance.GetAllowedDecks();
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
                var leader = _leaders.FirstOrDefault(f => f.Trigram == x.Trigram);
                x.DebugSanity.text = leader.MaxSanity.ToString();

                x.Face.GetComponent<LeaderText>().Trigram = x.Trigram;

                return (x.Trigram, new LeaderSanity()
                {
                    Image = x.Sprite,
                    Sanity = leader.MaxSanity
                });
            }).ToDictionary(x => x.Trigram, x => x.Item2);

            // Disable oxygen leader
            {
                LowerSectorSanity("OXY", int.MaxValue);
                _crisisEvents.RemoveAll(x => x.Choices.Any(c => c.TargetTrigram == "OXY"));
                _standardEvents.RemoveAll(x => x.Choices.Any(c => c.TargetTrigram == "OXY"));
                _leadersImages.FirstOrDefault(x => x.Trigram == "OXY").Face.gameObject.SetActive(false);
            }

            var images = JsonConvert.DeserializeObject<string[]>(Resources.Load<TextAsset>("ImageKeys").text);
            _eventLoader.Images = images.Select(x =>
            {
                return new EventImage()
                {
                    Code = x.ToLowerInvariant(),
                    Image = Resources.Load<Sprite>("Images/" + x.ToLowerInvariant())
                };
            }).ToArray();

            foreach (var i in _leadersImages)
            {
                i.DebugSanity.gameObject.SetActive(GlobalData.Instance.DisplaySanity);
            }
        }

        public int ReduceCostBy = 0;

        public void PreventNextCrisis()
        {
            _numberOfRoundsWithoutCrisis -= 5;
        }

        private const float _lightOffset = .005f;
        private float _lightObjective = 0.5f;
        private void FixedUpdate()
        {
            Score += 100;

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
                var index = Random.Range(0, trigrams.Count);
                if (LowerSectorSanity(trigrams[index], 1))
                {
                    // Died
                    trigrams.RemoveAt(index);
                }
                --cost;
            }
        }

        public bool LowerSectorSanity(string trigram, int cost)
        {
            if (!_leaderSanities.ContainsKey(trigram) || _leaderSanities[trigram].Sanity <= 0)
            {
                // Already dead...
                return true;
            }

            _leaderSanities[trigram].Sanity -= cost;
            if (_leaderSanities[trigram].Sanity <= 0) // Out of sanity...
            {
                // Remove the object
                _leadersImages.FirstOrDefault(x => x.Trigram == trigram).DebugSanity.text = "0";
                _leaderSanities[trigram].Image.gameObject.SetActive(false);
                _leaderSanities.Remove(trigram);
                return true;
            }
            _leadersImages.FirstOrDefault(x => x.Trigram == trigram).DebugSanity.text = _leaderSanities[trigram].Sanity.ToString();
            return false;
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
            try
            {
                NextEventInternal();
            }
            catch (System.Exception e)
            {
                _eventLoader.Load(new()
                {
                    Name = "Invalid report",
                    Description = "Looks like the daily report wasn't filled properly",
                    Choices = new[]
                    {
                            new Model.EventChoice()
                            {
                                Description = "Hopefully we didn't miss anything important",
                                Cost = "NONE"
                            }
                        }
                });
                Debug.LogError(e);
                NextEvent();
            }
        }

        private void NextEventInternal()
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
                if (_tutorialEvents.Count == 0 && Random.value < .25f)
                {
                    _eventLoader.Load(new()
                    {
                        Name = "Normal day",
                        Description = "Today was uneventful",
                        Choices = new[]
                        {
                            new Model.EventChoice()
                            {
                                Description = "We should use this time wisely",
                                Cost = "NONE"
                            }
                        }
                    });
                }
                else if (_tutorialEvents.Count == 0 && Random.value < .25f) // Random events are only for when we are done with the tutorial
                {
                    _eventLoader.Load(_standardEvents[Random.Range(0, _standardEvents.Count)]);
                }
                else if (_tutorialEvents.Count == 0 && Random.value < .25f && _cards.Count > 0)
                {
                    _eventLoader.Load(new()
                    {
                        Name = "Accident",
                        Description = "One of your crew mate got hurt during work, he need to rest for a bit",
                        Choices = new[]
                        {
                            new Model.EventChoice()
                            {
                                Description = "A regretable incident",
                                Cost = "NONE",
                                Effects = new[]
                                {
                                    new Function()
                                    {
                                        MethodName = "REM",
                                        Argument = "ANY 1"
                                    }
                                }
                            }
                        }
                    });
                }
                else if (_tutorialEvents.Count == 0 && Random.value < .33f && _cards.Count > 10)
                {
                    _eventLoader.Load(new()
                    {
                        Name = "Back to work",
                        Description = "Your team is starting to be quite big and the others sections are understaffed",
                        Choices = new[]
                        {
                            new Model.EventChoice()
                            {
                                Description = "We can still call them later when we will need them",
                                Cost = "NONE",
                                Effects = new[]
                                {
                                    new Function()
                                    {
                                        MethodName = "REM",
                                        Argument = "ANY 3"
                                    }
                                }
                            }
                        }
                    });
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
                            Choices = new EventChoice[] { choice1, choice2,
                                new EventChoice()
                                {
                                    Cost = "NONE",
                                    Description = "We don't need anyone else for now"
                                }
                            }
                        }
                    );
                }
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

        public (Model.Card, string) GetCard(string leaderTrigram, string cardTrigram)
        {
            if (leaderTrigram == null)
            {
                return (_staffCard, null);
            }
            if (leaderTrigram == "ANY")
            {
                var leader = _leaders[Random.Range(0, _leaders.Count)];
                var cards = leader.Cards.ToArray();
                var rand = cards[Random.Range(0, cards.Length)];
                return (rand.Value, leader.Trigram);
            }
            return (_leaders.Where(x => x.Trigram == leaderTrigram.ToUpperInvariant()).ElementAt(0).Cards[cardTrigram.ToUpperInvariant()], leaderTrigram);
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

        public void RemoveCard(UI.Card card)
        {
            Destroy(card.gameObject);
            _cards.Remove(card);
            UpdateCardPositions();
        }

        public void UpdateCardPositions()
        {
            // if <= 6 cards => classic setting
            float step;
            if (_cards.Count <= 4)
            {
                step = 1.5f;
            }
            else
            {
                // if 5 cards or more ??
                step = (_cards.Count - 1f) / 2f;
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

        public void LeaderSpeaking(string trigram)
        {
            _leaderText.gameObject.SetActive(true);
            // get the leader
            var leader = GetLeaderFromTrigram(trigram);
            // get the event
            var curentEvent = _eventLoader.CurrentEvent;

            Dictionary<int, string[]> sentenceDict = leader.SentencesConversation;

            if (curentEvent != null && curentEvent.IsCrisis)
            {
                sentenceDict = leader.SentencesCrisis;
            }

            var possibleTexts = sentenceDict[sentenceDict.Select(kv => kv.Key).OrderByDescending(x => x).Where(v => v < leader.MaxSanity).Max()];
            _leaderText.GetComponentInChildren<TMP_Text>().text = possibleTexts[Random.Range(0, possibleTexts.Length)];
        }

        public void LeaderStopSpeaking()
        {
            _leaderText.gameObject.SetActive(false);
        }
    }
}
