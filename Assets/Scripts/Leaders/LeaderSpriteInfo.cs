using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Unstable.Leaders
{
    [Serializable]
    public class LeaderSpriteInfo
    {
        public string Trigram;

        public Image Sprite;
        public Sprite Ending;
        public TMP_Text DebugSanity;

        public Image Face;
    }
}
