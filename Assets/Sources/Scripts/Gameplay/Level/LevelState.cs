using UniRx;

public class LevelState
{
    public ReactiveProperty<int> CardsCount = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> LevelNumber = new ReactiveProperty<int>(0);
}
