using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardsDatabase", menuName = "Config/Cards Database")]
public class CardsDatabase : ScriptableObject
{
    [Serializable]
    public struct CardType
    {
        public int id;
        public Color color;
        public Sprite sprite;
    }

    [Header("Card Types")]
    public List<CardType> cardTypes;

    public bool TryGetCard(int id, out CardType result)
    {
        foreach (var card in cardTypes)
        {
            if (card.id == id)
            {
                result = card;
                return true;
            }
        }

        result = default;
        return false;
    }
}
