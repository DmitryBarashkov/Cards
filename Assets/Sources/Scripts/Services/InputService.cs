using System;
using UnityEngine;

public class InputService
{
    private const string GenerateLevelMenu = "GenerateLevelMenu";

    private bool _isActive = true;

    public event Action GenerateLevelBtnPressed;

    public bool IsActive => _isActive;

    public void Activate()
    {
        _isActive = true;
    }

    public void Deactivate()
    {
        _isActive = false;
    }

    public void GetInput()
    {
        if (Input.GetButton(GenerateLevelMenu))
            GenerateLevelBtnPressed?.Invoke();
    }
}
