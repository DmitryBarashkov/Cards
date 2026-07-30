using Zenject;

public class ShuffleCardsButton : UIButton
{
    [Inject] private Field _field;
    
    public override void HandleClick()
    {
        _field.ShuffleCards();
    }
}
