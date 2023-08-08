using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CardGameFool.UI
{
    /// <summary>
    /// Логика взаимодействия для PlayerCards.xaml
    /// </summary>
    public partial class PlayerCards : UserControl
    {
        private const int _maxCardsCount = 6;

        private static readonly int[] _angles = { -65, -40, -15, 15, 40, 65 };
        private static readonly Point _transformOrigin = new(0.5, 1);

        private readonly CardUI?[] _cards = new CardUI[_maxCardsCount];

        public PlayerCards()
        {
            InitializeComponent();
        }

        public void AddCart(CardUI card, int positionIndex)
        {
            if (positionIndex < 0 || positionIndex > _maxCardsCount - 1)
            {
                throw new ArgumentException(
                    $"The index entered is greater than the maximum available({_maxCardsCount - 1}) or less than zero.");
            }

            if (_cards[positionIndex] is not null)
            {
                throw new InvalidOperationException("There is already a card in this position.");
            }

            card.Rotate = _angles[positionIndex];
            card.RenderTransformOrigin = _transformOrigin;

            _cards[positionIndex] = card;

            Workspace.Children.Add(card);
        }

        public void RemoveCard(int positionIndex)
        {
            if (positionIndex < 0 || positionIndex > _cards.Length - 1)
            {
                throw new ArgumentException($"The entered index is larger than the available one or less than zero.");
            }

            if (_cards[positionIndex] is null)
            {
                throw new InvalidOperationException("There is no card in this position.");
            }

            Workspace.Children.Remove(_cards[positionIndex]);

            _cards[positionIndex] = null;
        }

        private void CardUI_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((CardUI)sender).Side == CardSides.Shirt)
                return;

            MessageBox.Show("dsds");
        }
    }
}
