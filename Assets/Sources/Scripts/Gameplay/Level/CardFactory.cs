using System;
using Cards.Databases;
using UnityEngine;
using Zenject;

using static Cards.Databases.CardsDatabase;

namespace Cards.Gameplay
{
    public class CardFactory
    {
        private const float ContainerWidthOffset = 5;
        private const float ContainerHeightOffset = 20;

        private readonly DiContainer _container;
        private readonly CardsDatabase _database;
        private Card _prefab;

        private float _width;
        private float _height;
        private float _pureWidth;
        private float _pureHeight;

        public CardFactory(DiContainer container, CardsDatabase database, Card prefab)
        {
            _container = container;
            _database = database;
            _prefab = prefab;

            SetCardSizes();
        }

        public float Width => _width;

        public float Height => _height;

        public float PureWidth => _pureWidth;

        public float PureHeight => _pureHeight;

        public Card Create(int id, Transform parent)
        {
            if (_database.TryGetCard(id, out CardType cardType))
            {
                Card card = _container.InstantiatePrefabForComponent<Card>(_prefab, parent);

                return card;
            }

            return null;
        }

        private void SetCardSizes()
        {
            RectTransform rectTransform = _prefab.GetComponent<RectTransform>();

            _width = rectTransform.sizeDelta.x;
            _height = rectTransform.sizeDelta.y;
            _pureWidth = _width - ContainerWidthOffset;
            _pureHeight = _height - ContainerHeightOffset;
        }
    }
}