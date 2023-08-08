using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGameFool.Model
{
    public class Deck
    {
        public const int MaxCardsCount = 36;

        private const int _suitsCount = 4;
        private const int _ranksCount = 9;

        private readonly List<Card> _cards = new(MaxCardsCount);

        private readonly Suits _trumpSuit;

        public Deck()
        {
            _trumpSuit = (Suits)new Random().Next(_suitsCount);

            GenerateCards();
            ShuffleCards();

            ChoiceTrump();
        }

        /// <returns> The number of card in deck. </returns>
        public int Count => _cards.Count;

        public Card TrumpCard => _cards[^1];

        /// <returns> The first of card in deck. </returns>
        public Card GetTopCard()
        {
            if (Count < 1)
            {
                throw new InvalidOperationException("The deck is empty.");
            }

            Card topCard = _cards[0];

            _cards.Remove(topCard);

            return topCard;
        }

        private void GenerateCards()
        {
            for (int i = 0; i < _suitsCount; i++)
            {
                for (int j = 0; j < _ranksCount; j++)
                {
                    // The initial rank in the "Ranks" enum is six.
                    Ranks rank = j + Ranks.Six;

                    _cards.Add(new Card(rank, (Suits)i, _trumpSuit));
                }
            }
        }

        private void ShuffleCards()
        {
            Random random = new();

            for (int i = 0; i < MaxCardsCount; i++)
            {
                int index1 = random.Next(0, MaxCardsCount);
                int index2 = random.Next(0, MaxCardsCount);

                (_cards[index1], _cards[index2]) = (_cards[index2], _cards[index1]);
            }
        }

        /// <summary> 
        /// Choices a random trump card and places it at the end of the deck if the last card is not a trump card.
        /// </summary>
        private void ChoiceTrump()
        {
            if (_cards[^1].IsTrump)
            {
                return;
            }

            Card[] allTrump = _cards.Where(card => card.IsTrump).ToArray();
            Card trumpCard = allTrump[new Random().Next(allTrump.Length)];

            _cards.Remove(trumpCard);
            _cards.Add(trumpCard);
        }
    }
}