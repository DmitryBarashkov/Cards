using UniRx;

public class CancelCountText : UIPlayerCountText
{
    private void OnEnable()
    {
        _state.CurrentCancels.Subscribe((int count) =>
        {
            _text.text = count.ToString();
        })
        .AddTo(this);
    }
}
