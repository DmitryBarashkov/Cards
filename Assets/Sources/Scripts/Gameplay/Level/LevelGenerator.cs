using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LevelGenerator
{
    private int _totalTriplets;
    private int _uniqueTypes;
    private int _bankSize;

    private int _gridWidth;
    private int _gridHeight;
    private int _maxLayers;

    private LevelShape _shape;

    private CardNode[,,] _levelGrid;
    private int tripletInt = 3;

    private List<CardNode> _generatedCards;

    public enum LevelShape
    {
        Rectangle,
        Circle,
        Diamond,
        Triangle,
    }

    [Inject]
    public void Construct(int totalTriplets, int uniqueTypes, int bankSize, int gridWidth, int gridHeight, int maxLayers)
    {
        SetGeneratorParams(totalTriplets, uniqueTypes, bankSize, gridWidth, gridHeight, maxLayers);
    }

    public void SetGeneratorParams(int totalTriplets, int uniqueTypes, int bankSize, int gridWidth, int gridHeight, int maxLayers)
    {
        _totalTriplets = totalTriplets;
        _uniqueTypes = uniqueTypes;
        _bankSize = bankSize;

        _gridWidth = gridWidth;
        _gridHeight = gridHeight;
        _maxLayers = maxLayers;
        _shape = LevelShape.Rectangle;
    }

    public List<CardNode> Generate()
    {
        int totalCards = _totalTriplets * tripletInt;
        _levelGrid = new CardNode[_gridWidth, _gridHeight, _maxLayers];
        _generatedCards = new List<CardNode>();

        List<int> cardPool = CreateCardPool();
        List<int> reverseBank = new List<int>();

        while (cardPool.Count > 0 || reverseBank.Count > 0)
        {
            if (reverseBank.Count <= _bankSize - tripletInt && cardPool.Count >= tripletInt)
            {
                int typeId = cardPool[0];

                for (int i = 0; i < tripletInt; i++)
                    reverseBank.Add(typeId);

                cardPool.RemoveRange(0, tripletInt);
            }

            if (reverseBank.Count == 0)
                break;

            int cardIndexToPlace = Random.Range(0, reverseBank.Count);
            int currentTypeId = reverseBank[cardIndexToPlace];

            Vector3Int? availablePos = FindAvailablePositionForReverse();

            if (availablePos.HasValue)
            {
                Vector3Int pos = availablePos.Value;

                CardNode newNode = new CardNode
                {
                    GridPosition = pos,
                    CardTypeId = currentTypeId,
                    IsOccupied = true,
                };

                _levelGrid[pos.x, pos.y, pos.z] = newNode;
                _generatedCards.Add(newNode);

                reverseBank.RemoveAt(cardIndexToPlace);
            }
            else
            {
                Debug.LogError("Критическая ошибка: Закончилось свободное место на сетке! Увеличьте размеры сетки.");
                break;
            }
        }

        return _generatedCards;
    }

    public List<CardNode> GetInitialNodes()
    {
        return _generatedCards;
    }

    private List<int> CreateCardPool()
    {
        List<int> pool = new List<int>();

        for (int typeId = 0; typeId < _uniqueTypes; typeId++)
        {
            for (int i = 0; i < 3; i++)
            {
                pool.Add(typeId);
            }
        }

        int remainingTriplets = _totalTriplets - _uniqueTypes;

        if (remainingTriplets < 0)
        {
            Debug.LogError($"Ошибка баланса: totalTriplets ({_totalTriplets}) меньше, чем uniqueTypesCount ({_uniqueTypes})! Карточек не хватит, чтобы показать все типы. Увеличьте totalTriplets в инспекторе.");
            return pool;
        }

        for (int i = 0; i < remainingTriplets; i++)
        {
            int randomType = Random.Range(0, _uniqueTypes);
            for (int j = 0; j < 3; j++)
            {
                pool.Add(randomType);
            }
        }

        int totalGroups = pool.Count / 3;

        for (int i = 0; i < totalGroups; i++)
        {
            int randomIndex = Random.Range(i, totalGroups);

            for (int j = 0; j < 3; j++)
            {
                int temp = pool[(i * 3) + j];

                pool[(i * 3) + j] = pool[(randomIndex * 3) + j];
                pool[(randomIndex * 3) + j] = temp;
            }
        }

        return pool;
    }

    private Vector3Int? FindAvailablePositionForReverse()
    {
        bool isFirstCard = true;

        for (int z = 0; z < _maxLayers; z++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    if (_levelGrid[x, y, z] != null)
                    {
                        isFirstCard = false;
                        break;
                    }
                }

                if (isFirstCard == false)
                    break;
            }
        }

        if (isFirstCard)
        {
            return GetFirstCard();
        }

        List<Vector3Int> validPositions = new List<Vector3Int>();

        for (int z = 0; z < _maxLayers; z++)
        {
            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    if (IsPositionInsideShape(x, y) == false)
                        continue;

                    if (_levelGrid[x, y, z] == null && IsPositionBlockedFromAbove(x, y, z) == false)
                    {
                        if ((z % 2 == 0 && (x % 2 != 0 || y % 2 != 0)) ||
                            (z % 2 != 0 && (x % 2 == 0 || y % 2 == 0)))
                            continue;

                        if (HasNeighborOrSupport(x, y, z))
                            validPositions.Add(new Vector3Int(x, y, z));
                    }
                }
            }
        }

        if (validPositions.Count > 0)
            return validPositions[Random.Range(0, validPositions.Count)];

        return null;
    }

    private Vector3Int? GetFirstCard()
    {
        int centerX = Mathf.RoundToInt(_gridWidth / 2f);
        int centerY = Mathf.RoundToInt(_gridHeight / 2f);
        int startZ = 0;

        switch (_shape)
        {
            case LevelShape.Circle:
            case LevelShape.Diamond:
                return new Vector3Int(centerX, centerY, startZ);
            case LevelShape.Triangle:
                int triangleY = Mathf.RoundToInt(_gridHeight * 0.33f);

                return new Vector3Int(centerX, triangleY, startZ);
            case LevelShape.Rectangle:
            default:
                return new Vector3Int(centerX, centerY, startZ);
        }
    }

    private bool HasNeighborOrSupport(int x, int y, int z)
    {
        if (z > 0)
        {
            int lowerZ = z - 1;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int cx = x + dx;
                    int cy = y + dy;

                    if (cx >= 0 && cx < _gridWidth && cy >= 0 && cy < _gridHeight)
                    {
                        if (_levelGrid[cx, cy, lowerZ] != null)
                            return true;
                    }
                }
            }
        }

        int[] offsets = { -2, 2 };

        foreach (int dx in offsets)
        {
            int cx = x + dx;

            if (cx >= 0 && cx < _gridWidth && _levelGrid[cx, y, z] != null)
                return true;
        }

        foreach (int dy in offsets)
        {
            int cy = y + dy;

            if (cy >= 0 && cy < _gridHeight && _levelGrid[x, cy, z] != null)
                return true;
        }

        return false;
    }

    private bool IsPositionBlockedFromAbove(int x, int y, int z)
    {
        for (int upperZ = z + 1; upperZ < _maxLayers; upperZ++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int checkX = x + dx;
                    int checkY = y + dy;

                    if (checkX >= 0 && checkX < _gridWidth &&
                        checkY >= 0 && checkY < _gridHeight &&
                        _levelGrid[checkX, checkY, upperZ] != null)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool IsPositionInsideShape(int x, int y)
    {
        float centerX = _gridWidth / 2f;
        float centerY = _gridHeight / 2f;

        switch (_shape)
        {
            case LevelShape.Circle:
                float radius = Mathf.Min(centerX, centerY);
                float distanceSquare = Mathf.Pow(x - centerX, 2) + Mathf.Pow(y - centerY, 2);

                return distanceSquare <= Mathf.Pow(radius, 2);

            case LevelShape.Diamond:
                float maxRadiusX = centerX;
                float maxRadiusY = centerY;

                return (Mathf.Abs(x - centerX) / maxRadiusX) + (Mathf.Abs(y - centerY) / maxRadiusY) <= 1.0f;

            case LevelShape.Triangle:
                float normalizedY = (float)y / (_gridHeight - 1);
                float halfWidthAtY = centerX * normalizedY;

                return x >= (centerX - halfWidthAtY) && x <= (centerX + halfWidthAtY);
            case LevelShape.Rectangle:
            default:
                return true;
        }
    }
}
