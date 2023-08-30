using System;
using System.Collections.Generic;
using System.Linq;

using CardGameFool.Model.Cards;
using CardGameFool.Model.Players;

namespace CardGameFool.Model.Game;

public class CardsTable
{
    private readonly Card?[] _cards = new Card?[Player.StartingCardsCount * Fool.PlayersNumber];

    public event Action<Card, PlayerActions>? AddingСard;
    public event Action? Cleared;

    public IEnumerable<Card> Cards
    {
        get
        {
            foreach (Card? card in _cards)
            {
                if (card is not null)
                {
                    yield return (Card)card;
                }
            }
        }
    }

    public void AddCard(Card card, PlayerActions addCardThroughAction)
    {
        _cards[FindFreeIndex()] = card;
        AddingСard?.Invoke(card, addCardThroughAction);
    }

    public void Clear()
    {
        for (int i = 0; i < _cards.Length; i++)
        {
            _cards[i] = null;
        }

        Cleared?.Invoke();
    }

    private int FindFreeIndex()
    {
        if (_cards.Contains(null))
        {
            return Array.IndexOf(_cards, null);
        }

        throw new InvalidOperationException("There is no free index.");
    }
}
