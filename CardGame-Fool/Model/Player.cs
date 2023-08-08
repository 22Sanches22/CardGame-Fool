using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGameFool.Model;

public abstract class Player
{
    /// <summary> The maximum count of cards in a player's hands. </summary>
    public const int MaxCardsCount = 6;

    protected readonly List<Card> _cards = new(MaxCardsCount);

    private PlayerActions? _choicedAction = null;

    public Player(string name)
    {
        Name = name;
    }

    public Card[] Cards => _cards.ToArray();
    public int CardsCount => _cards.Count;

    public abstract event Action? MakeMoved;
    public abstract event Action? BeatedCard;

    public event Action? TakedСardsFromDeck;
    public event Action? TakedCards;

    public string Name { get; }

    public abstract void MakeMove(Card card);

    public abstract void BeatCard(Card card);

    public Card[] GetTrumpCards()
    {
        return _cards.Where(card => card.IsTrump).ToArray();
    }

    public Card[] GetSortedCards()
    {
        return _cards.OrderBy(card => card.Importance).ToArray();
    }

    public void SetAction(PlayerActions action)
    {
        _choicedAction = action;
    }

    public PlayerActions WaitСhoiceAction()
    {
        while (_choicedAction is null)
            ;

        PlayerActions returnedValue = (PlayerActions)_choicedAction;
        _choicedAction = null;

        return returnedValue;
    }

    public void TakeСardsFromDeck(Deck deck, int cardsCount)
    {
        if ((cardsCount < 0) || (cardsCount > MaxCardsCount))
        {
            throw new InvalidOperationException("The number of cards requested is less than zero " +
                "or greater than the maximum allowed.");
        }

        // If there are not enough cards in the deck, he takes all that is.
        if (cardsCount > deck.Count)
        {
            cardsCount = deck.Count;
        }

        for (var i = 0; i < cardsCount; i++)
        {
            _cards.Add(deck.GetTopCard());
        }

        TakedСardsFromDeck?.Invoke();
    }

    public void TakeCards(IEnumerable<Card> сards)
    {
        _cards.AddRange(_cards);

        TakedCards?.Invoke();
    }

    public abstract Card WaitCardChoiceToMakeMove();

    public abstract Card WaitCardChoiceToBeatCard();
}
