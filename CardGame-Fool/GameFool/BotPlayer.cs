using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame_Fool.GameFool;

internal class BotPlayer : IPlayer
{
    private readonly List<Card> _cards = new(IPlayer.MaxCardsCount);

    private PlayerActions _choicedAction = PlayerActions.None;

    public BotPlayer(string name)
    {
        Name = name;
    }

    public event Action? TakedСardsFromDeck;
    public event Action? MakeMoved;
    public event Action? BeatedCard;
    public event Action? TakedCards;

    public string Name { get; }
    
    public int CardsCount => _cards.Count;

    public IEnumerable<Card> GetCards()
    {
        foreach (Card card in _cards)
        {
            yield return card;
        }
    }

    public void ChoiceAction(PlayerActions action)
    {
        if (action == PlayerActions.None)
        {
            throw new ArgumentException("It is necessary to set the action and not its absence.");
        }

        _choicedAction = action;
    }

    public PlayerActions WaitСhoiceAction()
    {
        while (_choicedAction == PlayerActions.None)
            ;

        PlayerActions returnedValue = _choicedAction;
        _choicedAction = PlayerActions.None;

        return returnedValue;
    }

    public void TakeСardsFromDeck(Stack<Card> cardsDeck, uint cardsCount)
    {
        if (cardsCount > IPlayer.MaxCardsCount)
        {
            throw new InvalidOperationException("The count of requested cards exceeds the maximum allowed.");
        }

        // If there are not enough cards in the deck, he takes all that is.
        if (cardsCount > cardsDeck.Count)
        {
            cardsCount = cardsDeck.Count.ToUint();
        }

        for (var i = 0; i < cardsCount; i++)
        {
            _cards.Add(cardsDeck.Pop());
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
        foreach (Card card in сards)
        {
            _cards.Add(card);
        }

        TakedCards?.Invoke();
    }
}