using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Diagnostics;

using SharedGameClasses;

namespace GuiGame {

    /// <summary>
    /// The form that displays the GUI of the game named HareAndTortoise.
    /// </summary>
    public partial class HareAndTortoiseForm : Form {

        // Specify the numbers of rows and columns on the screen.
        const int NUM_OF_ROWS = 7;
        const int NUM_OF_COLUMNS = 6;


        // When we update what's on the screen, we show the movement of players 
        // by removing them from their old squares and adding them to their new squares.
        // This enum makes it clearer that we need to do both.
        enum TypeOfGuiUpdate { AddPlayer, RemovePlayer };
    
        /// <summary>
        /// Constructor with initialising parameters.
        /// Pre:  none.
        /// Post: the form is initialised, ready for the game to start.
        /// </summary>
        public HareAndTortoiseForm() {
            InitializeComponent();
            HareAndTortoiseGame.NumberOfPlayers = HareAndTortoiseGame.MAX_PLAYERS; // Max players, by default.
            HareAndTortoiseGame.InitialiseAllThePlayers();
            Board.SetUpBoard();
            SetupTheGui();
            ResetGame();
        }

        /// <summary>
        /// Set up the GUI when the game is first displayed on the screen.
        /// 
        /// This method is almost complete. It should only be changed by adding one line:
        ///     to set the initial ComboBox selection to "6"; 
        /// 
        /// Pre:  the form contains the controls needed for the game.
        /// Post: the game is ready for the user(s) to play.
        /// </summary>
        private void SetupTheGui() {
            CancelButton = exitButton;  // Allow the Esc key to close the form.
            ResizeGameBoard();
            SetupGameBoard();

            //####################### set intitial ComboBox Seletion to 6 here ####################################
            cbPlayers.SelectedItem = 4;
            SetupPlayersDataGridView();
              
        }// end SetupTheGui


        /// <summary>
        /// Resizes the entire form, so that the individual squares have their correct size, 
        /// as specified by SquareControl.SQUARE_SIZE.  
        /// This method allows us to set the entire form's size to approximately correct value 
        /// when using Visual Studio's Designer, rather than having to get its size correct to the last pixel.
        /// Pre:  none.
        /// Post: the board has the correct size.
        /// </summary>
        private void ResizeGameBoard() {

            // Uncomment all the lines in this method, once you've placed the boardTableLayoutPanel to your form.
            // Do not add any extra code.


            const int SQUARE_SIZE = SquareControl.SQUARE_SIZE;
            int currentHeight = boardTableLayoutPanel.Size.Height;
            int currentWidth = boardTableLayoutPanel.Size.Width;
            int desiredHeight = SQUARE_SIZE * NUM_OF_ROWS;
            int desiredWidth = SQUARE_SIZE * NUM_OF_COLUMNS;
            int increaseInHeight = desiredHeight - currentHeight;
            int increaseInWidth = desiredWidth - currentWidth;
            this.Size += new Size(increaseInWidth, increaseInHeight);
            boardTableLayoutPanel.Size = new Size(desiredWidth, desiredHeight);
        }

        /// <summary>
        /// Creates each SquareControl and adds it to the boardTableLayoutPanel that displays the board.
        /// Pre:  none.
        /// Post: the boardTableLayoutPanel contains all the SquareControl objects for displaying the board.
        /// </summary>
        private void SetupGameBoard() {

            // ########################### Code needs to be written to perform the following  ###############################################
            /*
             *   taking each Square of Baord separately create a SquareContol object containing that Square (look at the Constructor for SquareControl)
             *   
             *   when it is either the Start Square or the Finish Square set the BackColor of the SquareControl to BurlyWood
             *   
             *   DO NOT set the BackColor of any other Square Control 
             * 
             *   Call MapSquareNumtoScreenRowAnd Column  to determine the row and column position of the SquareControl on the TableLayoutPanel
             *   
             *   Add the Control to the TaleLayoutPanel
             * 
             */

            for (int i = Board.START_SQUARE_NUMBER; i <= Board.FINISH_SQUARE_NUMBER; i++)
            {
                Square square = new Square(Board.Squares[i].Number, Board.Squares[i].Name);
                SquareControl squareControl = new SquareControl(square, HareAndTortoiseGame.Players);

                if (square.Number == Board.START_SQUARE_NUMBER || square.Number == Board.FINISH_SQUARE_NUMBER)
                {
                    squareControl.BackColor = Color.BurlyWood;
                }

                int row;
                int col;
                
                
                MapSquareNumToScreenRowAndColumn(i, out row, out col);

                boardTableLayoutPanel.Controls.Add(squareControl, col, row);
            }
        }// SetupGameBaord


