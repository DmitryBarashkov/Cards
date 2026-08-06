using UniRx;

public class ShuffleCountText : UIPlayerCountText
{
    private void OnEnable()
    {
        _state.currentShuffles.Subscribe((int count) =>
        {
            _text.text = count.ToString();
        })
        .AddTo(this);
    }
}
