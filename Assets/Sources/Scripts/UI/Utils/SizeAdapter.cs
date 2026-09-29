using Cards.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SizeAdapter : ITickable
{
    private CanvasScaler[] _canvasScalers;
    private Field _field;

    private Vector2 _portraitResolution = new Vector2(1080, 1920);
    private Vector2 _albumResolution = new Vector2(1920, 1080);
    private Vector2 _portraitFieldSize = new Vector2(800, 1300);
    private Vector2 _albumFieldSize = new Vector2(1300, 600);

    private int _lastWidth;
    private int _lastHeight;
    private ScreenOrientation _lastOrientation;

    [Inject]
    public void Construct(CanvasScaler[] canvasScalers, Field field)
    {
        _canvasScalers = canvasScalers;
        _field = field;

        ApplyScaleMode();
        ResetTrackedValues();
    }

    public void ApplyScaleMode()
    {
        bool isPortrait = Screen.height > Screen.width;

        foreach (CanvasScaler scaler in _canvasScalers)
            scaler.referenceResolution = isPortrait ? _portraitResolution : _albumResolution;

        if (_field != null)
        {
            Vector2 fieldSize = isPortrait ? _portraitFieldSize : _albumFieldSize;
            _field.ApplySize(fieldSize);
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
