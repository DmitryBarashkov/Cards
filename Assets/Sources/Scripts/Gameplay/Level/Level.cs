using System;
using Zenject;

public class Level
{
    private LevelState _state;
    private Field _field;
    private Bank _bank;
    private LevelGenerator _generator;
    private UIService _service;

    private int _levelCardsCount;
    
    public int CardsCount => _levelCardsCount;    

    [Inject]
    public void Construct(LevelState state, LevelGenerator generator, Field field, Bank bank, UIService service)
    {
        _state = state;
        _field = field;
        _bank = bank;
        _generator = generator;
        _service = service;

        Initialize();
    }

    public void SetCardsCount()
    {
        _state.CardsCount.Value = _levelCardsCount = _field.CardsCount + _bank.CardsCount;
    }

    public void Restart()
    {
        var nodes = _generator.GetInitialNodes();

        _bank.Clear();
        _field.Initialize(nodes);
        SetCardsCount();
    }

    public void ShowLoseScreen()
    {
        _service.ShowEndGameScreen(false);
    }

    public void ShowWinScreen()
    {
        _service.ShowEndGameScreen(true);
    }

    private void Initialize()
    {
        SetCardsCount();
    }
}
