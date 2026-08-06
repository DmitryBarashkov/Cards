using UniRx;

public class ClearCountText : UIPlayerCountText
{
    private void OnEnable()
    {
        _state.currentCleanings.Subscribe((int count) =>
        {
            _text.text = count.ToString();
        })
        .AddTo(this);
    }
}