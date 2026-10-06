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

        public List<CardNode> Generate()
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