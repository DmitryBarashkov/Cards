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
    private int _levelNumber;
    
    public int CardsCount => _levelCardsCount;    

    [Inject]
    public void Construct(LevelState state, LevelGenerator generator, Field field, Bank bank, UIService service, int levelNumber)
    {
        _state = state;
        _field = field;
        _bank = bank;
        _generator = generator;
        _service = service;
        _levelNumber = levelNumber;

        Initialize();
    }

    public void SetLevelState()
    {
        _state.CardsCount.Value = _levelCardsCount = _field.CardsCount + _bank.CardsCount;
        _state.LevelNumber.Value = _levelNumber;
    }

    public void Restart()
    {
        var nodes = _generator.GetInitialNodes();

        _bank.Clear();
        _field.Initialize(nodes);
        SetLevelState();
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
        SetLevelState();
    }
}
