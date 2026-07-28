
public class InputService
{
    private bool _isActive = true;

    public bool IsActive => _isActive;

    public void Activate()
    {
        _isActive = true;
    }

    public void Deactivate()
    {
        _isActive = false;
    }
}
