using DG.Tweening;
using System;
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

    private int _cooldownTimerDuration = 5;
    private float _timer;
    private bool _isCountingDown;
    private bool _isAddCooldownActive;  

    private float _punchScaleFactor = 1.3f;
    private float _punchDuration = 0.3f;
    private int _lastDisplayedSecond = -1;

    [Inject]
    public void Construct(
        Level level, 
        [Inject(Id = "AdWarning")] GameObject adWarningPanel, 
        [Inject(Id = "AdvCountText")] TextMeshProUGUI countText
    )
    {
        _level = level;
        _adWarningPanel = adWarningPanel;
        _countText = countText;        

        YG2.onCloseInterAdv += ResetShowAd;
        YG2.onErrorInterAdv += ResetShowAd;
    }

    public void Tick()
    {
        if (_isCountingDown == false && _isAddCooldownActive == false && _level.IsActive && YG2.isTimerAdvCompleted)
            StartAdCountDown();
        
        if (_isCountingDown)
            ShowAdCounter();

        if (_isAddCooldownActive)
            UpdateCooldownTimer();
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
        if (_isAddCooldownActive)
            return;
        
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

    private void UpdateCooldownTimer()
    {
        if (_isCountingDown)
            return;

        _timer -= Time.unscaledDeltaTime;

        if (_timer <= 0)
        {
            _isAddCooldownActive = false;
            Debug.Log($"Cooldown completed. Adv Timer: {YG2.isTimerAdvCompleted}");
        }
    }
    
    private void StartCooldownTimer()
    {
        if (_isAddCooldownActive || _isCountingDown)
            return;

        _timer = _cooldownTimerDuration;

        _isAddCooldownActive = true;
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
        
        StartCooldownTimer();
    }
}
