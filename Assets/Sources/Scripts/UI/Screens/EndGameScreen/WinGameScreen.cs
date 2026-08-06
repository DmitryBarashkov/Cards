using DG.Tweening;
using TMPro;
using UnityEngine;
using YG;
using Zenject;

public class WinGameScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _cardsCountText;
    [SerializeField] private TextMeshProUGUI _coinsCountText;    

    [SerializeField] private AddCoinsButton _addCoinsButton;    

    [Inject] private Level _level;
    [Inject] private PlayerStats _stats;

    private int _coinsFactor = 1;
    private float _effectDuration = 1f;
    private int _earnedCoins;

    private void OnEnable()
    {
        int cardsCount = _level.CardsCount;        

        _earnedCoins = cardsCount * _coinsFactor;

        _cardsCountText.text = $"x {cardsCount}";
        _coinsCountText.text = $"x {_earnedCoins}";        

        _addCoinsButton.SetEnabled(true);

        YG2.saves.coins += _earnedCoins;
        YG2.saves.rating += cardsCount;
        YG2.saves.level++;
        YG2.SaveProgress();
        YG2.SetLeaderboard("Score", YG2.saves.rating);

        _stats.currentCoins.Value = YG2.saves.coins;
    }

    public void AddCoins(int coinsMultiplier)
    {
        int currentCoins = _earnedCoins;

        _earnedCoins *= coinsMultiplier;

        DOTween.To(() => currentCoins, x => currentCoins = x, _earnedCoins, _effectDuration)
            .OnUpdate(() =>
            {
                _coinsCountText.text = $"x {currentCoins}";
            })
            .SetEase(Ease.OutQuad);

        YG2.saves.coins += _earnedCoins - currentCoins;
        YG2.SaveProgress();

        _stats.currentCoins.Value = YG2.saves.coins;
    }
}