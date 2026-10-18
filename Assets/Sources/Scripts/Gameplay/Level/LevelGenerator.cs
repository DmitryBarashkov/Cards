using System;
using System.Collections.Generic;
using System.Linq;
using Cards.Databases;
using Cards.Services;
using UnityEngine;
using Zenject;

namespace Cards.Gameplay
{
    public class LevelGenerator
    {
        private int _totalTriplets;
        private int _uniqueTypes;
        private int _gridWidth;
        private int _gridHeight;
        private LevelShape _shape;

        private CardNode[,,] _levelGrid;
        private int tripletInt = 3;

        private List<CardNode> _generatedCards;

        [Inject]
        public void Construct(LoadLevelService service)
        {
            SetGeneratorParams(service.GetLevelConfig());
        }

        public List<CardNode> GetInitialNodes()
        {
            return _generatedCards;
        }

        public void SetGeneratorParams(LevelConfig config)
        {
            _totalTriplets = config.TotalTriplets;
            _uniqueTypes = config.UniqueTypesCount;

            _gridWidth = config.GridWidth;
            _gridHeight = config.GridHeight;

            _shape = config.Shape;
        }

        public List<CardNode> GenerateEasyMedium()
        {
            _generatedCards = new List<CardNode>();

            int requiredCardsCount = _totalTriplets * tripletInt;
            int absoluteMaxLayers = 20;

            List<int> layerCapacities = new List<int>();
            int totalEstimatedCapacity = 0;
            int maxLayers = 0;

            for (int z = 0; z < absoluteMaxLayers; z++)
            {
                int cardsOnThisLayer = 0;
                for (int x = 0; x < _gridWidth; x++)
                {
                    for (int y = 0; y < _gridHeight; y++)
                    {
                        if (IsPositionInsideShape(x, y, z) == false)
                            continue;

                        if (z % 2 == 0)
                        {
                            if (x % 2 != 0 || y % 2 != 0)
                                continue;
                        }
                        else
                        {
                            if (x % 2 == 0 || y % 2 == 0)
                                continue;
                        }

                        cardsOnThisLayer++;
                    }
                }

                if (cardsOnThisLayer == 0)
                    break;

                layerCapacities.Add(cardsOnThisLayer);
                totalEstimatedCapacity += cardsOnThisLayer;
                maxLayers++;

                if (totalEstimatedCapacity >= requiredCardsCount)
                    break;
            }

            _levelGrid = new CardNode[_gridWidth, _gridHeight, maxLayers];

            List<Vector3Int> allValidPositions = new List<Vector3Int>();

            for (int z = 0; z < maxLayers; z++)
            {
                for (int x = 0; x < _gridWidth; x++)
                {
                    for (int y = 0; y < _gridHeight; y++)
                    {
                        if (IsPositionInsideShape(x, y, z) == false)
                            continue;

                        if (z % 2 == 0)
                        {
                            if (x % 2 != 0 || y % 2 != 0)
                                continue;
                        }
                        else
                        {
                            if (x % 2 == 0 || y % 2 == 0)
                                continue;
                        }

                        allValidPositions.Add(new Vector3Int(x, y, z));
                    }
                }
            }

            if (allValidPositions.Count < requiredCardsCount)
            {
                Debug.Log($"[LevelGenerator] Ошибка! Фигура физически не вмещает такое количество карт. " +
                               $"Требуется: {requiredCardsCount}, Вместимость сетки: {allValidPositions.Count}. Увеличьте GridWidth/GridHeight.");
                return _generatedCards;
            }

            float centerX = (_gridWidth - 1) / 2f;
            float centerY = (_gridHeight - 1) / 2f;

            allValidPositions.Sort((a, b) =>
            {
                if (a.z != b.z)
                    return a.z.CompareTo(b.z);

                float distA = Mathf.Pow(a.x - centerX, 2) + Mathf.Pow(a.y - centerY, 2);
                float distB = Mathf.Pow(b.x - centerX, 2) + Mathf.Pow(b.y - centerY, 2);

                return distA.CompareTo(distB);
            });

            List<Vector3Int> positionsToFill = allValidPositions.GetRange(0, requiredCardsCount);
            List<int> cardPool = CreateCardPool();

            for (int i = 0; i < positionsToFill.Count; i++)
            {
                Vector3Int pos = positionsToFill[i];
                int cardTypeId = cardPool[i];

                CardNode newNode = new CardNode
                {
                    GridPosition = pos,
                    CardTypeId = cardTypeId,
                    IsOccupied = true,
                };

                _levelGrid[pos.x, pos.y, pos.z] = newNode;
                _generatedCards.Add(newNode);
            }

            Debug.Log($"[LevelGenerator] Пирамида ({_shape}) построена! " +
                      $"Высота: {maxLayers} слоев. " +
                      $"Вместимость сетки: {allValidPositions.Count}. " +
                      $"Размещено карт: {_generatedCards.Count}");
            return _generatedCards;
        }

