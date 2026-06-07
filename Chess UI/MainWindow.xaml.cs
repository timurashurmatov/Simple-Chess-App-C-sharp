using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TerminalChess;

namespace Chess_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button[,] UiSquares = new Button[8, 8];
        private (int y, int x)? selectedSquare = null;
        private Board board = new Board();
        private int moveCount = 0;
        private bool whiteAtBottom = true;

        public MainWindow()
        {
            InitializeComponent();
            InitializeUI();
            DrawBoard();
        }

        private void InitializeUI()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var btn = new Button();

                    int cx = x;
                    int cy = y;

                    btn.Click += (s, e) => SquareClicked(cx, cy);
                    
                    ChessGrid.Children.Add(btn);
                    UiSquares[y, x] = btn;
                }
            }
        }

        private (int bx, int by) UiToBoard(int x, int y)
        {
            return whiteAtBottom ? (x, y) : (7 - x, 7 - y);
        }

        private void SquareClicked(int x, int y)
        {
            var (bx, by) = UiToBoard(x, y);
            bool moved = false;

            if (bx < 0 || bx > 7 || by < 0 || by > 7) return;

            if (selectedSquare == null)
            {
                if (board.BoardArr[by, bx] != null && board.BoardArr[by, bx].IsWhite == (moveCount % 2 == 0))
                {
                    selectedSquare = (by, bx);
                    DrawBoard();
                }
            }
            else if (board.BoardArr[by, bx] != null && board.BoardArr[by, bx].IsWhite == (moveCount % 2 == 0))
            {
                selectedSquare = (by, bx);
                DrawBoard();
            }
            else
            {
                var from = selectedSquare.Value;
                var move = new Move(new Position(from.x, from.y), new Position(bx, by));
                var piece = board.BoardArr[from.y, from.x];

                if (piece is King)
                {
                    if (!piece.IsWhite && move.From.Y == 0 && move.From.X == 4)
                    {
                        if (move.To.Y == 0)
                        {
                            if (move.To.X == 6)
                            {
                                board.Castle("O-O", piece.IsWhite);
                                moved = true;
                            }
                            else if (move.To.X == 2)
                            {
                                board.Castle("O-O-O", piece.IsWhite);
                                moved = true;
                            }
                        }
                    }
                    else if (piece.IsWhite && move.From.Y == 7 && move.From.X == 4)
                    {
                        if (move.To.Y == 7)
                        {
                            if (move.To.X == 6)
                            {
                                board.Castle("O-O", piece.IsWhite);
                                moved = true;
                            }
                            else if (move.To.X == 2)
                            {
                                board.Castle("O-O-O", piece.IsWhite);
                                moved = true;
                            }
                        }
                    }
                }

                if (!moved)
                    moved = board.ApplyMove(move, piece.IsWhite, moveCount);

                if (moved)
                {
                    moveCount++;
                    whiteAtBottom = !whiteAtBottom;

                    if (board.BackRankPawn(piece.IsWhite))
                    {
                        var dialog = new Promotion(piece.IsWhite);
                        dialog.Owner = this;

                        if (dialog.ShowDialog() == true)
                        {
                            Piece Promotion = dialog.SelectedPiece;
                            board.Promote(Promotion, piece.IsWhite, new Position(bx, by));
                        }
                    }

                    DrawBoard();
                }
                else
                {
                    DrawBoard();
                }

                selectedSquare = null;
            }
        }

        private void DrawBoard()
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    var (bx, by) = UiToBoard(x, y);

                    var piece = board.BoardArr[by, bx];
                    UiSquares[y, x].Background = ((x + y) % 2 == 0) ? Brushes.Beige : Brushes.Brown;
                    UiSquares[y, x].FontSize = 60;
                    if (piece == null)
                        UiSquares[y, x].Content = null;
                    else
                        UiSquares[y, x].Content = piece.PieceChar;

                    if (selectedSquare != null && selectedSquare.Value.y == by && selectedSquare.Value.x == bx)
                    {
                        UiSquares[y, x].Background = Brushes.LightBlue;
                    }
                }
            }

            var kingPos = board.FindKing(moveCount % 2 == 0, board.BoardArr);

            /*
            if (board.IsKingInCheck(kingPos, moveCount % 2 == 0, board.BoardArr))
            {
                var (bx, by) = UiToBoard(kingPos.X, kingPos.Y);
                UiSquares[by, bx].Foreground = Brushes.Red;
            }
            else
            {
                var (bx, by) = UiToBoard(kingPos.X, kingPos.Y);
                UiSquares[by, bx].Foreground = Brushes.Black;
            }
            */


            if (board.IsItMate(moveCount % 2 == 0))
            {
                var message = new Checkmate(moveCount % 2 != 0);
                message.Owner = this;
                message.ShowDialog();

                if (message.DialogResult == true)
                {
                    Application.Current.Shutdown();
                }
            }
        }
    }
}