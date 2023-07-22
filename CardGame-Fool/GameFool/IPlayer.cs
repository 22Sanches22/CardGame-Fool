using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame_Fool.GameFool;

public interface IPlayer
{
    public const int MaxCardsCount = 6;

    public event EventHandlerPlayer? TakedСardsFromDeck;
    public event EventHandlerPlayer? MakeMoved;
    public event EventHandlerPlayer? BeatedCard;
    public event EventHandlerPlayer? TakedCards;

    public string Name { get; }

    public IEnumerable<Card> GetCards();
    
    public int CardsCount { get; }

    public void ChoiceAction(PlayerActions action);
    public PlayerActions WaitingСhoice();

    public void TakeСardsFromDeck(Stack<Card> cardsDeck, uint cardsCount);
    public void MakeMove(Card cardToMove);
    public void BeatCard(Card cardToBeat);
    public void TakeCards(IEnumerable<Card> сards);
}

public enum PlayerActions
{
    None,
    BeatCard,
    TakeCards
}

public delegate void EventHandlerPlayer();