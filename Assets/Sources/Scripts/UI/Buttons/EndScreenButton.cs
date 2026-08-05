using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class EndScreenButton : UIButton, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI _caption;
    [SerializeField] private List<Image> _icons;

    [SerializeField] protected EndGameScreen _screen;

    [Header("Settings")]
    [SerializeField] private Color _disabledTextColor;
    [SerializeField] private Color _disabledIconColor;

    public void SetEnabled(bool isEnabled)
    {
        if (_button == null)
            return;

        _button.interactable = isEnabled;

        if (_caption != null)
            _caption.color = isEnabled ? Color.white : _disabledTextColor;

        if (_icons.Count > 0)
        {
            _icons.ForEach((image) =>
            {
                image.color = isEnabled ? Color.white : _disabledIconColor;
            });
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_screen != null)
            _screen.OnPointerClick(eventData);
    }
}
