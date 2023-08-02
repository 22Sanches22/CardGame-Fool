using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGame_Fool.Model
{
    public class Deck
    {
        private const int _suitsCount = 4;
        private const int _ranksCount = 9;
        private const int _maxCardsCount = _suitsCount * _ranksCount;

        private readonly List<Card> _cards = new(_maxCardsCount);

        private readonly Suits _trumpSuit;

        public Deck()
        {
            _trumpSuit = (Suits)new Random().Next(_suitsCount);

            GenerateCards();
            ShuffleCards();

            MoveRandomTrumpToEndList();
        }

        public int CardsCount => _cards.Count;

        public Card GetTopCard()
        {
            Card topCard = _cards[0];

            _cards.Remove(topCard);

            return topCard;
        }

        private void ShuffleCards()
        {
            Random random = new();

            for (int i = 0; i < _maxCardsCount; i++)
            {
                int index1 = random.Next(0, _maxCardsCount);
                int index2 = random.Next(0, _maxCardsCount);

                (_cards[index1], _cards[index2]) = (_cards[index2], _cards[index1]);
            }
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

        private void MoveRandomTrumpToEndList()
        {
            Card[] allTrump = _cards.Where(card => card.IsTrump).ToArray();

            Card trumpCard = allTrump[new Random().Next(allTrump.Length)];

            _cards.Remove(trumpCard);
            _cards.Add(trumpCard);
        }
    }
}