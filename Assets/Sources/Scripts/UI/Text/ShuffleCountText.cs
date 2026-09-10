using UniRx;

public class ShuffleCountText : UIPlayerCountText
{
    private void OnEnable()
    {
        _state.CurrentShuffles.Subscribe((int count) =>
        {
            _text.text = count.ToString();
        })
        .AddTo(this);
    }
}
