using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static CardsDatabase;

public class Field : MonoBehaviour
{
    private CardsDatabase _database;
    private CardFactory _factory;
    private LevelGenerator _levelGenerator;
        
    private readonly List<Card> _activeCards = new List<Card>();
    
    private int _width = 64;
    private int _height = 64;

    private int _centerCoefficient = 2;
    private float _sizeMultiplier = 0.85f;

    public int CardsCount => _activeCards.Count;

    [Inject]
    public void Construct(CardsDatabase database, LevelGenerator levelGenerator, CardFactory factory)
    {
        _database = database;
        _levelGenerator = levelGenerator;
        _factory = factory;

        Initialize();
    }

    public void AddCard(Card card)
    {
        _activeCards.Add(card);
    }

    public void DeleteCard(Card card)
    {
        _activeCards.Remove(card);
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
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void Initialize()
    {
        var nodes = _levelGenerator.GenerateLevel();

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var node in nodes)
        {
            Vector2 uiPos = GetCanvasPosition(node, _width, _height);

            if (uiPos.x < minX) minX = uiPos.x;
            if (uiPos.x + _width > maxX) maxX = uiPos.x + _width;
            if (uiPos.y < minY) minY = uiPos.y;
            if (uiPos.y + _height > maxY) maxY = uiPos.y + _height;
        }

        float centerX = (minX + maxX) / _centerCoefficient;
        float centerY = (minY + maxY) / _centerCoefficient;
        Vector2 centerOffset = new Vector2(centerX, centerY);

        foreach (var node in nodes)
        {
            Card card = _factory.Create(node.CardTypeId, transform);
            RectTransform cardRect = card.GetComponent<RectTransform>();

            if (cardRect != null)
            {
                Vector2 uiPos = GetCanvasPosition(node, _width, _height);

                cardRect.localScale = Vector3.one;
                cardRect.sizeDelta = new Vector2(_width, _height);
                cardRect.anchoredPosition = GetCanvasPosition(node, _width, _height) - centerOffset;

                foreach (RectTransform child in cardRect)
                {
                    child.localScale = Vector3.one;
                    child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y, 0f);
                }
                
                if (_database.TryGetCard(node.CardTypeId, out CardType cardType))
                {
                    card.InitializeCardData(cardType);
                    _activeCards.Add(card);
                }
            }
        }
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
}
