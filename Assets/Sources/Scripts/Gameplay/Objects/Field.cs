using System.Collections.Generic;
using Cards.Databases;
using DG.Tweening;
using UnityEngine;
using Zenject;
using static Cards.Databases.CardsDatabase;

namespace Cards.Gameplay
{
    public class Field : MonoBehaviour
    {
        private readonly List<Card> _activeCards = new List<Card>();

        [SerializeField] private ClearContainer _clearContainer;
        [SerializeField] private RectTransform _background;
        [SerializeField] private RectTransform _cardsContainer;
        [SerializeField] private float _shuffleDuration = 0.5f;
        [SerializeField] private Ease _shuffleEaseType = Ease.OutQuad;

        private CardsDatabase _database;
        private CardFactory _factory;
        private Card _card;
        private InputService _input;
        private UIService _uiService;
        private IAudioService _audioService;

        private int _centerCoefficient = 2;
        private float _sizeMultiplier = 0.85f;

        public int CardsCount => _activeCards.Count;

        [Inject]
        public void Construct(
            CardsDatabase database,
            CardFactory factory,
            InputService input,
            UIService uiService,
            IAudioService audioService)
        {
            _database = database;
            _factory = factory;
            _input = input;
            _uiService = uiService;
            _audioService = audioService;
        }

        private void OnEnable()
        {
            _input.GenerateLevelBtnPressed += ShowGenerateLevelScreen;
        }

        private void OnDisable()
        {
            _input.GenerateLevelBtnPressed -= ShowGenerateLevelScreen;
        }

        private void Update()
        {
            _input.GetInput();
        }

        public void AddCard(Card card)
        {
            _activeCards.Add(card);
            UpdateCardsHighlight();
        }

        public void DeleteCard(Card card)
        {
            _activeCards.Remove(card);

            if (card.IsCleared)
                _clearContainer.DeleteCard(card);

            UpdateCardsHighlight();
        }

        public bool IsCardOverlapped(Card targetCard)
        {
            Transform container = targetCard.transform.parent;
            int targetIndex = targetCard.transform.GetSiblingIndex();
            int childCount = container.childCount;

            if (targetIndex >= childCount - 1)
                return false;

            Rect targetScreenRect = GetScreenRect(targetCard.RectTransform);

            for (int i = targetIndex + 1; i < childCount; i++)
            {
                Transform child = container.GetChild(i);

                if (child.gameObject.activeSelf == false)
                    continue;

                if (child.TryGetComponent<Card>(out var otherCard))
                {
                    if (_activeCards.Contains(otherCard) == false)
                        continue;

                    Rect otherScreenRect = GetScreenRect(otherCard.RectTransform);

                    if (targetScreenRect.Overlaps(otherScreenRect))
                        return true;
                }
            }

            return false;
        }

        public void UpdateCardsHighlight()
        {
            foreach (var card in _activeCards)
            {
                bool isOverlapped = IsCardOverlapped(card);

                card.ToggleHighlight(isOverlapped == false);
            }
        }

        public void Initialize(List<CardNode> nodes)
        {
            ClearField();
            GenerateCards(nodes);
        }

        public void ShuffleCards()
        {
            int childCount = _cardsContainer.childCount - 1;

            if (childCount <= 1)
                return;

            List<Transform> cards = new List<Transform>();
            List<Vector3> positions = new List<Vector3>();
            List<int> siblingIndices = new List<int>();

            for (int i = 1; i <= childCount; i++)
            {
                Transform card = _cardsContainer.GetChild(i);

                cards.Add(card);
                positions.Add(card.localPosition);
                siblingIndices.Add(card.GetSiblingIndex());
            }

            for (int i = childCount - 1; i > 0; i--)
            {
                int randomIndex = UnityEngine.Random.Range(0, i + 1);
                Vector3 tempPos = positions[i];

                positions[i] = positions[randomIndex];
                positions[randomIndex] = tempPos;

                int tempIndex = siblingIndices[i];

                siblingIndices[i] = siblingIndices[randomIndex];
                siblingIndices[randomIndex] = tempIndex;
            }

            Sequence shuffleSequence = DOTween.Sequence();

            _audioService.PlaySound(SoundType.Shuffle);

            for (int i = 0; i < childCount; i++)
            {
                Transform card = cards[i];

                card.SetSiblingIndex(siblingIndices[i]);
                shuffleSequence.Join(card.DOLocalMove(positions[i], _shuffleDuration).SetEase(_shuffleEaseType));
            }

            shuffleSequence.OnComplete(() =>
            {
                UpdateCardsHighlight();
            });
        }

        public void MoveToClearContainer(Card card)
        {
            AddCard(card);

            _clearContainer.AddNewCard(card);
        }

        private void ClearField()
        {
            foreach (var card in _activeCards)
                Destroy(card.gameObject);

            _activeCards.Clear();
            _clearContainer.Clear();
        }

