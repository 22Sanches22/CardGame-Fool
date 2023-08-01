using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CardGame_Fool.GameFool;

internal class BotPlayer : IPlayer
{
    private readonly List<Card> _cards = new(IPlayer.MaxCardsCount);

    private PlayerActions? _choicedAction = null;

    public BotPlayer(string name)
    {
        Name = name;
    }

    public event Action? TakedСardsFromDeck;
    public event Action? MakeMoved;
    public event Action? BeatedCard;
    public event Action? TakedCards;

    public string Name { get; }

    public Card[] Cards => _cards.ToArray();
    public int CardsCount => _cards.Count;
    
    public Card[] GetTrumpCards(Suits trumpSuit)
    {
        return _cards.Where(card => card.Suit == trumpSuit).ToArray();
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
    
    public void TakeСardsFromDeck(Deck cardsDeck, int cardsCount)
    {
        if ((cardsCount < 0) || (cardsCount > IPlayer.MaxCardsCount))
        {
            throw new InvalidOperationException("The number of cards requested is less than zero " +
                "or greater than the maximum allowed.");
        }

        // If there are not enough cards in the deck, he takes all that is.
        if (cardsCount > cardsDeck.CardsCount)
        {
            cardsCount = cardsDeck.CardsCount;
        }
        
        for (var i = 0; i < cardsCount; i++)
        {
            _cards.Add(cardsDeck.GetTopCard());
        }

        TakedСardsFromDeck?.Invoke();
    }

    public void MakeMove(Card cardToMove)
    {
        if (!_cards.Contains(cardToMove))
        {
            throw new ArgumentException("The requested card is not in the player's hand.");
        }

        _cards.Remove(cardToMove);

        MakeMoved?.Invoke();
    }

    public void BeatCard(Card cardToBeat)
    {
        if (!_cards.Contains(cardToBeat))
        {
            throw new ArgumentException("The requested card is not in the player's hand.");
        }

        _cards.Remove(cardToBeat);

        BeatedCard?.Invoke();
    }

    public void TakeCards(IEnumerable<Card> сards)
    {
        _cards.AddRange(_cards);

        TakedCards?.Invoke();
    }

    public Card WaitCardChoiceToMakeMove()
    {

    }

    public Card WaitCardChoiceToBeatCard()
    {
        
    }
}