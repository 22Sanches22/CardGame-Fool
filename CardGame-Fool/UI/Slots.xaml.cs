using CardGameFool.Model.Players;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CardGameFool.UI;

/// <summary>
/// Логика взаимодействия для Slots.xaml
/// </summary>
public partial class Slots : UserControl
{
    private const int _slotCount = Player.StartingCardsCount;

    private static readonly int[] _leftSlotsPosition = { 0, 100, 200, 300, 400, 500 };

    private readonly CardUI?[] _attackingCards = new CardUI?[_slotCount];
    private readonly CardUI?[] _defenseCards = new CardUI?[_slotCount];

    public Slots()
    {
        InitializeComponent();
    }

    /// <returns> Last free defense slot index or -1 if all slots are occupied. </returns>
    public int LastFreeAttackingSlotIndex
    {
        get
        {
            return Array.IndexOf(_attackingCards, null);
        }
    }

    /// <returns> Last free attacking slot index or -1 if all slots are occupied. </returns>
    public int LastFreeDefenseSlotIndex
    {
        get
        {
            return Array.IndexOf(_defenseCards, null);
        }
    }

    public void Put(CardUI card, int slotIndex, SlotTypes slotType)
    {
        if (slotIndex < 0 || slotIndex > _slotCount - 1)
        {
            throw new ArgumentException(
                $"The index entered is greater than the maximum available({_slotCount - 1}) or less than zero.");
        }

        int leftPos = _leftSlotsPosition[slotIndex];
        int topPos = 0;

        if (slotType == SlotTypes.Attacking)
        {
            card.Rotate = 0;

            _attackingCards[slotIndex] = card;
        }
        else if (slotType == SlotTypes.Defense)
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

        card.Cursor = Cursors.Arrow;

        Canvas.SetLeft(card, leftPos);
        Canvas.SetTop(card, topPos);

        WorkspaceCanvas.Children.Add(card);
    }

    public void Remove(int slotIndex, SlotTypes slotType)
    {
        if (slotType == SlotTypes.Attacking)
        {
            if (_defenseCards[slotIndex] is not null)
            {
                throw new InvalidOperationException("First you need to remove the defense card in this position.");
            }

            ClearSlot(_attackingCards);
        }
        else if (slotType == SlotTypes.Defense)
        {
            ClearSlot(_defenseCards);
        }


        void ClearSlot(CardUI?[] cards)
        {
            if (cards[slotIndex] is null)
            {
                throw new InvalidOperationException("There is no card in this position.");
            }

            WorkspaceCanvas.Children.Remove(cards[slotIndex]);

            cards[slotIndex] = null;
        }
    }

    public void Clear()
    {
        for (int i = 0; i < _slotCount; i++)
        {
            if (_defenseCards[i] is not null)
            {
                Remove(i, SlotTypes.Defense);
            }

            if (_attackingCards[i] is not null)
            {
                Remove(i, SlotTypes.Attacking);
            }
        }        
    }
}