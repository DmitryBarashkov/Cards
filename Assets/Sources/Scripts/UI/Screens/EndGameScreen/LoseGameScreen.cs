using UnityEngine;

namespace Cards.UI
{
    public class LoseGameScreen : MonoBehaviour
    {
        [SerializeField] private AddClearButton _clearbutton;
        [SerializeField] private AddCancelButton _cancelButton;

        private void OnEnable()
        {
            _clearbutton.SetEnabled(true);
            _cancelButton.SetEnabled(true);
        }
    }
}