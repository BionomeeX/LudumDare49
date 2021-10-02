using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;
using Unstable.Model;

namespace Unstable
{
    public class GameManager : MonoBehaviour
    {
        private Leader[] _leaders;
        private Model.Event[] _events;

        private void Start()
        {
            _leaders = JsonConvert.DeserializeObject<Leader[]>(Resources.Load<TextAsset>("Leaders").text);
            _events = JsonConvert.DeserializeObject<Model.Event[]>(Resources.Load<TextAsset>("Events").text);

            Assert.IsNotNull(_leaders, "Leaders info failed to load");
            Assert.IsNotNull(_events, "Events info failed to load");
            Assert.IsTrue(_leaders.Length > 0, "No leader was found");
            Assert.IsTrue(_events.Length > 0, "No event was found");
        }
    }
}