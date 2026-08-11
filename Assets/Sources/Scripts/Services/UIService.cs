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

    private Transform _endGameContainer;
    private Transform _shopContainer;

    private readonly Dictionary<Component, GameObject> _cachedWindows = new();

    [Inject]
    public void Construct(DiContainer container, UIScreen winScreenPrefab, UIScreen loseScreenPrefab,
                     [Inject(Optional = true)] UIScreen shopScreenPrefab, [Inject(Optional = true)] UIScreen buyGameScreen,
                     Transform endGameContainer, Transform shopContainer)
    {
        _container = container;
        _winScreenPrefab = winScreenPrefab;
        _loseScreenPrefab = loseScreenPrefab;
        _shopScreenPrefab = shopScreenPrefab;
        _buyScreenPrefab = buyGameScreen;
        _endGameContainer = endGameContainer;
        _shopContainer = shopContainer;
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
