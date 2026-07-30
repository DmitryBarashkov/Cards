using UnityEngine;

public class ClearCell : MonoBehaviour
{
    private bool _isEmpty = true;

    public bool IsEmpty => _isEmpty;

    public void SetEmpty(bool value)
    {
        _isEmpty = value;
    }
}
