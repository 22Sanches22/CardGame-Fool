using System;
using System.Windows;

using CardGameFool.Model;

namespace CardGameFool.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        BotPlayer player = new("player1");
        BotPlayer player1 = new("player2");

        Fool gameFool = new(player, player1);

        Deck deck = new();

        double offsetX = 10;
        double offsetY = 12;

        Random random = new();

        while (deck.Count > 0)
        {
            CardUI cardUI = new(deck.GetTopCard())
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(offsetX, offsetY, 0, 0),
                Side = (CardSides)random.Next(2),
            };

            Workspace.Children.Add(cardUI);

            offsetX += 93;

            if (offsetX == (93 * 9 + 10))
            {
                offsetX = 10;
                offsetY += 120;
            }
        }
    }
}