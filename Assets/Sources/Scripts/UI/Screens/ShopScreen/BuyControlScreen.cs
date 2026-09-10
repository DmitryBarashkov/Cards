using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BuyControlScreen : UIScreen
{
    public List<ControlSprite> Sprites;
    [SerializeField] private Image _controlIcon;
    [SerializeField] private BuyForCoinsButton _buyForCoinsButton;
    [SerializeField] private BuyByAdsButton _buyForAdsButton;
    [SerializeField] private TextMeshProUGUI _priceText;

    [Inject] private PlayerStats _stats;

    public void Initialize(ControlType type, int price)
    {
        _priceText.text = price.ToString();

        _buyForAdsButton.SetReward(type);

        if (TryGetSprite(type, out Sprite result))
            _controlIcon.sprite = result;

        if (price <= _stats.CurrentCoins.Value)
            _buyForCoinsButton.SetProductType(type);
        else
            _buyForCoinsButton.SetEnabled(false);
    }

    private bool TryGetSprite(ControlType type, out Sprite result)
    {
        foreach (var item in Sprites)
        {
            if (item.Type == type)
            {
                result = item.Sprite;
                return true;
            }
        }

        result = null;
        return false;
    }

    [Serializable]
    public struct ControlSprite
    {
        public ControlType Type;
        public Sprite Sprite;
    }
}
