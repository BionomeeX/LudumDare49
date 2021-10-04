using System;
using TMPro;
using UnityEngine.UI;

namespace Unstable.Leaders
{
    [Serializable]
    public class LeaderSpriteInfo
    {
        public string Trigram;

        public Image Sprite;
        public TMP_Text DebugSanity;

        public Image Face;
    }
}
