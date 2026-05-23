using UnityEngine;
using System.Collections.Generic;

namespace ChronicSurvival.Cards
{
    [CreateAssetMenu(fileName = "ActionCardDatabase", menuName = "ChronicSurvival/Cards/Action Card Database")]
    public class ActionCardDatabase : ScriptableObject
    {
        public List<ActionCard> cards = new List<ActionCard>();

        public List<ActionCard> GetPlayableCards(int currentDay)
        {
            var result = new List<ActionCard>();
            foreach (var card in cards)
            {
                if (card != null && card.isUnlocked && card.unlockAtDay <= currentDay && card.cardType != CardType.Debuff)
                {
                    result.Add(card);
                }
            }
            return result;
        }
    }
}
