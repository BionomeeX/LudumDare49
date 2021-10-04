using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Unstable.UI
{
    public class LeaderText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        public string Trigram {set; private get;}

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GameManager.Instance.LeaderSpeaking(Trigram);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GameManager.Instance.LeaderStopSpeaking();
        }
    }

}