        private void GenerateCards(List<CardNode> nodes)
        {
            nodes.Sort((a, b) =>
            {
                int zCompare = a.GridPosition.z.CompareTo(b.GridPosition.z);

                if (zCompare != 0)
                    return zCompare;

                return b.GridPosition.y.CompareTo(a.GridPosition.y);
            });

            float width = _factory.Width;
            float height = _factory.Height;
            float pureWidth = _factory.PureWidth;
            float pureHeight = _factory.PureHeight;

            float layerVisualShiftY = pureHeight * 0.2f;
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (var node in nodes)
            {
                Vector2 uiPos = GetCanvasPosition(node, pureWidth, pureHeight);

                uiPos.y += node.GridPosition.z * layerVisualShiftY;

                if (uiPos.x < minX)
                    minX = uiPos.x;
                if (uiPos.x + width > maxX)
                    maxX = uiPos.x + width;
                if (uiPos.y < minY)
                    minY = uiPos.y;
                if (uiPos.y + height > maxY)
                    maxY = uiPos.y + height;
            }

            float centerX = (minX + maxX) / _centerCoefficient;
            float centerY = (minY + maxY) / _centerCoefficient;
            Vector2 centerOffset = new Vector2(centerX, centerY);

            foreach (var node in nodes)
            {
                Card card = _factory.Create(node.CardTypeId, _cardsContainer);
                RectTransform cardRect = card.GetComponent<RectTransform>();

                if (cardRect != null)
                {
                    Vector2 uiPos = GetCanvasPosition(node, pureWidth, pureHeight);

                    uiPos.y += node.GridPosition.z * layerVisualShiftY;

                    cardRect.localScale = Vector3.one;
                    cardRect.sizeDelta = new Vector2(width, height);
                    cardRect.anchoredPosition = uiPos - centerOffset;
                    cardRect.anchoredPosition3D = new Vector3(cardRect.anchoredPosition.x, cardRect.anchoredPosition.y, -node.GridPosition.z * 0.1f);

                    foreach (var child in cardRect)
                    {
                        if (child is RectTransform rect)
                        {
                            rect.localScale = Vector3.one;
                            rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y, 0f);
                        }
                    }

                    if (_database.TryGetCard(node.CardTypeId, out CardType cardType))
                    {
                        card.InitializeCardData(cardType);
                        _activeCards.Add(card);
                    }
                }
            }

            CenterCards();
            UpdateCardsHighlight();
        }

        private Rect GetScreenRect(RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];

            rectTransform.GetWorldCorners(corners);

            Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
            Camera camera = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

            Vector2 screenCorner0 = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
            Vector2 screenCorner2 = RectTransformUtility.WorldToScreenPoint(camera, corners[2]);

            float width = screenCorner2.x - screenCorner0.x;
            float height = screenCorner2.y - screenCorner0.y;

            float newWidth = width * _sizeMultiplier;
            float newHeight = height * _sizeMultiplier;

            float offsetX = (width - newWidth) / _centerCoefficient;
            float offsetY = (height - newHeight) / _centerCoefficient;

            return new Rect(screenCorner0.x + offsetX, screenCorner0.y + offsetY, newWidth, newHeight);
        }

        private Vector2 GetCanvasPosition(CardNode node, float width, float height)
        {
            float posX = node.GridPosition.x * (width / _centerCoefficient);
            float posY = node.GridPosition.y * (height / _centerCoefficient);

            return new Vector2(posX, posY);
        }

        private void CenterCards()
        {
            if (_activeCards.Count > 0)
            {
                float minTileX = float.MaxValue;
                float maxTileX = float.MinValue;
                float minTileY = float.MaxValue;
                float maxTileY = float.MinValue;

                foreach (Card card in _activeCards)
                {
                    RectTransform rectTransform = card.GetComponent<RectTransform>();

                    if (rectTransform == null)
                        continue;

                    Vector2 pos = rectTransform.anchoredPosition;

                    if (pos.x < minTileX)
                        minTileX = pos.x;

                    if (pos.x > maxTileX)
                        maxTileX = pos.x;

                    if (pos.y < minTileY)
                        minTileY = pos.y;

                    if (pos.y > maxTileY)
                        maxTileY = pos.y;
                }

                float realCenterX = (minTileX + maxTileX) / 2f;
                float realCenterY = (minTileY + maxTileY) / 2f;
                Vector2 finalVisualCenter = new Vector2(realCenterX, realCenterY);
                RectTransform containerRect = _cardsContainer.GetComponent<RectTransform>();

                if (containerRect != null)
                    containerRect.anchoredPosition = -finalVisualCenter;
            }
        }

        private void ShowGenerateLevelScreen()
        {
            _uiService.ShowGenerateLevelScreen();
        }
    }
}