using UnityEngine;

namespace Unstable.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/FactionInfo", fileName = "FactionInfo")]
    public class FactionInfo : ScriptableObject
    {
        public string Trigram;
        public Sprite CardBackground;
    }
}