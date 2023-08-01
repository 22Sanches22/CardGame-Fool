using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame_Fool.GameFool
{
    public class Deck
    {
        private const int _suitsCount = 4;
        private const int _ranksCount = 9;
        private const int _cardsCount = _suitsCount * _ranksCount;

        private readonly Stack<Card> _cards;
        private readonly Dictionary<Card, int> _cardsImportance = new(_cardsCount);

        public Deck()
        {
            Card[] cardsDeck = GenerateCards();
            ShuffleCards(cardsDeck);
            
            _cards = new Stack<Card>(cardsDeck);

            Card trumpCard = cardsDeck[0];
            TrumpSuit = trumpCard.Suit;

            CalculateCardsImportance();
        }

        public Suits TrumpSuit { get; }

        public ReadOnlyDictionary<Card, int> CardsImportance => new(_cardsImportance);

        public int CardsCount => _cards.Count;
        
        public Card[] SortCardsCollection(ICollection<Card> playerCards)
        {
            Card[] cards = playerCards.ToArray();

            for (int i = 0; i < cards.Length; i++)
            {
                for (int j = 0; j < cards.Length - 1; j++)
                {
                    if (_cardsImportance[cards[j]] > _cardsImportance[cards[j + 1]])
                    {
                        (cards[j], cards[j + 1]) = (cards[j + 1], cards[j]);
                    }
                }
            }

            return cards;
        }

        public Card GetTopCard()
        {
            return _cards.Pop();
        }

        private static Card[] GenerateCards()
        {
            Card[] cardsDeck = new Card[_cardsCount];
 
            int cardNumber = 0;

            for (int i = 0; i < _suitsCount; i++)
            {
                for (int j = 0; j < _ranksCount; j++)
                {
                    // The initial rank in the "Ranks" enum is six.
                    cardsDeck[cardNumber++] = new Card((Suits)i, j + Ranks.Six);
                }
            }

            return cardsDeck;
        }

        private static void ShuffleCards(Card[] cardsDeck)
        {
            Random random = new();

            for (int i = 0; i < _cardsCount; i++)
            {
                int index1 = random.Next(0, _cardsCount);
                int index2 = random.Next(0, _cardsCount);

                (cardsDeck[index1], cardsDeck[index2]) = (cardsDeck[index2], cardsDeck[index1]);
            }
        }

        private void CalculateCardsImportance()
        {
            foreach (Card card in _cards)
            {
                int cardImportance = (int)card.Rank + (card.Suit == TrumpSuit ? 10 : 0);
                _cardsImportance.Add(card, cardImportance);
            }
        }
    }
}