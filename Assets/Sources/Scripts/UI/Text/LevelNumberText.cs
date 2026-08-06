using UniRx;

public class LevelNumberText : UILevelCountText
{
    private void OnEnable()
    {
        _state.LevelNumber.Subscribe((int count) =>
        {
            _text.text = count.ToString();
        })
        .AddTo(this);
    }
}