        public List<CardNode> GenerateHard()
        {
            _generatedCards = new List<CardNode>();
            int requiredCardsCount = _totalTriplets * tripletInt;
            int absoluteMaxLayers = 15;

            LogOptimalCardsCount(absoluteMaxLayers);

            int[,] maxHeightsGrid = new int[_gridWidth, _gridHeight];
            float seedX = UnityEngine.Random.Range(0f, 100f);
            float seedY = UnityEngine.Random.Range(0f, 100f);
            float noiseScale = 0.12f;

            for (int x = 0; x < _gridWidth; x++)
            {
                for (int y = 0; y < _gridHeight; y++)
                {
                    float noise = Mathf.PerlinNoise((x * noiseScale) + seedX, (y * noiseScale) + seedY);
                    int minAllowedHeight = 3;
                    int calculatedHeight = Mathf.RoundToInt(minAllowedHeight + (noise * (absoluteMaxLayers - minAllowedHeight)));

                    maxHeightsGrid[x, y] = calculatedHeight;
                }
            }

            List<Vector3Int> allPositions = new List<Vector3Int>();

            for (int z = 0; z < absoluteMaxLayers; z++)
            {
                for (int x = 0; x < _gridWidth; x++)
                {
                    for (int y = 0; y < _gridHeight; y++)
                    {
                        if (IsPositionInsideShape(x, y, 0) == false)
                            continue;

                        if (z % 2 == 0)
                        {
                            if (x % 2 != 0 || y % 2 != 0)
                                continue;
                        }
                        else
                        {
                            if (x % 2 == 0 || y % 2 == 0)
                                continue;
                        }

                        if (z > maxHeightsGrid[x, y])
                            continue;

                        allPositions.Add(new Vector3Int(x, y, z));
                    }
                }
            }

            if (allPositions.Count < requiredCardsCount)
            {
                allPositions.Clear();

                for (int z = 0; z < absoluteMaxLayers; z++)
                {
                    for (int x = 0; x < _gridWidth; x++)
                    {
                        for (int y = 0; y < _gridHeight; y++)
                        {
                            if (IsPositionInsideShape(x, y, 0) == false)
                                continue;

                            if (z % 2 == 0)
                            {
                                if (x % 2 != 0 || y % 2 != 0)
                                    continue;
                            }
                            else
                            {
                                if (x % 2 == 0 || y % 2 == 0)
                                    continue;
                            }

                            allPositions.Add(new Vector3Int(x, y, z));
                        }
                    }
                }
            }

            float centerX = (_gridWidth - 1) / 2f;
            float centerY = (_gridHeight - 1) / 2f;

            allPositions.Sort((a, b) =>
            {
                if (a.z != b.z)
                    return a.z.CompareTo(b.z);

                float distA = Mathf.Abs(a.x - centerX) + Mathf.Abs(a.y - centerY);
                float distB = Mathf.Abs(b.x - centerX) + Mathf.Abs(b.y - centerY);

                return distA.CompareTo(distB);
            });

            HashSet<Vector3Int> targetPositions = GetPositions(allPositions, requiredCardsCount);
            int finalMaxLayers = 0;

            foreach (var p in targetPositions)
            {
                if (p.z > finalMaxLayers)
                    finalMaxLayers = p.z;
            }

            _levelGrid = new CardNode[_gridWidth, _gridHeight, finalMaxLayers + 1];

            List<int> availableCardTypes = new List<int>();
            int tripletsToPlace = _totalTriplets;

            for (int i = 0; i < _uniqueTypes; i++)
                availableCardTypes.Add(i);

            while (tripletsToPlace > 0 && targetPositions.Count >= 3)
            {
                List<Vector3Int> freePositions = new List<Vector3Int>();

                foreach (var pos in targetPositions)
                {
                    bool isBlocked = false;
                    int topZ = pos.z + 1;

                    if (topZ <= finalMaxLayers)
                    {
                        Vector3Int[] overlayers = new Vector3Int[]
                        {
                            new Vector3Int(pos.x + 1, pos.y + 1, topZ),
                            new Vector3Int(pos.x - 1, pos.y + 1, topZ),
                            new Vector3Int(pos.x + 1, pos.y - 1, topZ),
                            new Vector3Int(pos.x - 1, pos.y - 1, topZ),
                        };

                        foreach (var over in overlayers)
                        {
                            if (over.x >= 0 && over.x < _gridWidth && over.y >= 0 && over.y < _gridHeight)
                            {
                                if (targetPositions.Contains(over) && _levelGrid[over.x, over.y, over.z] == null)
                                {
                                    isBlocked = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (isBlocked == false)
                        freePositions.Add(pos);
                }

                if (freePositions.Count < 3)
                    freePositions = new List<Vector3Int>(targetPositions);

                freePositions.Sort((a, b) => a.z.CompareTo(b.z));

                int poolSize = Mathf.Min(12, freePositions.Count);
                List<Vector3Int> lowerPool = freePositions.GetRange(0, poolSize);
                List<Vector3Int> chosenPositions = new List<Vector3Int>();

                for (int k = 0; k < 3; k++)
                {
                    if (lowerPool.Count == 0)
                        break;

                    int idx = UnityEngine.Random.Range(0, lowerPool.Count);

                    chosenPositions.Add(lowerPool[idx]);
                    targetPositions.Remove(lowerPool[idx]);
                    lowerPool.RemoveAt(idx);
                }

                if (chosenPositions.Count < 3)
                    break;

                int cardTypeId = availableCardTypes[UnityEngine.Random.Range(0, availableCardTypes.Count)];

                foreach (var pos in chosenPositions)
                {
                    CardNode newNode = new CardNode
                    {
                        GridPosition = pos,
                        CardTypeId = cardTypeId,
                        IsOccupied = true,
                    };

                    _levelGrid[pos.x, pos.y, pos.z] = newNode;
                    _generatedCards.Add(newNode);
                }

                tripletsToPlace--;
            }

            Debug.Log($"[LevelGenerator Hard] Готово! Слоев: {finalMaxLayers + 1}. Карт: {_generatedCards.Count}. Проходимость: 100%.");
            return _generatedCards;
        }

        private void LogOptimalCardsCount(int absoluteMaxLayers)
        {
            int totalPositionsAccumulated = 0;

            Debug.Log($"=== [АНАЛИЗ ВМЕСТИМОСТИ ФОРМЫ ПО СЛОЯМ] ===");
            Debug.Log($"Размер сетки: {_gridWidth} x {_gridHeight}");

            for (int z = 0; z < absoluteMaxLayers; z++)
            {
                int cardsOnThisLayer = 0;

                for (int x = 0; x < _gridWidth; x++)
                {
                    for (int y = 0; y < _gridHeight; y++)
                    {
                        if (IsPositionInsideShape(x, y, 0) == false)
                            continue;

                        if (z % 2 == 0)
                        {
                            if (x % 2 != 0 || y % 2 != 0)
                                continue;
                        }
                        else
                        {
                            if (x % 2 == 0 || y % 2 == 0)
                                continue;
                        }

                        cardsOnThisLayer++;
                    }
                }

                if (cardsOnThisLayer == 0)
                {
                    Debug.Log($"[Слой {z}] Пусто. Форма физически закончилась на высоте {z} слоев.");
                    break;
                }

                totalPositionsAccumulated += cardsOnThisLayer;

                int optimalTriplets = totalPositionsAccumulated / 3;
                int optimalCardsCount = optimalTriplets * 3;
                int leftoverEmptySpaces = totalPositionsAccumulated % 3;

                Debug.Log($" Слой {z:00} | " +
                          $"Мест на слое: {cardsOnThisLayer:000} | " +
                          $"Всего мест (0-{z}): {totalPositionsAccumulated:000} || " +
                          $"ОПТИМАЛЬНО КАРТ: {optimalCardsCount:000} ({optimalTriplets} трипл.) | " +
                          $"Остаток дыр: {leftoverEmptySpaces}");
            }

            Debug.Log($"===========================================");
        }

        private HashSet<Vector3Int> GetPositions(List<Vector3Int> allPositions, int requiredCardsCount)
        {
            List<Vector3Int> preSelectedPositions = allPositions.GetRange(0, requiredCardsCount);
            int highestZ = 0;

            foreach (var pos in preSelectedPositions)
            {
                if (pos.z > highestZ)
                    highestZ = pos.z;
            }

            if (highestZ > 0)
            {
                List<Vector3Int> topLayerCards = preSelectedPositions.Where(pos => pos.z == highestZ).ToList();
                List<Vector3Int> tilesToRemove = new List<Vector3Int>();

                foreach (var tile in topLayerCards)
                {
                    int normX = tile.x / 2;
                    int normY = tile.y / 2;

                    if ((normX + normY) % 2 != 0)
                        tilesToRemove.Add(tile);
                }

                foreach (var tile in tilesToRemove)
                    preSelectedPositions.Remove(tile);

                int shortage = requiredCardsCount - preSelectedPositions.Count;

                if (shortage > 0)
                {
                    var availableLowerPositions = allPositions
                        .Where(pos => pos.z < highestZ && !preSelectedPositions.Contains(pos))
                        .ToList();

                    if (availableLowerPositions.Count >= shortage)
                        preSelectedPositions.AddRange(availableLowerPositions.GetRange(0, shortage));
                    else
                        Debug.LogWarning("[LevelGenerator] Не удалось полностью компенсировать шахматный слой снизу.");
                }
            }

            return new HashSet<Vector3Int>(preSelectedPositions);
        }

        private bool IsPositionInsideShape(int x, int y, int z)
        {
            float centerX = _gridWidth / 2f;
            float centerY = _gridHeight / 2f;
            float baseRadius = Mathf.Min(centerX, centerY);
            float layerInsetStep = 1.0f;
            float currentInset = z * layerInsetStep;
            float currentRadius = baseRadius - currentInset;

            if (currentRadius <= 0)
                return false;

            switch (_shape)
            {
                case LevelShape.Circle:
                    float circleX = (x < centerX) ? x : x + 1f;
                    float circleY = (y < centerY) ? y : y + 1f;
                    float distCircleSq = Mathf.Pow(circleX - centerX, 2) + Mathf.Pow(circleY - centerY, 2);

                    return distCircleSq <= Mathf.Pow(currentRadius, 2);

                case LevelShape.Diamond:
                    float maxRadiusX = centerX - currentInset;
                    float maxRadiusY = centerY - currentInset;

                    if (maxRadiusX <= 0 || maxRadiusY <= 0)
                        return false;

                    float diamondX = (x < centerX) ? x : x + 1f;
                    float diamondY = (y < centerY) ? y : y + 1f;

                    return (Mathf.Abs(diamondX - centerX) / maxRadiusX) + (Mathf.Abs(diamondY - centerY) / maxRadiusY) <= 1.0f;

                case LevelShape.Triangle:
                    float minValidY = currentInset;
                    float maxValidY = (_gridHeight - 1) - currentInset;

                    if (y < minValidY || y > maxValidY)
                        return false;

                    float normalizedY = (float)(y - minValidY) / (maxValidY - minValidY);
                    float triangleWidthAtY = (centerX - currentInset) * normalizedY;
                    float leftmostX = x;
                    float rightmostX = x + 1f;

                    return leftmostX >= (centerX - triangleWidthAtY) && rightmostX <= (centerX + triangleWidthAtY);

                case LevelShape.Hourglass:
                    if (y < currentInset || y > (_gridHeight - 1) - currentInset)
                        return false;

                    float normY = (((float)y / (_gridHeight - 1)) * 2f) - 1f;
                    float baseWidthAtY = 0.25f + (0.75f * Mathf.Abs(normY));
                    float currentHourglassWidth = baseWidthAtY * (1.0f - (currentInset / centerX));

                    if (currentHourglassWidth <= 0)
                        return false;

                    float hX = (((float)x / (_gridWidth - 1)) * 2f) - 1f;
                    float hWidthHalf = 1f / _gridWidth;
                    float furthestHX = (x < centerX) ? hX : hX + hWidthHalf;

                    return Mathf.Abs(furthestHX) <= currentHourglassWidth;

                case LevelShape.Donut:
                    float donutOuterRadius = baseRadius - currentInset;
                    float donutInnerRadius = (baseRadius * 0.4f) + currentInset;

                    if (donutOuterRadius <= donutInnerRadius)
                        return false;

                    float dOuterX = (x < centerX) ? x : x + 1f;
                    float dOuterY = (y < centerY) ? y : y + 1f;
                    float dInnerX = (x < centerX) ? x + 1f : x;
                    float dInnerY = (y < centerY) ? y + 1f : y;

                    float distOuterSq = Mathf.Pow(dOuterX - centerX, 2) + Mathf.Pow(dOuterY - centerY, 2);
                    float distInnerSq = Mathf.Pow(dInnerX - centerX, 2) + Mathf.Pow(dInnerY - centerY, 2);

                    return distOuterSq <= Mathf.Pow(donutOuterRadius, 2) && distInnerSq >= Mathf.Pow(donutInnerRadius, 2);

                case LevelShape.Heart:
                    float heartScale = 1.0f - (currentInset / baseRadius);

                    if (heartScale <= 0)
                        return false;

                    float[] cornersX = { x, x + 1f };
                    float[] cornersY = { y, y + 1f };

                    foreach (float cx in cornersX)
                    {
                        foreach (float cy in cornersY)
                        {
                            float hScaleX = (((cx / (_gridWidth - 1)) * 3f) - 1.5f) / heartScale;
                            float hScaleY = (((cy / (_gridHeight - 1)) * 3f) - 1.3f) / heartScale;

                            float f = (hScaleX * hScaleX) + (hScaleY * hScaleY) - 1f;
                            if (((f * f * f) - (hScaleX * hScaleX * hScaleY * hScaleY * hScaleY)) > 0f)
                                return false;
                        }
                    }

                    return true;

                case LevelShape.Cross:
                    float baseThicknessX = _gridWidth * 0.35f;
                    float baseThicknessY = _gridHeight * 0.35f;

                    float currentThicknessX = baseThicknessX - (currentInset * 2f);
                    float currentThicknessY = baseThicknessY - (currentInset * 2f);
                    if (currentThicknessX <= 0 || currentThicknessY <= 0)
                        return false;

                    bool inVerticalBar = Mathf.Abs(x - centerX) <= currentThicknessX / 2f && Mathf.Abs((x + 1f) - centerX) <= currentThicknessX / 2f;
                    bool inHorizontalBar = Mathf.Abs(y - centerY) <= currentThicknessY / 2f && Mathf.Abs((y + 1f) - centerY) <= currentThicknessY / 2f;

                    return inVerticalBar || inHorizontalBar;

                case LevelShape.Rectangle:
                default:
                    return x >= (centerX - currentRadius) && x < (centerX + currentRadius) &&
                           y >= (centerY - currentRadius) && y < (centerY + currentRadius);
            }
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
                Debug.LogWarning($"[LevelGenerator] Ошибка баланса: totalTriplets ({_totalTriplets}) меньше, чем uniqueTypesCount ({_uniqueTypes})! Карточек не хватит, чтобы показать все типы. Увеличьте totalTriplets в инспекторе.");
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
    }
}