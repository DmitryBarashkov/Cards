using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
public abstract class UIButton : MonoBehaviour
{
    [Inject] protected IAudioService _audioService;

    protected Button _button;
    protected RectTransform _rectTransform;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
        _rectTransform = GetComponent<RectTransform>();        
    }

    protected virtual void OnEnable()
    {
        _button.onClick.AddListener(HandleClick);
    }

    protected virtual void OnDisable()
    {
        _button.onClick.RemoveListener(HandleClick);
    }

    public abstract void HandleClick();
}