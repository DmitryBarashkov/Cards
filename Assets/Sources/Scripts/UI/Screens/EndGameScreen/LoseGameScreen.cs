using TMPro;
using UnityEngine;
using Zenject;

public class LoseGameScreen : MonoBehaviour
{
    [SerializeField] private AddCellButton _addCellButton;
    [SerializeField] private AddClearButton _clearbutton;
    [SerializeField] private AddCancelButton _cancelButton;
    [SerializeField] private TextMeshProUGUI _text;

    [Inject] private Bank _bank;

    private void OnEnable()
    {
        if (_bank.IsAllCellsEnabled)
        {
            _addCellButton.gameObject.SetActive(false);
            _text.gameObject.SetActive(false);
        }
        else
        {
            _addCellButton.SetEnabled(true);
        }

        _clearbutton.SetEnabled(true);
        _cancelButton.SetEnabled(true);
    }
}
