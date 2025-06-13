using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

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
        
        // Game mode properties
        private enum GameMode { Classic, Timed }
        private GameMode currentGameMode = GameMode.Classic;
        private int timeLimitPerMove = 5; // 5 seconds per move
        private DispatcherTimer moveTimer = new DispatcherTimer();
        private int timeLeftForCurrentMove;
        private double totalTimeX = 0; // Total time taken by player X in seconds
        private double totalTimeO = 0; // Total time taken by player O in seconds
        private DateTime moveStartTime;

        public MainWindow()
        {
            InitializeComponent();
            
            // Explicitly set initial visibility
            GameModeScreen.Visibility = Visibility.Visible;
            BoardSizeScreen.Visibility = Visibility.Collapsed;
            GameModeSelectionScreen.Visibility = Visibility.Collapsed;
            GameScreen.Visibility = Visibility.Collapsed;
            
            // Initialize the timer
            moveTimer.Interval = TimeSpan.FromSeconds(1);
            moveTimer.Tick += MoveTimer_Tick;
        }

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            timeLeftForCurrentMove--;
            TimerInfo.Text = $"Time left: {timeLeftForCurrentMove} seconds";
            
            if (timeLeftForCurrentMove <= 0)
            {
                moveTimer.Stop();
                
                // Record time for the current player
                RecordTimeForCurrentPlayer();
                
                // Skip the player's turn if time runs out
                TimerInfo.Text = $"Time's up for Player {currentPlayer}!";
                currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
                GameInfo.Text = $"Player {currentPlayer}'s Turn";
                
                // Restart timer for next player
                StartMoveTimer();
                
                // If AI game and it's AI's turn, make AI move
                if (isAIGame && currentPlayer == 'O')
                {
                    _ = MakeAIMove();
                }
            }
        }

        private void RecordTimeForCurrentPlayer()
        {
            TimeSpan elapsed = DateTime.Now - moveStartTime;
            
            if (currentPlayer == 'X')
                totalTimeX += elapsed.TotalSeconds;
            else
                totalTimeO += elapsed.TotalSeconds;
        }
        
        private void StartMoveTimer()
        {
            if (currentGameMode == GameMode.Timed)
            {
                moveStartTime = DateTime.Now;
                timeLeftForCurrentMove = timeLimitPerMove;
                TimerInfo.Text = $"Time left: {timeLeftForCurrentMove} seconds";
                moveTimer.Start();
            }
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
            // Stop the timer if it's running
            if (moveTimer.IsEnabled)
            {
                moveTimer.Stop();
            }
            
            GameScreen.Visibility = Visibility.Collapsed;
            GameModeScreen.Visibility = Visibility.Visible;
            AIDifficultyText.Visibility = Visibility.Collapsed;
            AIDifficultyPanel.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Game Setup Methods

        private void Board3x3_Click(object sender, RoutedEventArgs e)
        {
            boardSize = 3;
            winLength = 3;
            BoardSizeScreen.Visibility = Visibility.Collapsed;
            GameModeSelectionScreen.Visibility = Visibility.Visible;
        }

        private void Board4x4_Click(object sender, RoutedEventArgs e)
        {
            boardSize = 4;
            winLength = 4;
            BoardSizeScreen.Visibility = Visibility.Collapsed;
            GameModeSelectionScreen.Visibility = Visibility.Visible;
        }

        private void Board5x5_Click(object sender, RoutedEventArgs e)
        {
            boardSize = 5;
            winLength = 4;
            BoardSizeScreen.Visibility = Visibility.Collapsed;
            GameModeSelectionScreen.Visibility = Visibility.Visible;
        }

        private void Board6x6_Click(object sender, RoutedEventArgs e)
        {
            boardSize = 6;
            winLength = 5;
            BoardSizeScreen.Visibility = Visibility.Collapsed;
            GameModeSelectionScreen.Visibility = Visibility.Visible;
        }
        
        private void ClassicMode_Click(object sender, RoutedEventArgs e)
        {
            currentGameMode = GameMode.Classic;
            StartGame();
        }

        private void TimedMode_Click(object sender, RoutedEventArgs e)
        {
            currentGameMode = GameMode.Timed;
            StartGame();
        }
        
        private void BackToBoardSize_Click(object sender, RoutedEventArgs e)
        {
            GameModeSelectionScreen.Visibility = Visibility.Collapsed;
            BoardSizeScreen.Visibility = Visibility.Visible;
        }

        private void StartGame()
        {
            board = new char[boardSize, boardSize];
            
            // Reset time tracking
            totalTimeX = 0;
            totalTimeO = 0;
            
            // Initialize the game
            InitializeGameBoard();
            InitializeBoard();
            
            // Update UI
            GameInfo.Text = "Player X's Turn";
            GameModeSelectionScreen.Visibility = Visibility.Collapsed;
            GameScreen.Visibility = Visibility.Visible;
            
            // Set up timer visibility
            if (currentGameMode == GameMode.Timed)
            {
                TimerInfo.Visibility = Visibility.Visible;
                TimerInfo.Text = $"Time left: {timeLimitPerMove} seconds";
                StartMoveTimer();
            }
            else
            {
                TimerInfo.Visibility = Visibility.Collapsed;
            }
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
            // Stop the timer if it's running
            if (moveTimer.IsEnabled)
            {
                moveTimer.Stop();
            }
            
            ResetGame();
            
            // Restart timer if in timed mode
            if (currentGameMode == GameMode.Timed)
            {
                totalTimeX = 0;
                totalTimeO = 0;
                TimerInfo.Text = $"Time left: {timeLimitPerMove} seconds";
                StartMoveTimer();
            }
        }

        #endregion

        #region Game Logic Methods

        private async void Button_Click(object sender, RoutedEventArgs e)
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
                await MakeAIMove();
            }
        }

        private void MakeMove(int row, int col)
        {
            // Stop timer if in timed mode
            if (currentGameMode == GameMode.Timed)
            {
                moveTimer.Stop();
                RecordTimeForCurrentPlayer();
            }
            
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
                if (currentGameMode == GameMode.Timed)
                {
                    MessageBox.Show($"Player {currentPlayer} wins!\nX total time: {totalTimeX:F1}s, O total time: {totalTimeO:F1}s");
                    TimerInfo.Text = $"X total time: {totalTimeX:F1}s, O total time: {totalTimeO:F1}s";
                }
                else
                {
                    MessageBox.Show($"Player {currentPlayer} wins!");
                }
                gameEnded = true;
            }
            else if (IsBoardFull())
            {
                // In timed mode, if it's a draw, the player who used less time wins
                if (currentGameMode == GameMode.Timed)
                {
                    string winnerByTime = totalTimeX < totalTimeO ? "X" : "O";
                    GameInfo.Text = $"Draw! Player {winnerByTime} wins by time!";
                    MessageBox.Show($"It's a draw! Player {winnerByTime} wins by using less time.\nX: {totalTimeX:F1}s, O: {totalTimeO:F1}s");
                    TimerInfo.Text = $"X total time: {totalTimeX:F1}s, O total time: {totalTimeO:F1}s";
                }
                else
                {
                    GameInfo.Text = "It's a draw!";
                    MessageBox.Show("It's a draw!");
                }
                gameEnded = true;
            }
            else
            {
                currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
                GameInfo.Text = $"Player {currentPlayer}'s Turn";
                
                // Start timer for next player if in timed mode
                if (currentGameMode == GameMode.Timed)
                {
                    StartMoveTimer();
                }
            }
        }

        private async System.Threading.Tasks.Task MakeAIMove()
        {
            // Delay to make AI move seem like thinking (1.2-2.2 seconds)
            var random = new Random();
            int thinkTime = random.Next(1200, 2200); // Random time between 1.2-2.2 seconds
            
            // Update UI to show AI is thinking
            GameInfo.Text = "AI is thinking...";
            
            // Use Task.Delay instead of Thread.Sleep to keep UI responsive
            await System.Threading.Tasks.Task.Delay(thinkTime);
            
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
            char originalPlayer = currentPlayer;
            
            // First check if AI can win
            currentPlayer = 'O'; // Temporarily set to AI
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
                            currentPlayer = originalPlayer; // Restore original player
                            return (i, j); // Winning move
                        }
                        board[i, j] = ' '; // Undo move
                    }
                }
            }
            
            // Check if player can win and block
            currentPlayer = 'X'; // Temporarily set to player
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
                            currentPlayer = originalPlayer; // Restore original player
                            return (i, j); // Blocking move
                        }
                        board[i, j] = ' '; // Undo move
                    }
                }
            }
            
            // Restore original player
            currentPlayer = originalPlayer;
            
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
            
            if (currentGameMode == GameMode.Timed)
            {
                StartMoveTimer();
            }
        }

        #endregion
    }
}