        /// <summary>
        /// Tells the players DataGridView to get its data from the hareAndTortoiseGame.Players BindingList.
        /// Pre:  players DataGridView exists on the form.
        ///       HareAndTortoiseGame.Players BindingList is not null.
        /// Post: players DataGridView displays the correct rows and columns.
        /// </summary>
        private void SetupPlayersDataGridView()
        {
            // ########################### Code needs to be written  ###############################################
            playerBindingSource.DataSource = HareAndTortoiseGame.Players;
        }


        /// <summary>
        /// Resets the game, including putting all the players on the Start square.
        /// This requires updating what is displayed in the GUI, 
        /// as well as resetting the attrtibutes of HareAndTortoiseGame .
        /// This method is used by both the Reset button and 
        /// when a new value is chosen in the Number of Players ComboBox.
        /// Pre:  none.
        /// Post: the form displays the game in the same state as when the program first starts 
        ///       (except that any user names that the player has entered are not reset).
        /// </summary>
        private void ResetGame() {

            // ########################### Code needs to be written  ###############################################

            //UpdatePlayersGuiLocations(TypeOfGuiUpdate.RemovePlayer);
            cbPlayers.SelectedItem = HareAndTortoiseGame.NumberOfPlayers;
            HareAndTortoiseGame.SetPlayersAtTheStart();
            cbPlayers.SelectedItem = HareAndTortoiseGame.NumberOfPlayers;
            btnRollDice.Enabled = true;
            
            UpdatePlayersGuiLocations(TypeOfGuiUpdate.AddPlayer);

            foreach (Player player in HareAndTortoiseGame.Players)
            {                
                player.Money = 100;
                player.Winner = false;                
            }

            RefreshPlayersInfoInDataGridView();             
        }


        /// <summary>
        /// At several places in the program's code, it is necessary to update the GUI board,
        /// so that player's tokens (or "pieces") are removed from their old squares
        /// or added to their new squares. E.g. when all players are moved back to the Start.
        /// 
        /// For each of the players, this method is to use the GetSquareNumberOfPlayer method to find out 
        /// which square number the player is on currently, then use the SquareControlAt method
        /// to find the corresponding SquareControl, and then update that SquareControl so that it
        /// knows whether the player is on that square or not.
        /// 
        /// Moving all players from their old to their new squares requires this method to be called twice: 
        /// once with the parameter typeOfGuiUpdate set to RemovePlayer, and once with it set to AddPlayer.
        /// In between those two calls, the players locations must be changed by using one or more methods 
        /// in the HareAndTortoiseGame class. Otherwise, you won't see any change on the screen.
        /// 
        /// Because this method moves ALL players, it should NOT be used when animating a SINGLE player's
        /// movements from square to square.
        /// 
        /// 
        /// Post: the GUI board is updated to match the locations of all Players objects.
        /// </summary>
        /// <param name="typeOfGuiUpdate">Specifies whether all the players are being removed 
        /// from their old squares or added to their new squares</param>
        private void UpdatePlayersGuiLocations(TypeOfGuiUpdate typeOfGuiUpdate) {

            //##################### Code needs to be added here. ############################################################
            for (int i = 0; i < HareAndTortoiseGame.NumberOfPlayers; i++)
            {
                int playerLocation = GetSquareNumberOfPlayer(i);
                SquareControl squareControl = SquareControlAt(playerLocation);

                if (typeOfGuiUpdate == TypeOfGuiUpdate.AddPlayer)
                {
                    squareControl.ContainsPlayers[i] = true;
                }

                else
                {
                    squareControl.ContainsPlayers[i] = false;
                }
            }
            RefreshPlayersInfoInDataGridView();
            RefreshBoardTablePanelLayout(); // Must be the last line in this method. DO NOT put it inside a loop.
        }// end UpdatePlayersGuiLocations


