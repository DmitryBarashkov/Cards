using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UIService
{
    private DiContainer _container;

    private UIScreen _winScreenPrefab;
    private UIScreen _loseScreenPrefab;
    private UIScreen _shopScreenPrefab;
    private UIScreen _buyScreenPrefab;
    private UIScreen _generateLevelScreenPrefab;

    private Transform _endGameContainer;
    private Transform _shopContainer;
    private Transform _generateLevelContainer;

    private readonly Dictionary<Component, GameObject> _cachedWindows = new();

    [Inject]
    public void Construct(
        DiContainer container,
        [Inject(Id = "Win")] UIScreen winScreenPrefab,
        [Inject(Id = "Lose")] UIScreen loseScreenPrefab,
        [Inject(Id = "Generate")] UIScreen generateLevelScreenPrefab,
        [Inject(Id = "Shop", Optional = true)] UIScreen shopScreenPrefab, 
        [Inject(Id = "Buy", Optional = true)] UIScreen buyGameScreen,
        [Inject(Id = "EndContainer")] Transform endGameContainer,
        [Inject(Id = "ShopContainer")] Transform shopContainer,
        [Inject(Id = "GenerateContainer")] Transform generateLevelContainer)
    {
        _container = container;
        _winScreenPrefab = winScreenPrefab;
        _loseScreenPrefab = loseScreenPrefab;
        _generateLevelScreenPrefab = generateLevelScreenPrefab;
        _shopScreenPrefab = shopScreenPrefab;
        _buyScreenPrefab = buyGameScreen;
        _endGameContainer = endGameContainer;
        _shopContainer = shopContainer;
        _generateLevelContainer = generateLevelContainer;
    }

    public void ShowShop()
    {
        GameObject shop = GetOrCreateWindow(_shopScreenPrefab, _shopContainer);
        ShopScreen screen = shop.GetComponent<ShopScreen>();

        screen.Setup();
    }

    public void ShowBuyScreen(ControlType type, int price)
    {
        GameObject buyScreen = GetOrCreateWindow(_buyScreenPrefab, _shopContainer);
        BuyControlScreen screen = buyScreen.GetComponent<BuyControlScreen>();

        screen.Initialize(type, price);
        screen.Setup();
    }

    public void ShowEndGameScreen(bool isWin)
    {
        UIScreen targetPrefab = isWin ? _winScreenPrefab : _loseScreenPrefab;
        GameObject window = GetOrCreateWindow(targetPrefab, _endGameContainer);
        UIScreen endGameScreen = window.GetComponent<UIScreen>();

        endGameScreen.Setup();
    }

    public void ShowGenerateLevelScreen()
    {
        GameObject window = GetOrCreateWindow(_generateLevelScreenPrefab, _generateLevelContainer);
        UIScreen generateLevelScreen = window.GetComponent<UIScreen>();

        generateLevelScreen.Setup();
    }

    private GameObject GetOrCreateWindow(UIScreen prefab, Transform container)
    {
        if (_cachedWindows.TryGetValue(prefab, out GameObject activeWindow))
        {
            return activeWindow;
        }

        GameObject spawnedInstance = _container.InstantiatePrefab(prefab, container);

        _cachedWindows[prefab] = spawnedInstance;

        return spawnedInstance;
    }
}
