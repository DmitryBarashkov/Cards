using Zenject;

public class Level
{
    private LevelState _state;
    private Field _field;
    private Bank _bank;

    [Inject]
    public void Construct(LevelState state, Field field, Bank bank)
    {
        _state = state;
        _field = field;
        _bank = bank;

        Initialize();
    }

    public void SetCardsCount()
    {
        _state.CardsCount.Value = _field.CardsCount + _bank.CardsCount;
    }

    private void Initialize()
    {
        SetCardsCount();
    }
}
