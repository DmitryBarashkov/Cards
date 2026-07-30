using System;
using Zenject;

public class Level
{
    private LevelState _state;
    private Field _field;
    private Bank _bank;
    private LevelGenerator _generator;

    [Inject]
    public void Construct(LevelState state, LevelGenerator generator, Field field, Bank bank)
    {
        _state = state;
        _field = field;
        _bank = bank;
        _generator = generator;

        Initialize();
    }

    public void SetCardsCount()
    {
        _state.CardsCount.Value = _field.CardsCount + _bank.CardsCount;
    }

    public void Restart()
    {
        var nodes = _generator.GetInitialNodes();

        _bank.Clear();
        _field.Initialize(nodes);
        SetCardsCount();
    }

    private void Initialize()
    {
        SetCardsCount();
    }
}
