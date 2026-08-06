using UniRx;

public class LeftCardsText : UILevelCountText
{
    private void OnEnable()
    {
        _state.CardsCount.Subscribe((int count) =>
        {
            _text.text = count.ToString();
        })
        .AddTo(this);
    }
}
