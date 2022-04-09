using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BallsGame
{
    public partial class Form1 : Form
    {
        static string[] colors = new string[] { "Red", "White", "Orange", "Yellow", "Brown" };
        static List<string> avaliableColors = new List<string>();
        static string[,] positions = new string[5, 6];
        static int extraRowPosition = 2;

        static bool paused = false;
        static int time = 0;

        static int moves = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void ShuffleButtons()
        {
            avaliableColors.Clear();
            positions = new string[5, 6];
            extraRowPosition = 2;
            positions[0, 5] = "E";
            positions[1, 5] = "E";
            positions[2, 5] = "E";
            positions[3, 5] = "E";
            positions[4, 5] = "E";
            gameTimer.Start();
            time = 0;
            moves = 0;
            movesLabel.Text = "Moves: 0";
            timeLabel.Text = "00:00";

            for (int i = 0; i < 5; i++)
            {
                for (int k = 0; k < 5; k++)
                {
                    avaliableColors.Add(colors[i]);
                }
            }

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    string buttonName = "btn" + y + x;
                    Button btn = mainPanel.Controls[buttonName] as Button;
                    int randomValue = new Random().Next(0, avaliableColors.Count);
                    string value = avaliableColors[randomValue];
                    switch (value)
                    {
                        case "Red":
                            btn.Image = Properties.Resources.RedBall;
                            break;
                        case "White":
                            btn.Image = Properties.Resources.WhiteBall;
                            break;
                        case "Brown":
                            btn.Image = Properties.Resources.BrownBall;
                            break;
                        case "Yellow":
                            btn.Image = Properties.Resources.YellowBall;
                            break;
                        case "Orange":
                            btn.Image = Properties.Resources.OrangeBall;
                            break;
                    }
                    positions[y, x] = value;
                    avaliableColors.RemoveAt(randomValue);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximumSize = new Size(390, 430);
            startPanel.Location = new Point((this.Width - startPanel.Width) / 2, (this.Height - startPanel.Height) / 2);
            endPanel.Location = new Point((this.Width - endPanel.Width) / 2, (this.Height - endPanel.Height) / 2);
            row0left.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            row1left.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            row2left.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            row3left.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            row4left.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            extraRowLeft.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
            ShuffleButtons();
        }

        private void Rowleft_Click(object sender, EventArgs e)
        {
            if (paused) return;

            Button btn = sender as Button;
            int row = int.Parse(btn.Name[3].ToString());
            string temp = positions[0, row];
            Button button = mainPanel.Controls["btn" + 0 + row] as Button;
            var imagePath = button.Image;
            for (int i = 0; i < 4; i++)
            {
                positions[i, row] = positions[i + 1, row];
                Button currentButton = mainPanel.Controls["btn" + i + row] as Button;
                Button nextButton = mainPanel.Controls["btn" + (i + 1) + row] as Button;
                currentButton.Image = nextButton.Image;
            }
            positions[4, row] = temp;
            Button lastButton = mainPanel.Controls["btn" + 4 + row] as Button;
            lastButton.Image = imagePath;
            UpdateMoves();
        }
        private void Rowright_Click(object sender, EventArgs e)
        {
            if (paused) return;
            Button btn = sender as Button;
            int row = int.Parse(btn.Name[3].ToString());
            string temp = positions[4, row];
            Button button = mainPanel.Controls["btn" + 4 + row] as Button;
            var imagePath = button.Image;
            for (int i = 4; i > 0; i--)
            {
                positions[i, row] = positions[i - 1, row];
                Button currentButton = mainPanel.Controls["btn" + i + row] as Button;
                Button nextButton = mainPanel.Controls["btn" + (i - 1) + row] as Button;
                currentButton.Image = nextButton.Image;
            }
            positions[0, row] = temp;
            Button lastButton = mainPanel.Controls["btn" + 0 + row] as Button;
            lastButton.Image = imagePath;
            UpdateMoves();
        }

        private void ExtraRowLeft_Click(object sender, EventArgs e)
        {
            if (paused) return;
            if (extraRowPosition < 1) return;
            btnExtra.Left -= 50;
            if (positions[extraRowPosition, 5] != "E")
            {
                positions[extraRowPosition - 1, 5] = positions[extraRowPosition, 5];
                positions[extraRowPosition, 5] = "E";
            }
            extraRowPosition -= 1;
            UpdateMoves();
        }

        private void ExtraRowRight_Click(object sender, EventArgs e)
        {
            if (paused) return;
            if (extraRowPosition > 3) return;
            btnExtra.Left += 50;
            if (positions[extraRowPosition, 5] != "E")
            {
                positions[extraRowPosition + 1, 5] = positions[extraRowPosition, 5];

                positions[extraRowPosition, 5] = "E";
            }
            extraRowPosition += 1;
            UpdateMoves();
        }

        private void MoveBallVertically(object sender, EventArgs e)
        {
            if (paused) return;
            Button button = sender as Button;
            int column;
            int row;
            if (button.Name == "btnExtra")
            {
                column = extraRowPosition;
                row = 5;
            }
            else
            {
                column = int.Parse(button.Name[3].ToString());
                row = int.Parse(button.Name[4].ToString());
            }

            //Try moving up
            if (row != 0)
            {
                bool canMoveUp = false;
                int moveUpPosition = 0;

                for (int i = row; i >= 0; i--)
                {
                    if (positions[column, i] == "E")
                    {
                        canMoveUp = true;
                        moveUpPosition = i;
                    }
                }

                if (canMoveUp)
                {
                    for (int i = moveUpPosition; i < row; i++)
                    {
                        positions[column, i] = positions[column, i + 1];
                        Button currentButton = mainPanel.Controls["btn" + column + i] as Button;
                        Button nextButton = mainPanel.Controls["btn" + column + (i + 1)] as Button;
                        if (i + 1 == 5)
                        {
                            nextButton = mainPanel.Controls["btnExtra"] as Button;
                        }
                        currentButton.Image = nextButton.Image;
                    }
                    button.Image = null;
                    positions[column, row] = "E";

                    if (checkForVictory())
                    {
                        //Victory
                        endMoves.Text = $"Moves: {moves}";

                        TimeSpan timerTime = TimeSpan.FromSeconds(time);
                        if (time < 3600)
                            endTime.Text = string.Format("{0:D2}:{1:D2}", timerTime.Minutes, timerTime.Seconds);
                        else
                            endTime.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", timerTime.Hours, timerTime.Minutes, timerTime.Seconds);

                        int score = ((1000 / time) + (1000 / moves)) * 25/10;
                        endScore.Text = $"Score: {score}";

                        endPanel.Visible = true;
                        mainPanel.Visible = false;
                        gameTimer.Stop();
                    }

                    UpdateMoves();
                    return;
                }
            }

            //Try moving down
            if (row != 5)
            {
                bool canMoveDown = false;
                int moveDownPosition = 0;

                int maxPosition = 5;
                if (column != extraRowPosition) maxPosition--;
                for (int i = row; i <= maxPosition; i++)
                {
                    if (positions[column, i] == "E")
                    {
                        canMoveDown = true;
                        moveDownPosition = i;
                        break;
                    }
                }

                if (canMoveDown)
                {
                    for (int i = moveDownPosition; i > row; i--)
                    {
                        positions[column, i] = positions[column, i - 1];

                        Button currentButton = mainPanel.Controls["btn" + column + i] as Button;
                        if (i == 5)
                        {
                            currentButton = mainPanel.Controls["btnExtra"] as Button;
                        }

                        Button nextButton = mainPanel.Controls["btn" + column + (i - 1)] as Button;
                        currentButton.Image = nextButton.Image;
                    }
                    button.Image = null;
                    positions[column, row] = "E";

                    UpdateMoves();
                    return;
                }
            }
        }

        private bool checkForVictory()
        {
            for (int i = 0; i < 5; i++)
            {
                string firstColor = positions[i, 0];
                for (int k = 1; k < 5; k++)
                {
                    if (positions[i, k] != firstColor)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            startPanel.Visible = false;
            ShuffleButtons();
            mainPanel.Visible = true;
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            paused = !paused;
            if (paused)
            {
                pauseBtn.Image = Properties.Resources.PlayButton;
                gameTimer.Enabled = false;
                pausedLabel.Visible = true;
            }
            else
            {
                pauseBtn.Image = Properties.Resources.PauseButton;
                gameTimer.Enabled = true;
                pausedLabel.Visible = false;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            time += 1;
            TimeSpan timerTime = TimeSpan.FromSeconds(time);
            if (time < 3600)
                timeLabel.Text = string.Format("{0:D2}:{1:D2}", timerTime.Minutes, timerTime.Seconds);
            else
                timeLabel.Text = string.Format("{0:D2}:{1:D2}:{2:D2}", timerTime.Hours, timerTime.Minutes, timerTime.Seconds);
        }

        private void UpdateMoves()
        {
            movesLabel.Text = $"Moves: {++moves}";
        }

        private void continueBtn_Click(object sender, EventArgs e)
        {
            endPanel.Visible = false;
            startPanel.Visible = true;
        }
    }
}
