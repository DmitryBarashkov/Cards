using Cards.Gameplay;
using TMPro;
using UnityEngine;
using Zenject;

namespace Cards.UI
{
    public class LoseGameScreen : MonoBehaviour
    {
        [SerializeField] private AddClearButton _clearbutton;
        [SerializeField] private AddCancelButton _cancelButton;
        [SerializeField] private TextMeshProUGUI _text;

        [Inject] private Bank _bank;

        private void OnEnable()
        {
            _clearbutton.SetEnabled(true);
            _cancelButton.SetEnabled(true);
        }
    }
}