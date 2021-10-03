using UnityEngine;

namespace Unstable.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameInfo", fileName = "GameInfo")]
    public class GameInfo : ScriptableObject
    {
        [Range(0f, 1f)]
        [Tooltip("Chance that the game put a staff member in a choice instead of a specialized unit")]
        public float StaffMemberChance = .75f;

        [Tooltip("Number of staff card earned when selecting this option")]
        public int StaffCount = 2;

        [Tooltip("Number of turns we need to wait before a crisis")]
        public int MinTurnBeforeCrisis = 5;
    }
}