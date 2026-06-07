using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TerminalChess;

namespace Chess_UI
{
    /// <summary>
    /// Interaction logic for Promotion.xaml
    /// </summary>
    public partial class Promotion : Window
    {
        public Piece? SelectedPiece = null;
        public Promotion(bool isWhite)
        {
            InitializeComponent();

            var knight = new Knight(isWhite);
            var bishop = new Bishop(isWhite);
            var rook = new Rook(isWhite);
            var queen = new Queen(isWhite);

            KnightBtn.Tag = knight;
            KnightBtn.Content = knight.PieceChar;

            BishopBtn.Tag = bishop;
            BishopBtn.Content = bishop.PieceChar;

            RookBtn.Tag = rook;
            RookBtn.Content = rook.PieceChar;

            QueenBtn.Tag = queen;
            QueenBtn.Content = queen.PieceChar;
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            SelectedPiece = (Piece)((Button)sender).Tag;
            DialogResult = true;
        }
    }
}
