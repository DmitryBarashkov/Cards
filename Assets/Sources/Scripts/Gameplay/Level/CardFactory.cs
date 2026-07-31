using UnityEngine;
using Zenject;
using static CardsDatabase;

public class CardFactory
{
    private readonly DiContainer _container;
    private readonly CardsDatabase _database;
    private Card _prefab;
    
    public CardFactory(DiContainer container, CardsDatabase database, Card prefab)
    {
        _container = container;
        _database = database;
        _prefab = prefab;        
    }

    public Card Create(int id, Transform parent)
    {
        if (_database.TryGetCard(id, out CardType cardType))
        {
            Card card = _container.InstantiatePrefabForComponent<Card>(_prefab, parent);

            return card;
        }

        return null;
    }
}
