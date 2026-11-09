using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class SizeAdapter : ITickable
    {
        private RectTransform _uiRoot;
        private List<CanvasScaler> _canvasScalers;
        private float _portraitWidth = 1080;
        private float _portraitHeight = 1920;
        private float _albumWidth = 1920;
        private float _albumHeight = 1080;

        private int _lastWidth;
        private int _lastHeight;
        private ScreenOrientation _lastOrientation;

        [Inject]
        public void Construct(RectTransform root, List<CanvasScaler> canvasScalers)
        {
            _uiRoot = root;
            _canvasScalers = canvasScalers;

            ApplyScaleMode();
            ResetTrackedValues();
        }

        public void ApplyScaleMode()
        {
            bool isPortrait = Screen.height > Screen.width;
            float width = isPortrait ? _portraitWidth : _albumWidth;

            _uiRoot.sizeDelta = new Vector2(width, _uiRoot.sizeDelta.y);

            foreach (var scaler in _canvasScalers)
            {
                scaler.referenceResolution = isPortrait
                    ? new Vector2(_portraitWidth, _portraitHeight)
                    : new Vector2(_albumWidth, _albumHeight);
            }
        }

        public void Tick()
        {
            if (Screen.width != _lastWidth || Screen.height != _lastHeight || Screen.orientation != _lastOrientation)
            {
                ResetTrackedValues();
                ApplyScaleMode();
            }
        }

        private void ResetTrackedValues()
        {
            _lastWidth = Screen.width;
            _lastHeight = Screen.height;
            _lastOrientation = Screen.orientation;
        }
    }
}