        /// <summary>
        /// Adds an animation to the player tokens to move one square at a time.
        /// Post: the GUI board is updated to match the locations of all Players objects.
        /// </summary>
        /// <param name="typeOfGuiUpdate">Specifies whether all the players are being removed 
        /// from their old squares or added to their new squares</param>
        private void SingleStepAnimation(TypeOfGuiUpdate typeOfGuiUpdate)
        {
            for (int i = 0; i < HareAndTortoiseGame.NumberOfPlayers; i++)
            {
                int playerLocation = GetSquareNumberOfPlayer(i);

                for (int j = 0; j <= playerLocation; j++)
                {
                    SquareControl squareControl = SquareControlAt(j);

                    if (typeOfGuiUpdate == TypeOfGuiUpdate.AddPlayer)
                    {
                        squareControl.ContainsPlayers[i] = true;
                        RefreshBoardTablePanelLayout();
                        Application.DoEvents();
                        Thread.Sleep(100);
                    }

                    else
                    {
                        squareControl.ContainsPlayers[i] = false;
                    }
                }
            }

            RefreshPlayersInfoInDataGridView();
            RefreshBoardTablePanelLayout();
        }





        /*** START OF LOW-LEVEL METHODS *****************************************************************************
         * 
         *   The methods in this section are "helper" methods that can be called to do basic things. 
         *   That makes coding easier in other methods of this class.
         *   You should NOT CHANGE these methods, except where otherwise specified in the assignment. 
         *   
         *   ********************************************************************************************************/

        /// <summary>
        /// When the SquareControl objects are updated (when players move to a new square),
        /// the board's TableLayoutPanel is not updated immediately.  
        /// Each time that players move, this method must be called so that the board's TableLayoutPanel 
        /// is told to refresh what it is displaying.
        /// Pre:  none.
        /// Post: the board's TableLayoutPanel shows the latest information 
        ///       from the collection of SquareControl objects in the TableLayoutPanel.
        /// </summary>
        private void RefreshBoardTablePanelLayout()
        {
            // ################# Uncomment the following line once you've placed the boardTableLayoutPanel on your form. ########################################
            boardTableLayoutPanel.Invalidate(true);
        }//end RefreshBoardTablePanelLayout


        /// <summary>
        /// When the Player objects are updated (location, money, etc.),
        /// the players DataGridView is not updated immediately.  
        /// Each time that those player objects are updated, this method must be called 
        /// so that the players DataGridView is told to refresh what it is displaying.
        /// Pre:  none.
        /// Post: the players DataGridView shows the latest information 
        ///       from the collection of Player objects in the HareAndTortoiseGame.
        /// </summary>
        private void RefreshPlayersInfoInDataGridView() {
            HareAndTortoiseGame.Players.ResetBindings();
        } //end RefreshPlayersInfoInDataGridView


        /// <summary>
        /// Tells you the current square number that a given player is on.
        /// Pre:  a valid playerNumber is specified.
        /// Post: returns the square number of the square the player is on.
        /// </summary>
        /// <param name="playerNumber">The player number.</param>
        /// <returns>Returns the square number of the playerNumber.</returns>
        private int GetSquareNumberOfPlayer(int playerNumber) {
            Square playerSquare = HareAndTortoiseGame.Players[playerNumber].Location;
            return playerSquare.Number;
            
            
        } //end GetSquareNumberOfPlayer


        /// <summary>
        /// Tells you which SquareControl object is associated with a given square number.
        /// Pre:  a valid squareNumber is specified; and
        ///       the boardTableLayoutPanel is properly constructed.
        /// Post: the SquareControl object associated with the square number is returned.
        /// </summary>
        /// <param name="squareNumber">The square number.</param>
        /// <returns>Returns the SquareControl object associated with the square number.</returns>
        private SquareControl SquareControlAt(int squareNumber)
        {
            int rowNumber;
            int columnNumber;
            MapSquareNumToScreenRowAndColumn(squareNumber, out rowNumber, out columnNumber);
            
            // Uncomment the following line once you've added the boardTableLayoutPanel to your form.
            return (SquareControl)boardTableLayoutPanel.GetControlFromPosition(columnNumber, rowNumber);
        } //end SquareControlAt


        /// <summary>
        /// For a given square number, tells you the corresponding row and column numbers.
        /// Pre:  none.
        /// Post: returns the row and column numbers, via "out" parameters.
        /// </summary>
        /// <param name="squareNumber">The input square number.</param>
        /// <param name="rowNumber">The output row number.</param>
        /// <param name="columnNumber">The output column number.</param>
        private static void MapSquareNumToScreenRowAndColumn(int squareNumber, out int rowNumber, out int columnNumber) {

            // ######################## Add more code to this method and replace the next two lines by something more sensible.  ###############################
            const int rows = 7;
            const int cols = 6;

            rowNumber = rows - 1 - Math.DivRem(squareNumber, cols, out columnNumber);
            
            if ((rowNumber & 1) != 0)
                columnNumber = cols - 1 - columnNumber;    
            
        }//end MapSquareNumToScreenRowAndColumn

