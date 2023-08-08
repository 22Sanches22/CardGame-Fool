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

        Deck deck = new();

        Fool gameFool = new(player, player1, deck);



        CardsSlots.PutCard(new CardUI(deck.TrumpCard), 5, SlotsType.Attacking);
        CardsSlots.PutCard(new CardUI(deck.TrumpCard), 5, SlotsType.Defense);

        Player1Cards.AddCart(new CardUI(deck.TrumpCard), 0);
        Player1Cards.AddCart(new CardUI(deck.TrumpCard), 1);
        Player1Cards.AddCart(new CardUI(deck.TrumpCard), 2);
        Player1Cards.AddCart(new CardUI(deck.TrumpCard), 3);
        Player1Cards.AddCart(new CardUI(deck.TrumpCard), 4);
        Player1Cards.AddCart(new CardUI(deck.TrumpCard), 5);

        Player2Cards.AddCart(new CardUI(deck.TrumpCard) { Side = CardSides.Shirt }, 5);
        Player2Cards.AddCart(new CardUI(deck.TrumpCard) { Side = CardSides.Shirt }, 4);
        Player2Cards.AddCart(new CardUI(deck.TrumpCard) { Side = CardSides.Shirt }, 3);
        Player2Cards.AddCart(new CardUI(deck.TrumpCard) { Side = CardSides.Shirt }, 2);
        Player2Cards.AddCart(new CardUI(deck.TrumpCard) { Side = CardSides.Shirt }, 1);
        Player2Cards.AddCart(new CardUI(deck.TrumpCard) { Side = CardSides.Shirt }, 0);
        #region
        //double offsetX = 10;
        //double offsetY = 12;

        //Random random = new();

        //while (deck.Count > 0)
        //{
        //    CardUI cardUI = new(deck.GetTopCard())
        //    {
        //        HorizontalAlignment = HorizontalAlignment.Left,
        //        VerticalAlignment = VerticalAlignment.Top,
        //        Margin = new Thickness(offsetX, offsetY, 0, 0),
        //        Side = (CardSides)random.Next(2),
        //    };

        //    Workspace.Children.Add(cardUI);

        //    offsetX += 93;

        //    if (offsetX == (93 * 9 + 10))
        //    {
        //        offsetX = 10;
        //        offsetY += 120;
        //    }
        //}
        #endregion
    }
}