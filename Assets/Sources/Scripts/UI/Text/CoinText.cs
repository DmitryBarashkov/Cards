using UniRx;

public class CoinText : UIPlayerCountText
{
    private void OnEnable()
    {
        _state.currentCoins.Subscribe((int count) =>
        {
            _text.text = count.ToString();
        })
        .AddTo(this);
    }
}
