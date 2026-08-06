using TMPro;
using UnityEngine;
using Zenject;

public class UILevelCountText : MonoBehaviour
{
    [Inject] protected LevelState _state;

    [SerializeField] protected TextMeshProUGUI _text;
}
