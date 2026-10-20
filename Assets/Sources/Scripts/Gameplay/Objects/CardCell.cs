using UnityEngine;
using Zenject;

namespace Cards.Gameplay
{
    public class CardCell : MonoBehaviour
    {
        [Inject] private CardFactory _factory;
        private RectTransform _rectTransform;
        private bool _isEmpty = true;

        public bool IsEmpty => _isEmpty;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.sizeDelta = new Vector2(_factory.GetWidth(), _factory.GetHeight());
        }

        public void SetEmpty(bool value)
        {
            _isEmpty = value;
        }
    }
}