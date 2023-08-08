﻿using System;
using System.Windows.Controls;

namespace CardGameFool.UI
{
    /// <summary>
    /// Логика взаимодействия для Slots.xaml
    /// </summary>
    public partial class Slots : UserControl
    {
        private const int _slotCount = 6;

        private static readonly int[] _leftSlotsPosition = { 0, 100, 200, 300, 400, 500 };

        private readonly CardUI?[] _attackingCards = new CardUI?[_slotCount];
        private readonly CardUI?[] _defenseCards = new CardUI?[_slotCount];

        public Slots()
        {
            InitializeComponent();
        }

        public void PutCard(CardUI card, int slotIndex, SlotsType slotType)
        {
            if (slotIndex < 0 || slotIndex > _slotCount - 1)
            {
                throw new ArgumentException(
                    $"The index entered is greater than the maximum available({_slotCount - 1}) or less than zero.");
            }

            int leftPos = _leftSlotsPosition[slotIndex];
            int topPos = 0;

            if (slotType == SlotsType.Attacking)
            {
                _attackingCards[slotIndex] = card;
            }
            else if (slotType == SlotsType.Defense)
            {
                if (_attackingCards[slotIndex] is null)
                {
                    throw new InvalidOperationException("There is no card to beat in this position.");
                }

                _defenseCards[slotIndex] = card;

                const int leftPosIncrease = 3;
                const int topPosIncrease = 30;

                leftPos += leftPosIncrease;
                topPos += topPosIncrease;

                const int angle = 20;

                card.Rotate = angle;
            }

            Canvas.SetLeft(card, leftPos);
            Canvas.SetTop(card, topPos);

            Workspace.Children.Add(card);
        }

        public void RemoveCard(int slotIndex, SlotsType slotType)
        {
            if (slotType == SlotsType.Attacking)
            {
                if (_defenseCards[slotIndex] is not null)
                {
                    throw new InvalidOperationException("First you need to remove the defense card in this position.");
                }

                ClearSlot(_attackingCards);
            }
            else if (slotType == SlotsType.Defense)
            {
                ClearSlot(_defenseCards);
            }


            void ClearSlot(CardUI?[] cards)
            {
                if (cards[slotIndex] is null)
                {
                    throw new InvalidOperationException("There is no card in this position.");
                }

                Workspace.Children.Remove(cards[slotIndex]);

                cards[slotIndex] = null;
            }
        }
    }
}
