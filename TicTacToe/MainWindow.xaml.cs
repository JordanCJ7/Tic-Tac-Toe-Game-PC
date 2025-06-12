using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        private char[,] board = null!;
        private char currentPlayer = 'X';
        private bool gameEnded = false;
        private int boardSize;
        private int winLength;

        public MainWindow()
        {
            InitializeComponent();
            
            // Explicitly set initial visibility
            StartScreen.Visibility = Visibility.Visible;
            GameScreen.Visibility = Visibility.Collapsed;
        }

        #region Game Setup Methods

        private void Board3x3_Click(object sender, RoutedEventArgs e)
        {
            StartGame(3, 3); // 3x3 board with 3 in a row to win
        }

        private void Board4x4_Click(object sender, RoutedEventArgs e)
        {
            StartGame(4, 4); // 4x4 board with 4 in a row to win
        }

        private void Board5x5_Click(object sender, RoutedEventArgs e)
        {
            StartGame(5, 4); // 5x5 board with 4 in a row to win
        }

        private void Board6x6_Click(object sender, RoutedEventArgs e)
        {
            StartGame(6, 5); // 6x6 board with 5 in a row to win
        }

        private void StartGame(int size, int winCondition)
        {
            boardSize = size;
            winLength = winCondition;
            board = new char[boardSize, boardSize];
            
            // Initialize the game
            InitializeGameBoard();
            InitializeBoard();
            
            // Update UI
            GameInfo.Text = "Player X's Turn";
            StartScreen.Visibility = Visibility.Collapsed;
            GameScreen.Visibility = Visibility.Visible;
        }

        private void InitializeGameBoard()
        {
            // Set up UniformGrid dimensions
            GameGrid.Rows = boardSize;
            GameGrid.Columns = boardSize;
            GameGrid.Children.Clear();
            
            // Calculate button size (smaller for larger boards)
            double buttonSize = 300.0 / boardSize;
            
            // Create buttons
            for (int i = 0; i < boardSize * boardSize; i++)
            {
                var button = new Button { 
                    FontSize = 32 / (boardSize / 3.0), 
                    Tag = i,
                    Width = buttonSize,
                    Height = buttonSize,
                    Margin = new Thickness(2)
                };
                button.Click += Button_Click;
                GameGrid.Children.Add(button);
            }
        }

        private void InitializeBoard()
        {
            for (int i = 0; i < boardSize; i++)
                for (int j = 0; j < boardSize; j++)
                    board[i, j] = ' ';
            currentPlayer = 'X';
            gameEnded = false;
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            ResetGame();
        }

        private void ChangeSize_Click(object sender, RoutedEventArgs e)
        {
            GameScreen.Visibility = Visibility.Collapsed;
            StartScreen.Visibility = Visibility.Visible;
        }

        #endregion

        #region Game Logic Methods

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (gameEnded) return;
            if (sender is not Button button || button.Tag is not int index)
                return;
                
            int row = index / boardSize;
            int col = index % boardSize;
            
            if (board[row, col] != ' ')
                return;
                
            board[row, col] = currentPlayer;
            button.Content = currentPlayer.ToString();
            
            // Set color based on player
            button.Foreground = currentPlayer == 'X' ? 
                new SolidColorBrush(Colors.DarkBlue) : 
                new SolidColorBrush(Colors.DarkRed);

            if (CheckWin(row, col))
            {
                GameInfo.Text = $"Player {currentPlayer} wins!";
                MessageBox.Show($"Player {currentPlayer} wins!");
                gameEnded = true;
            }
            else if (IsBoardFull())
            {
                GameInfo.Text = "It's a draw!";
                MessageBox.Show("It's a draw!");
                gameEnded = true;
            }
            else
            {
                currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
                GameInfo.Text = $"Player {currentPlayer}'s Turn";
            }
        }

        private bool CheckWin(int lastMoveRow, int lastMoveCol)
        {
            // We only need to check lines that include the last move
            return CheckHorizontal(lastMoveRow) || 
                   CheckVertical(lastMoveCol) || 
                   CheckDiagonals(lastMoveRow, lastMoveCol);
        }

        private bool CheckHorizontal(int row)
        {
            for (int startCol = 0; startCol <= boardSize - winLength; startCol++)
            {
                bool win = true;
                for (int i = 0; i < winLength; i++)
                {
                    if (board[row, startCol + i] != currentPlayer)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) return true;
            }
            return false;
        }

        private bool CheckVertical(int col)
        {
            for (int startRow = 0; startRow <= boardSize - winLength; startRow++)
            {
                bool win = true;
                for (int i = 0; i < winLength; i++)
                {
                    if (board[startRow + i, col] != currentPlayer)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) return true;
            }
            return false;
        }

        private bool CheckDiagonals(int row, int col)
        {
            // Check diagonal (top-left to bottom-right)
            for (int offset = -Math.Min(row, col); offset <= Math.Min(boardSize - 1 - row, boardSize - 1 - col) - (winLength - 1); offset++)
            {
                bool win = true;
                for (int i = 0; i < winLength; i++)
                {
                    if (row + offset + i >= boardSize || col + offset + i >= boardSize || 
                        board[row + offset + i, col + offset + i] != currentPlayer)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) return true;
            }

            // Check anti-diagonal (top-right to bottom-left)
            for (int offset = -Math.Min(row, boardSize - 1 - col); offset <= Math.Min(boardSize - 1 - row, col) - (winLength - 1); offset++)
            {
                bool win = true;
                for (int i = 0; i < winLength; i++)
                {
                    if (row + offset + i >= boardSize || col - offset - i < 0 || 
                        board[row + offset + i, col - offset - i] != currentPlayer)
                    {
                        win = false;
                        break;
                    }
                }
                if (win) return true;
            }

            return false;
        }

        private bool IsBoardFull()
        {
            for (int i = 0; i < boardSize; i++)
                for (int j = 0; j < boardSize; j++)
                    if (board[i, j] == ' ')
                        return false;
            return true;
        }

        private void ResetGame()
        {
            InitializeBoard();
            foreach (Button btn in GameGrid.Children)
                btn.Content = string.Empty;
            GameInfo.Text = "Player X's Turn";
        }

        #endregion
    }
}
