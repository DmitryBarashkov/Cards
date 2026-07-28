using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class LeftCardsText : MonoBehaviour
{
    [Inject] private LevelState _state;

    [SerializeField] private TextMeshProUGUI _text;
    
    private void OnEnable()
    {
        _state.CardsCount.Subscribe((int count) =>
        {
            _text.text = count.ToString();
        })
        .AddTo(this);
    }
}