        /*** END OF LOW-LEVEL METHODS **********************************************/



        /*** START OF EVENT-HANDLING METHODS ***/
        // ####################### Place EVENT HANDLER Methods to this area of code  ##################################################
        /// <summary>
        /// Handle the Exit button being clicked.
        /// Pre:  the Exit button is clicked.
        /// Post: the game is closed.
        /// </summary>
        private void exitButton_Click(object sender, EventArgs e)
        {
            // Terminate immediately, rather than calling Close(), 
            // so that we don't have problems with any animations that are running at the same time.
            Environment.Exit(0);  
        }

        /// <summary>
        /// Handle the Roll Dice button being clicked.
        /// Pre:  the Roll Dice button is clicked.
        /// Post: plays one round of the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRollDice_Click(object sender, EventArgs e)
        {
            UpdatePlayersGuiLocations(TypeOfGuiUpdate.RemovePlayer);

            HareAndTortoiseGame.PlayOneRound();
            
            UpdatePlayersGuiLocations(TypeOfGuiUpdate.AddPlayer);

            if (HareAndTortoiseGame.Finished)
            {
                btnRollDice.Enabled = false;
            }           
        }

        /// <summary>
        /// Handle the Reset button being clicked.
        /// Pre:  the Reset button is clicked.
        /// Post: the game is reset.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetGame();
        }

        /// <summary>
        /// Handle a value in Players Combo box being clicked.
        /// Pre:  a value in Players Combo Box is clicked.
        /// Post: the game is reset with correct number of players.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbPlayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbPlayers.SelectedIndex)
            {
                // Number of Players: 2
                case 0:
                    UpdatePlayersGuiLocations(TypeOfGuiUpdate.RemovePlayer);
                    HareAndTortoiseGame.NumberOfPlayers = 2;
                    UpdatePlayersGuiLocations(TypeOfGuiUpdate.AddPlayer);
                    break;
                // Number of Players: 3
                case 1:

                    UpdatePlayersGuiLocations(TypeOfGuiUpdate.RemovePlayer);                    
                    HareAndTortoiseGame.NumberOfPlayers = 3;
                    UpdatePlayersGuiLocations(TypeOfGuiUpdate.AddPlayer);
                    break;
                // Number of Players: 4
                case 2:
                    UpdatePlayersGuiLocations(TypeOfGuiUpdate.RemovePlayer);
                    HareAndTortoiseGame.NumberOfPlayers = 4;
                    UpdatePlayersGuiLocations(TypeOfGuiUpdate.AddPlayer);
                    break;
                // Number of Players: 5
                case 3:
                    UpdatePlayersGuiLocations(TypeOfGuiUpdate.RemovePlayer);
                    HareAndTortoiseGame.NumberOfPlayers = 5;
                    UpdatePlayersGuiLocations(TypeOfGuiUpdate.AddPlayer);
                    break;
                // Number of Players: 6
                case 4:
                    UpdatePlayersGuiLocations(TypeOfGuiUpdate.RemovePlayer);
                    HareAndTortoiseGame.NumberOfPlayers = 6;
                    UpdatePlayersGuiLocations(TypeOfGuiUpdate.AddPlayer); 
                    break;
                default:
                    break;                
            }
        }

        /// <summary>
        /// Handle the Next Players Roll button being clicked.
        /// Pre:  the Next Players Roll button is clicked.
        /// Post: based on single move animation, plays one round of the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNextPlayersRoll_Click(object sender, EventArgs e)
        {
            SingleStepAnimation(TypeOfGuiUpdate.RemovePlayer);
            HareAndTortoiseGame.PlayOneRound();
            SingleStepAnimation(TypeOfGuiUpdate.AddPlayer);

            if (HareAndTortoiseGame.Finished)
            {
                btnNextPlayersRoll.Enabled = false;
            }
        }

        /// <summary>
        /// Handle the Yes Radio Button being clicked.
        /// Pre:  the Yes Radio Button is clicked.
        /// Post: if Yes, the Roll Dice button is disabled and the Next Players Roll button is enabled. 
        ///         if No, the Roll Dice button is enabled and the Next Players Roll button is disabled. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbYes_CheckedChanged(object sender, EventArgs e)
        {
            if (rbYes.Checked)
            {
                btnRollDice.Enabled = false;
                btnNextPlayersRoll.Enabled = true;
            }

            else
            {
                btnRollDice.Enabled = true;
                btnNextPlayersRoll.Enabled = false;
            }
        }




        /*** END OF EVENT-HANDLING METHODS ***/
    }
}

