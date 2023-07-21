using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame_Fool.GameFool;

internal interface IPlayer
{
    public const int MaxNumberOfCardsInHand = 6;

    public event EventHandlerPlayer? TakedСardsFromDeck;
    public event EventHandlerPlayer? MakeMoved;
    public event EventHandlerPlayer? BeatedCard;
    public event EventHandlerPlayer? TakedEnemyCards;

    public string Name { get; }
    public IEnumerable<Card> Cards { get; }

    public void TakeСardsFromDeck(Stack<Card> cardsDeck, int numberOfCards);
    public void MakeMove(Card cardToMove);
    public void BeatCard(Card cardToBeat);
    public void TakeEnemyCards(IEnumerable<Card> enemyCards);
}

internal delegate void EventHandlerPlayer();