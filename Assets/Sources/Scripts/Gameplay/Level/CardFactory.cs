using System;
using Cards.Databases;
using UnityEngine;
using Zenject;

using static Cards.Databases.CardsDatabase;

namespace Cards.Gameplay
{
    public class CardFactory
    {
        private readonly DiContainer _container;
        private readonly CardsDatabase _database;
        private Card _prefab;
        private float _width;
        private float _height;

        public CardFactory(DiContainer container, CardsDatabase database, Card prefab)
        {
            _container = container;
            _database = database;
            _prefab = prefab;

            RectTransform rectTransform = _prefab.GetComponent<RectTransform>();

            _width = rectTransform.sizeDelta.x;
            _height = rectTransform.sizeDelta.y;
        }

        public Card Create(int id, Transform parent)
        {
            if (_database.TryGetCard(id, out CardType cardType))
            {
                Card card = _container.InstantiatePrefabForComponent<Card>(_prefab, parent);

                return card;
            }

            return null;
        }

        public float GetWidth()
        {
            return _width;
        }

        public float GetHeight()
        {
            return _height;
        }
    }
}