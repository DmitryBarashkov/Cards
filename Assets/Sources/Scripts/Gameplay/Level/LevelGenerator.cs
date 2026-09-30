using System;
using System.Collections.Generic;
using Cards.Databases;
using Cards.Services;
using UnityEngine;
using Zenject;

namespace Cards.Gameplay
{
    public class LevelGenerator
    {
        private const int MaxAttempts = 10;

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

        [Inject]
        public void Construct(LoadLevelService service)
        {
            SetGeneratorParams(service.GetLevelConfig());
        }

        public void SetGeneratorParams(LevelConfig config)
        {
            _totalTriplets = config.TotalTriplets;
            _uniqueTypes = config.UniqueTypesCount;
            _bankSize = config.BankSize;

            _gridWidth = config.GridWidth;
            _gridHeight = config.GridHeight;
            _maxLayers = config.MaxLayers;
            _shape = config.Shape;
        }

        public List<CardNode> Generate()
        {
            for (int attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                int totalCards = _totalTriplets * tripletInt;

                _levelGrid = new CardNode[_gridWidth, _gridHeight, _maxLayers];
                _generatedCards = new List<CardNode>();

                List<int> cardPool = CreateCardPool();
                List<int> reverseBank = new List<int>();
                bool isGenerationFailed = false;

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

                    int cardIndexToPlace = UnityEngine.Random.Range(0, reverseBank.Count);
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
                        isGenerationFailed = true;
                        Debug.LogWarning("Закончилось свободное место на сетке! Увеличьте размеры сетки.");
                        break;
                    }
                }

                if (isGenerationFailed)
                    continue;

                return _generatedCards;
            }

            Debug.LogError("Не удалось созать уровень!");
            return null;
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
                Debug.LogWarning($"Ошибка баланса: totalTriplets ({_totalTriplets}) меньше, чем uniqueTypesCount ({_uniqueTypes})! Карточек не хватит, чтобы показать все типы. Увеличьте totalTriplets в инспекторе.");
                return pool;
            }

            for (int i = 0; i < remainingTriplets; i++)
            {
                int randomType = UnityEngine.Random.Range(0, _uniqueTypes);
                for (int j = 0; j < 3; j++)
                {
                    pool.Add(randomType);
                }
            }

            int totalGroups = pool.Count / 3;

            for (int i = 0; i < totalGroups; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, totalGroups);

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

                    if (!isFirstCard)
                        break;
                }
            }

            if (isFirstCard)
                return GetFirstCard();

            List<Vector3Int> strictPositions = new List<Vector3Int>();
            List<Vector3Int> fallbackPositions = new List<Vector3Int>();

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
                            fallbackPositions.Add(new Vector3Int(x, y, z));

                            if ((z % 2 == 0 && (x % 2 != 0 || y % 2 != 0)) ||
                                (z % 2 != 0 && (x % 2 == 0 || y % 2 == 0)))
                                continue;

                            if (HasNeighborOrSupport(x, y, z))
                                strictPositions.Add(new Vector3Int(x, y, z));
                        }
                    }
                }
            }

            if (strictPositions.Count > 0)
                return strictPositions[UnityEngine.Random.Range(0, strictPositions.Count)];

            if (fallbackPositions.Count > 0)
            {
                Debug.LogWarning("Алгоритм зашел в микро-тупик, активировано резервное место.");
                return fallbackPositions[UnityEngine.Random.Range(0, fallbackPositions.Count)];
            }

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
            float centerX = (_gridWidth - 1) / 2f;
            float centerY = (_gridHeight - 1) / 2f;
            float padding = 0.05f;
            float nx = (x - centerX) / (centerX > 0 ? centerX : 1f);
            float ny = (y - centerY) / (centerY > 0 ? centerY : 1f);

            nx /= 1f - padding;
            ny /= 1f - padding;

            switch (_shape)
            {
                case LevelShape.Circle:
                    return (nx * nx) + (ny * ny) <= 1f;

                case LevelShape.Diamond:
                    return (Mathf.Abs(nx) + Mathf.Abs(ny)) <= 1f;

                case LevelShape.Triangle:
                    bool insideHorizontal = Mathf.Abs(nx) <= (1f - ((ny + 1f) / 2f));
                    bool insideVertical = ny >= -1f && ny <= 1f;

                    return insideHorizontal && insideVertical;
                case LevelShape.Hourglass:
                    float widthAtY = 0.25f + (0.75f * Mathf.Abs(ny));

                    return Mathf.Abs(nx) <= widthAtY && Mathf.Abs(ny) <= 1f;
                case LevelShape.Donut:
                    float normDistSq = (nx * nx) + (ny * ny);
                    float outerRadiusSq = 1f;
                    float innerRadiusSq = 0.16f;

                    return normDistSq <= outerRadiusSq && normDistSq >= innerRadiusSq;
                case LevelShape.Heart:
                    float hx = nx * 1.5f;
                    float hy = (ny * 1.5f) - 0.2f;

                    float equationLeft = (hx * hx) + (hy * hy) - 1f;
                    float equationRight = hx * hx * hy * hy * hy;

                    return (equationLeft * equationLeft * equationLeft) - equationRight <= 0f;
                case LevelShape.Cross:
                    float thickness = 0.35f;
                    bool inVerticalBar = Mathf.Abs(nx) <= thickness;
                    bool inHorizontalBar = Mathf.Abs(ny) <= thickness;

                    return (inVerticalBar || inHorizontalBar) && Mathf.Abs(nx) <= 1f && Mathf.Abs(ny) <= 1f;
                case LevelShape.Rectangle:
                    return Mathf.Abs(nx) <= 1f && Mathf.Abs(ny) <= 1f;
                default:
                    return true;
            }
        }
    }
}