using System;
using Cards.Gameplay;
using DG.Tweening;
using TMPro;
using UnityEngine;
using YG;
using Zenject;

namespace Cards.Services
{
    public class AdService : ITickable, IDisposable
    {
        private const float ShowAdTime = 300f;
        private const int MaxLevelWithoutAds = 3;

        private GameObject _adWarningPanel;
        private TextMeshProUGUI _countText;

        private Level _level;

        private int _countdownDuration = 3;

        private float _timer;
        private float _showTimer;
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
            _showTimer = ShowAdTime;

            YG2.onCloseInterAdv += ResetShowAd;
            YG2.onErrorInterAdv += ResetShowAd;
        }

        public void Tick()
        {
            if (YG2.saves.IsAdsDisabled || _isActive == false || YG2.saves.Level <= MaxLevelWithoutAds)
                return;

            _showTimer -= Time.deltaTime;

            if (_isCountingDown)
            {
                ShowAdCounter();
                return;
            }

            if (_isCountingDown == false && _level.IsActive && YG2.isTimerAdvCompleted && _showTimer <= 0)
                StartAdCountDown();
        }

        public void SetActive(bool value)
        {
            _isActive = value;
        }

        public void ShowInterstitialAdv()
        {
            if (YG2.saves.Level > MaxLevelWithoutAds)
            YG2.InterstitialAdvShow();
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
            _showTimer = ShowAdTime;

            if (_adWarningPanel != null)
                _adWarningPanel.SetActive(false);
        }
    }
}