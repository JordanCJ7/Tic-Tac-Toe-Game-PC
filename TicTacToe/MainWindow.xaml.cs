using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        private char[,] board = null!;
        private char currentPlayer = 'X';
        private bool gameEnded = false;
        private int boardSize;
        private int winLength;
        private bool isAIGame = false;
        private int aiDifficulty = 0; // 0: Easy, 1: Medium, 2: Hard

        public MainWindow()
        {
            InitializeComponent();
            
            // Explicitly set initial visibility
            GameModeScreen.Visibility = Visibility.Visible;
            BoardSizeScreen.Visibility = Visibility.Collapsed;
            GameScreen.Visibility = Visibility.Collapsed;
        }

        #region Navigation Methods
        
        private void PlayerVsPlayer_Click(object sender, RoutedEventArgs e)
        {
            isAIGame = false;
            GameModeScreen.Visibility = Visibility.Collapsed;
            BoardSizeScreen.Visibility = Visibility.Visible;
        }

        private void PlayerVsAI_Click(object sender, RoutedEventArgs e)
        {
            // Show AI difficulty options
            AIDifficultyText.Visibility = Visibility.Visible;
            AIDifficultyPanel.Visibility = Visibility.Visible;
        }

        private void AIEasy_Click(object sender, RoutedEventArgs e)
        {
            isAIGame = true;
            aiDifficulty = 0;
            GameModeScreen.Visibility = Visibility.Collapsed;
            BoardSizeScreen.Visibility = Visibility.Visible;
        }

        private void AIMedium_Click(object sender, RoutedEventArgs e)
        {
            isAIGame = true;
            aiDifficulty = 1;
            GameModeScreen.Visibility = Visibility.Collapsed;
            BoardSizeScreen.Visibility = Visibility.Visible;
        }

        private void AIHard_Click(object sender, RoutedEventArgs e)
        {
            isAIGame = true;
            aiDifficulty = 2;
            GameModeScreen.Visibility = Visibility.Collapsed;
            BoardSizeScreen.Visibility = Visibility.Visible;
        }

        private void BackToGameMode_Click(object sender, RoutedEventArgs e)
        {
            BoardSizeScreen.Visibility = Visibility.Collapsed;
            GameModeScreen.Visibility = Visibility.Visible;
            AIDifficultyText.Visibility = Visibility.Collapsed;
            AIDifficultyPanel.Visibility = Visibility.Collapsed;
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            GameScreen.Visibility = Visibility.Collapsed;
            GameModeScreen.Visibility = Visibility.Visible;
            AIDifficultyText.Visibility = Visibility.Collapsed;
            AIDifficultyPanel.Visibility = Visibility.Collapsed;
        }

        #endregion

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
            BoardSizeScreen.Visibility = Visibility.Collapsed;
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
                
            // Make player move
            MakeMove(row, col);
            
            // If AI game and player didn't win or draw, make AI move
            if (isAIGame && !gameEnded && currentPlayer == 'O')
            {
                MakeAIMove();
            }
        }

        private void MakeMove(int row, int col)
        {
            board[row, col] = currentPlayer;
            
            // Update the button
            int index = row * boardSize + col;
            Button button = (Button)GameGrid.Children[index];
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

        private void MakeAIMove()
        {
            // Delay to make AI move seem natural
            System.Threading.Thread.Sleep(500);
            
            // Choose AI move based on difficulty
            (int row, int col) = aiDifficulty switch
            {
                0 => GetEasyAIMove(),
                1 => GetMediumAIMove(),
                2 => GetHardAIMove(),
                _ => GetEasyAIMove()
            };
            
            MakeMove(row, col);
        }

        private (int, int) GetEasyAIMove()
        {
            // Easy: Random move
            var random = new Random();
            int row, col;
            
            do
            {
                row = random.Next(0, boardSize);
                col = random.Next(0, boardSize);
            } while (board[row, col] != ' ');
            
            return (row, col);
        }

        private (int, int) GetMediumAIMove()
        {
            // Medium: 50% chance of choosing the best move, 50% random
            var random = new Random();
            if (random.Next(2) == 0)
            {
                return GetHardAIMove(); // Choose best move
            }
            else
            {
                return GetEasyAIMove(); // Choose random move
            }
        }

        private (int, int) GetHardAIMove()
        {
            // Hard: Try to win, block player, or make strategic move
            
            // First check if AI can win
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        board[i, j] = 'O'; // Try move
                        if (CheckWin(i, j))
                        {
                            board[i, j] = ' '; // Undo move
                            return (i, j); // Winning move
                        }
                        board[i, j] = ' '; // Undo move
                    }
                }
            }
            
            // Check if player can win and block
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    if (board[i, j] == ' ')
                    {
                        board[i, j] = 'X'; // Try player move
                        if (CheckWin(i, j))
                        {
                            board[i, j] = ' '; // Undo move
                            return (i, j); // Blocking move
                        }
                        board[i, j] = ' '; // Undo move
                    }
                }
            }
            
            // If center is empty, take it (for 3x3 board)
            if (boardSize == 3 && board[1, 1] == ' ')
            {
                return (1, 1);
            }
            
            // Take a corner if available (strategic for 3x3)
            if (boardSize == 3)
            {
                int[][] corners = new int[][] { new int[] { 0, 0 }, new int[] { 0, 2 }, new int[] { 2, 0 }, new int[] { 2, 2 } };
                var random = new Random();
                var shuffledCorners = corners.OrderBy(x => random.Next()).ToArray();
                
                foreach (var corner in shuffledCorners)
                {
                    if (board[corner[0], corner[1]] == ' ')
                    {
                        return (corner[0], corner[1]);
                    }
                }
            }
            
            // Otherwise, make a random move
            return GetEasyAIMove();
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
