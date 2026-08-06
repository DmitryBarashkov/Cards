using TMPro;
using UnityEngine;
using Zenject;

public abstract class UIPlayerCountText : MonoBehaviour
{
    [Inject] protected PlayerStats _state;

    [SerializeField] protected TextMeshProUGUI _text;
}
