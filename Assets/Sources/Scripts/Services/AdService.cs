using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using YG;
using Zenject;

public class AdService : ITickable, IDisposable
{
    private GameObject _adWarningPanel;
    private TextMeshProUGUI _countText;

    private Level _level;

    private int _countdownDuration = 3;

    private float _timer;
    private bool _isCountingDown;
    private bool _isActive = true;

    private float _punchScaleFactor = 1.3f;
    private float _punchDuration = 0.3f;
    private int _lastDisplayedSecond = -1;

    [Inject]
    public void Construct(
        Level level,
        [Inject(Id = "AdWarning")] GameObject adWarningPanel,
        [Inject(Id = "AdvCountText")] TextMeshProUGUI countText)
    {
        _level = level;
        _adWarningPanel = adWarningPanel;
        _countText = countText;

        YG2.onCloseInterAdv += ResetShowAd;
        YG2.onErrorInterAdv += ResetShowAd;
    }

    public void Tick()
    {
        if (YG2.saves.IsAdsDisabled || _isActive == false)
            return;

        if (_isCountingDown)
        {
            ShowAdCounter();
            return;
        }

        if (_isCountingDown == false && _level.IsActive && YG2.isTimerAdvCompleted)
            StartAdCountDown();
    }

    public void SetActive(bool value)
    {
        _isActive = value;
    }

    public void Dispose()
    {
        YG2.onCloseInterAdv -= ResetShowAd;
        YG2.onErrorInterAdv -= ResetShowAd;
    }

    private void PerformTextAnimation()
    {
        if (_countText == null)
            return;

        _countText.text = _lastDisplayedSecond.ToString();
        _countText.transform.localScale = Vector3.one;
        _countText.transform.DOKill();

        _countText.transform.DOPunchScale(Vector3.one * (_punchScaleFactor - 1f), _punchDuration, 0, 0)
            .SetUpdate(true);
    }

    private void ShowAdCounter()
    {
        _timer -= Time.unscaledDeltaTime;

        if (_timer > 0)
        {
            int currentSecond = Mathf.CeilToInt(_timer);

            if (currentSecond != _lastDisplayedSecond)
            {
                _lastDisplayedSecond = currentSecond;
                PerformTextAnimation();
            }
        }
        else
        {
            _isCountingDown = false;

            if (_adWarningPanel != null)
                _adWarningPanel.SetActive(false);

            if (_countText != null)
                _countText.transform.DOKill();

            YG2.InterstitialAdvShow();
        }
    }

    private void StartAdCountDown()
    {
        if (_isCountingDown)
            return;

        _timer = _countdownDuration;
        _isCountingDown = true;
        _lastDisplayedSecond = _countdownDuration;

        if (_adWarningPanel != null)
            _adWarningPanel.SetActive(true);

        PerformTextAnimation();
    }

    private void ResetShowAd()
    {
        _isCountingDown = false;
        _lastDisplayedSecond = -1;

        if (_adWarningPanel != null)
            _adWarningPanel.SetActive(false);
    }
}
