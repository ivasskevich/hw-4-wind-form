namespace hw_4_wind_form
{
    public partial class Form1 : Form
    {
        private Button[,] gameBoard;
        private bool isPlayerTurn;
        private bool gameFinished;
        private const int ButtonFontSize = 50;

        public Form1()
        {
            InitializeComponent();
            ConfigureGameSettings();
            BindMenuAndToolbarEvents();
            SetDefaultConfiguration();
        }

        private void ConfigureGameSettings()
        {
            gameBoard = new Button[3, 3]
            {
                { button1, button2, button3 },
                { button4, button5, button6 },
                { button7, button8, button9 }
            };

            foreach (var button in gameBoard)
            {
                button.Font = new Font("Arial", ButtonFontSize, FontStyle.Bold);
                button.TextAlign = ContentAlignment.MiddleCenter;
                button.Enabled = false;
                button.Click += OnPlayerMove;
            }
        }

        private void BindMenuAndToolbarEvents()
        {
            startToolStripMenuItem.Click += (s, e) => StartNewGame();
            closeToolStripMenuItem.Click += (s, e) => Close();

            computerFirstToolStripMenuItem.Click += (s, e) =>
            {
                computerFirstToolStripMenuItem.Checked = !computerFirstToolStripMenuItem.Checked;
                toolStripButton2.Checked = computerFirstToolStripMenuItem.Checked;
            };

            easyToolStripMenuItem.Click += (s, e) => SelectGameMode(isEasy: true);
            hardToolStripMenuItem.Click += (s, e) => SelectGameMode(isEasy: false);

            toolStripButton1.Click += (s, e) => StartNewGame();
            toolStripButton2.Click += (s, e) => computerFirstToolStripMenuItem.PerformClick();

            toolStripSplitButton1.DropDownItemClicked += (s, e) =>
            {
                if (e.ClickedItem == toolStripMenuItem2) SelectGameMode(isEasy: true);
                if (e.ClickedItem == toolStripMenuItem3) SelectGameMode(isEasy: false);
            };
        }

        private void SetDefaultConfiguration()
        {
            SelectGameMode(isEasy: true);
        }

        private void SelectGameMode(bool isEasy)
        {
            easyToolStripMenuItem.Checked = isEasy;
            hardToolStripMenuItem.Checked = !isEasy;

            toolStripMenuItem2.Checked = isEasy;
            toolStripMenuItem3.Checked = !isEasy;
        }

        private void StartNewGame()
        {
            ResetGameBoard();

            isPlayerTurn = !computerFirstToolStripMenuItem.Checked;
            gameFinished = false;

            if (!isPlayerTurn)
            {
                MakeComputerMove();
            }
        }

        private void ResetGameBoard()
        {
            foreach (var button in gameBoard)
            {
                button.Text = string.Empty;
                button.Enabled = true;
            }
        }

        private void OnPlayerMove(object sender, EventArgs e)
        {
            if (gameFinished) return;

            Button selectedButton = sender as Button;
            if (selectedButton != null && string.IsNullOrWhiteSpace(selectedButton.Text))
            {
                selectedButton.Text = "X";

                if (CheckForWinner("X"))
                {
                    EndGame("You WIN!");
                    return;
                }

                if (IsBoardFull())
                {
                    EndGame("Draw!");
                    return;
                }

                isPlayerTurn = false;
                MakeComputerMove();
            }
        }

        private void MakeComputerMove()
        {
            if (gameFinished) return;

            if (easyToolStripMenuItem.Checked)
            {
                ExecuteEasyMove();
            }
            else if (hardToolStripMenuItem.Checked)
            {
                ExecuteHardMove();
            }

            if (CheckForWinner("O"))
            {
                EndGame("Computer has won!");
            }
            else if (IsBoardFull())
            {
                EndGame("Draw!");
            }
            else
            {
                isPlayerTurn = true;
            }
        }

        private void ExecuteEasyMove()
        {
            var availableButtons = gameBoard.Cast<Button>()
                .Where(b => string.IsNullOrWhiteSpace(b.Text))
                .ToList();

            if (availableButtons.Any())
            {
                var random = new Random();
                Button randomMove = availableButtons[random.Next(availableButtons.Count)];
                randomMove.Text = "O";
            }
        }

        private void ExecuteHardMove()
        {
            Button bestMove = FindOptimalMove();
            if (bestMove != null)
            {
                bestMove.Text = "O";
            }
        }

        private Button FindOptimalMove()
        {
            foreach (var button in gameBoard)
            {
                if (string.IsNullOrWhiteSpace(button.Text))
                {
                    button.Text = "O";
                    if (CheckForWinner("O"))
                    {
                        button.Text = string.Empty;
                        return button;
                    }

                    button.Text = "X";
                    if (CheckForWinner("X"))
                    {
                        button.Text = string.Empty;
                        return button;
                    }

                    button.Text = string.Empty;
                }
            }

            var availableButtons = gameBoard.Cast<Button>()
                .Where(b => string.IsNullOrWhiteSpace(b.Text))
                .ToList();

            if (availableButtons.Any())
            {
                var random = new Random();
                return availableButtons[random.Next(availableButtons.Count)];
            }

            return null;
        }

        private bool CheckForWinner(string symbol)
        {
            for (int row = 0; row < 3; row++)
            {
                if (gameBoard[row, 0].Text == symbol &&
                    gameBoard[row, 1].Text == symbol &&
                    gameBoard[row, 2].Text == symbol)
                    return true;
            }

            for (int col = 0; col < 3; col++)
            {
                if (gameBoard[0, col].Text == symbol &&
                    gameBoard[1, col].Text == symbol &&
                    gameBoard[2, col].Text == symbol)
                    return true;
            }

            if (gameBoard[0, 0].Text == symbol &&
                gameBoard[1, 1].Text == symbol &&
                gameBoard[2, 2].Text == symbol)
                return true;

            if (gameBoard[0, 2].Text == symbol &&
                gameBoard[1, 1].Text == symbol &&
                gameBoard[2, 0].Text == symbol)
                return true;

            return false;
        }

        private bool IsBoardFull()
        {
            return gameBoard.Cast<Button>().All(button => !string.IsNullOrWhiteSpace(button.Text));
        }

        private void EndGame(string message)
        {
            gameFinished = true;
            MessageBox.Show(message, "Game Over");

            foreach (var button in gameBoard)
            {
                button.Enabled = false;
            }
        }
    }
}