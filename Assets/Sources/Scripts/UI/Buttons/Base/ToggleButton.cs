using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ToggleButton : UIButton, IPointerClickHandler
{
    [SerializeField] protected EndGameScreen _screen;

    private const string DisabledTextHex = "#5B5B5B";
    private const string DisabledIconHex = "#787878";

    [SerializeField] private TextMeshProUGUI _caption;
    [SerializeField] private List<Image> _icons;

    private Color _disabledTextColor = Utils.GetColorFromHex(DisabledTextHex);
    private Color _disabledIconColor = Utils.GetColorFromHex(DisabledIconHex);

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
