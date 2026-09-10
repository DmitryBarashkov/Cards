using UniRx;

public class ClearCountText : UIPlayerCountText
{
    private void OnEnable()
    {
        _state.CurrentCleanings.Subscribe((int count) =>
        {
            _text.text = count.ToString();
        })
        .AddTo(this);
    }
}