using UnityEngine;
using UnityEngine.UI;
using Unstable.Model;

namespace Unstable.UI
{
    public class EventChoiceLoader : MonoBehaviour
    {
        [SerializeField]
        private Text _title, _description;

        public void Init(EventChoice choice)
        {
            _title.text = GameManager.Instance.GetLeaderFromTrigram(choice.TargetTrigram).DomainName;
            _description.text = choice.Description;
        }
    }
}
