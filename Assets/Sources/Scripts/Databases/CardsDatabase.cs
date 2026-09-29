using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cards.Databases
{
    [CreateAssetMenu(fileName = "CardsDatabase", menuName = "Config/Cards Database")]
    public class CardsDatabase : ScriptableObject
    {
        [Header("Card Types")]
        public List<CardType> CardTypes;

        public bool TryGetCard(int id, out CardType result)
        {
            foreach (var card in CardTypes)
            {
                if (card.Id == id)
                {
                    result = card;
                    return true;
                }
            }

            result = default;
            return false;
        }

        [Serializable]
        public struct CardType
        {
            public int Id;
            public Sprite Sprite;
        }
    }
}