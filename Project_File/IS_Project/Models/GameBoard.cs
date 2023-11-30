using IS_Project.AI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IS_Project.Models
{
    public class GameBoard : Sprites
    {
        const int gridSize = 30;
        const int tileSize = 48;

        MouseState mouseState;
        bool mReleased = false;
        public bool diceRolled = false;
        int heldKey = 0;

        public int[] redPiece1 = { 1, 0 };
        public int[] redPiece2 = { 0, 0 };
        public int[] redPiece3 = { 0, 1 };

        public int[] bluePiece1 = { 29, 28 };
        public int[] bluePiece2 = { 29, 29 };
        public int[] bluePiece3 = { 28, 29 };

        public int[] minotuarPos = { -5, -5 };

        public int[,] selectedWall = { { -2, -2 }, { -2, -2 } };

        public string[,] gameBoard = {  { "5R", "5R", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "1"},
                                        { "5R", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1"},
                                        { "0", "0", "0", "0", "0", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "0", "0", "0", "0", "0"},
                                        { "0", "0", "3W", "0", "0", "1", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "1", "0", "0", "3W", "0", "0"},
                                        { "0", "0", "3E", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "3E", "0", "0"},
                                        { "0", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "0", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "0", "0", "0", "0", "0", "0", "0", "0"},
                                        { "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0"},
                                        { "0", "0", "1", "1", "1", "1", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "1", "0", "0"},
                                        { "0", "0", "0", "0", "0", "1", "0", "0", "3N", "3S", "0", "0", "1", "1", "0", "0", "1", "1", "0", "0", "3N", "3S", "0", "0", "1", "0", "0", "0", "0", "0"},
                                        { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                                        { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                                        { "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0"},
                                        { "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0"},
                                        { "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "7R", "7R", "1", "1", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0"},
                                        { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "7R", "T", "T", "1", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                                        { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "1", "T", "T", "7B", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                                        { "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "1", "1", "7B", "7B", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0"},
                                        { "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0"},
                                        { "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0"},
                                        { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                                        { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                                        { "0", "0", "0", "0", "0", "1", "0", "0", "3N", "3S", "0", "0", "1", "1", "0", "0", "1", "1", "0", "0", "3N", "3S", "0", "0", "1", "0", "0", "0", "0", "0"},
                                        { "0", "0", "1", "1", "1", "1", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "1", "0", "0"},
                                        { "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0"},
                                        { "0", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "0", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "0", "0", "0", "0", "0", "0", "0", "0"},
                                        { "0", "0", "3W", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "3W", "0", "0"},
                                        { "0", "0", "3E", "0", "0", "1", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "1", "0", "0", "3E", "0", "0"},
                                        { "0", "0", "0", "0", "0", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "0", "0", "0", "0", "0"},
                                        { "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "5B"},
                                        { "1", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "5B", "5B"}
                                    };
        //moved to const
        /*public List<((int, int), (int, int))> baseChokepointList = new List<((int, int), (int, int))>()
        {
            ((7, 0),(7, 1)), ((11, 0),(11, 1)), ((13, 0),(13, 1)), ((16, 0),(16, 1)), ((18, 0),(18, 1)), ((22, 0),(22, 1)),
            ((14, 2),(15, 2)),
            ((0, 5),(1, 5)), ((4, 5),(5,5)), ((9, 5),(10, 5)), ((14, 5),(15, 5)), ((19, 5),(20, 5)), ((24, 5),(25, 5)), ((28, 5),(29, 5)),
            ((6, 6),(6, 7)), ((11, 6),(11, 7)), ((13, 6),(13, 7)), ((16, 6),(16, 7)), ((18, 6),(18, 7)), ((23, 6),(23, 7)),
            ((0, 7),(1, 7)), ((28, 7),(29, 7)),
            ((14, 8),(15, 8)),
            ((2, 8),(2, 9)), ((27, 8),(27, 9)),
            ((0, 10),(1, 10)), ((28, 10),(29, 10)),
            ((12, 11),(13, 11)), ((16, 11),(17, 11)),
            ((11, 12),(11, 13)), ((18, 12),(18, 13)),
            ((0, 13),(1, 13)), ((28, 13),(29, 13)),
            ((2, 14),(2, 15)), ((6, 14),(6, 15)), ((8, 14),(8, 15)), ((21, 14),(21, 15)), ((23, 14),(23, 15)), ((27, 14),(27, 15)),
            ((0, 16),(1, 16)), ((28, 16),(29, 16)),
            ((11, 16),(11, 17)), ((18, 16),(18, 17)),
            ((12, 18),(13, 18)), ((16, 18),(17, 18)),
            ((0, 19),(1, 19)), ((28, 19),(29, 19)),
            ((2, 20),(2, 21)), ((27, 20),(27, 21)),
            ((14, 21),(15, 21)),
            ((0, 22),(1, 22)), ((28, 22),(29, 22)),
            ((6, 22),(6, 23)), ((11, 22),(11, 23)), ((13, 22),(13, 23)), ((16, 22),(16, 23)), ((18, 22),(18, 23)), ((23, 22) , (23, 23)),
            ((0, 24),(1, 24)), ((4, 24),(5,24)), ((9, 24),(10, 24)), ((14, 24),(15, 24)), ((19, 24),(20, 24)), ((24, 24),(25, 24)), ((28, 24),(29, 24)),
            ((7, 28),(7, 29)), ((11, 28),(11, 29)), ((13, 28) , (13, 29)), ((16, 28) , (16, 29)), ((18, 28) , (18, 29)), ((22, 28) , (22, 29))
        };*/

        public List<((int, int), (int, int))> availableChokepointList = new List<((int, int), (int, int))>()
        {
            ((14, 2),(15, 2)),
            ((0, 5),(1, 5)), ((4, 5),(5,5)), ((9, 5),(10, 5)), ((14, 5),(15, 5)), ((19, 5),(20, 5)), ((24, 5),(25, 5)), ((28, 5),(29, 5)),
            ((6, 6),(6, 7)), ((11, 6),(11, 7)), ((13, 6),(13, 7)), ((16, 6),(16, 7)), ((18, 6),(18, 7)), ((23, 6),(23, 7)),
            ((0, 7),(1, 7)), ((28, 7),(29, 7)),
            ((14, 8),(15, 8)),
            ((2, 8),(2, 9)), ((27, 8),(27, 9)),
            ((0, 10),(1, 10)), ((28, 10),(29, 10)),
            ((12, 11),(13, 11)), ((16, 11),(17, 11)),
            ((11, 12),(11, 13)), ((18, 12),(18, 13)),
            ((0, 13),(1, 13)), ((28, 13),(29, 13)),
            ((2, 14),(2, 15)), ((6, 14),(6, 15)), ((8, 14),(8, 15)), ((21, 14),(21, 15)), ((23, 14),(23, 15)), ((27, 14),(27, 15)),
            ((0, 16),(1, 16)), ((28, 16),(29, 16)),
            ((11, 16),(11, 17)), ((18, 16),(18, 17)),
            ((12, 18),(13, 18)), ((16, 18),(17, 18)),
            ((0, 19),(1, 19)), ((28, 19),(29, 19)),
            ((2, 20),(2, 21)), ((27, 20),(27, 21)),
            ((14, 21),(15, 21)),
            ((0, 22),(1, 22)), ((28, 22),(29, 22)),
            ((6, 22),(6, 23)), ((11, 22),(11, 23)), ((13, 22),(13, 23)), ((16, 22),(16, 23)), ((18, 22),(18, 23)), ((23, 22) , (23, 23)),
            ((0, 24),(1, 24)), ((4, 24),(5,24)), ((9, 24),(10, 24)), ((14, 24),(15, 24)), ((19, 24),(20, 24)), ((24, 24),(25, 24)), ((28, 24),(29, 24))
        };

        public List<((int, int), (int, int))> wallPositionList = new List<((int, int), (int, int))>()
        {
            ((3, 2),(4, 2)), ((25, 2),(26, 2)),
            ((8, 8),(8, 9)), ((21, 8),(21, 9)),
            ((8, 20),(8, 21)), ((21, 20),(21, 21)),
            ((3, 27),(4, 27)), ((25, 27),(26, 27)),
        };

        public string[] impassableList = { "1", "3N", "3S", "3E", "3W", "T" };
        public string[] passable = { "0", "4R", "4B" };
        public string[] playerBases = { "5R", "5B", "7R", "7B" };

        public List<(int, int)> redVictoryTiles = new List<(int, int)>()
        {
            (13, 13), (13, 14), (14, 13),
        };
        public List<(int, int)> blueVictoryTiles = new List<(int, int)>()
        {
            ( 15, 16 ), (16,16), (16,15),
        };

        public bool isRedTurn = true;
        public bool redVictory = false;
        public bool blueVictory = false;

        public bool isGreyActive = false;
        public bool isBlackActive = false;
        public bool addWallActive = false;

        public string selectedPiece = "";

        public int availableMoves = 0;

        int mPosX;
        int mPosY;

        //for testing
        string clicked = "";
        public string LastBlueMove = "";
        public int boardState = 0;

        public GameBoard() { }
        //copy constructor
        public GameBoard(bool dr, List<int[]> pieceList, int[] minoPos, int[,] selWall, string[,] board,
                         List<((int, int), (int, int))> cpList, List<((int, int), (int, int))> wallPosList,
                         bool redTurn, bool vicR, bool vicB, bool isGrey, bool blackActive, string selPiece, int moves)
        {
            this.diceRolled = dr;
            this.redPiece1 = pieceList[0];
            this.redPiece2 = pieceList[1];
            this.redPiece3 = pieceList[2];
            this.bluePiece1 = pieceList[3];
            this.bluePiece2 = pieceList[4];
            this.bluePiece3 = pieceList[5];
            this.minotuarPos = minoPos;
            this.selectedWall = selWall;
            this.gameBoard = board;
            this.availableChokepointList = cpList;
            this.wallPositionList = wallPosList;

            this.isRedTurn = redTurn;
            this.redVictory = vicR;
            this.blueVictory = vicB;
            this.isGreyActive = isGrey;
            this.isBlackActive = blackActive;

            this.selectedPiece = selPiece;
            this.availableMoves = moves;
        }

        public void Update()
        {
            mouseState = Mouse.GetState();
            mPosX = mouseState.Position.X / 48;
            mPosY = mouseState.Position.Y / 48;

            //if the mouse selects a piece
            if (mouseState.LeftButton == ButtonState.Pressed && mReleased)
            {
                //clicked = mPosX < 30 ? gameBoard[mPosX, mPosY] : "";
                //if dice roll selected 
                if (Vector2.Distance(new Vector2(1524, 114), mouseState.Position.ToVector2()) < 24 && !diceRolled)
                {
                    rollDice();
                }

                //if black rolled
                else if (!isBlackActive && selectedPiece == "mino")
                {
                    if ((((mPosX == 12 || mPosX == 17) && (mPosY > 11 && mPosY < 18)) || 
                         ((mPosX > 12 && mPosX < 17) && (mPosY == 12 || mPosY == 17))) &&
                          gameBoard[mPosX, mPosY] == "0")
                    {
                        minotuarPos[0] = mPosX;
                        minotuarPos[1] = mPosY;
                        gameBoard[minotuarPos[0], minotuarPos[1]] = "6";
                        isBlackActive = true;
                        availableMoves--;
                    }
                }

                //if a movable wall is selected after a Grey has been "rolled" the selectedWall variable will be set to the possition of the two wall-tiles
                //N and W will always be the first wall in selected wall
                else if (isGreyActive && selectedWall[0, 0] == -2)
                {
                    selectWall(mPosX, mPosY, ((mPosX, mPosY) == (31, 13) || (mPosX, mPosY) == (32, 13)));
                }

                //if a wall is selected and the mouse clicks the rotate wall "button" @ 31,9
                //the wall will try to rotate (hopefully)
                else if (mPosX == 31 && mPosY == 9 && selectedWall[0, 1] != -2 && selectedWall[0, 0] < 30)
                {
                    rotateWall();
                }

                //if a wall is selected (implying isGreyActive is true) and the user clicks to move
                else if (selectedWall[0, 0] != -2 && (mPosX < 30 && gameBoard[mPosX, mPosY].Contains('0')))
                {
                    moveWallByClick(mPosX, mPosY);
                }

                //for player piece selection (not mino)
                else if (selectedPiece == "")
                {
                    if ((redPiece1[0], redPiece1[1]) == (mPosX, mPosY) && isRedTurn)
                    {
                        selectedPiece = "red1";
                    }
                    else if ((redPiece2[0], redPiece2[1]) == (mPosX, mPosY) && isRedTurn)
                    {
                        selectedPiece = "red2";
                    }
                    else if ((redPiece3[0], redPiece3[1]) == (mPosX, mPosY) && isRedTurn)
                    {
                        selectedPiece = "red3";
                    }
                    //commented for now as the player should be red
                    /*else if (Vector2.Distance(new Vector2((bluePiece1[0] * tileSize) + 24, (bluePiece1[1] * tileSize) + 24), mouseState.Position.ToVector2()) < 24 && !isRedTurn)
                            {
                                selectedPiece = "blue1";
                            }
                            else if (Vector2.Distance(new Vector2((bluePiece2[0] * tileSize) + 24, (bluePiece2[1] * tileSize) + 24), mouseState.Position.ToVector2()) < 24 && !isRedTurn)
                            {
                                selectedPiece = "blue2";
                            }
                            else if (Vector2.Distance(new Vector2((bluePiece3[0] * tileSize) + 24, (bluePiece3[1] * tileSize) + 24), mouseState.Position.ToVector2()) < 24 && !isRedTurn)
                            {
                                selectedPiece = "blue3";
                            }*/
                }

                //click movement for player pieces
                else if (selectedPiece != "" && mPosX < 30 && mPosX >= 0 && mPosY < 30 && mPosY >= 0)
                {
                    movePieceByClick(mPosX, mPosY);
                }
                mReleased = false;
            }

            //if there is a selected piece (movement by key)
            if (selectedPiece != "" && availableMoves > 0 && heldKey == 0)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    movePieceByKey("up");
                    heldKey = 1;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    movePieceByKey("down");
                    heldKey = 2;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    movePieceByKey("left");
                    heldKey = 3;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    movePieceByKey("right");
                    heldKey = 4;
                }

                //mino collision conditions
                //if (selectedPiece == "mino" && isBlackActive)
                //{
                //    minoCollisionCheck(minotuarPos[0], minotuarPos[1]);
                //}
            }

            //movement for wall (arrow keys)
            if (isGreyActive && !addWallActive && heldKey == 0)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    moveWallByKey("up");
                    heldKey = 1;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    moveWallByKey("down");
                    heldKey = 2;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    moveWallByKey("left");
                    heldKey = 3;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    moveWallByKey("right");
                    heldKey = 4;
                }
            }

            //logic for end-turn key (enter key)
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                endTurn();
            }

            //to ensure mouse 1 is released
            if (mouseState.LeftButton == ButtonState.Released)
            {
                mReleased = true;
            }

            //logic to prevent a held input (keyboard)
            switch (heldKey)
            {
                //up
                case 1:
                    if (Keyboard.GetState().IsKeyUp(Keys.Up))
                    {
                        heldKey = 0;
                    }
                    break;
                //udown
                case 2:
                    if (Keyboard.GetState().IsKeyUp(Keys.Down))
                    {
                        heldKey = 0;
                    }
                    break;
                //up
                case 3:
                    if (Keyboard.GetState().IsKeyUp(Keys.Left))
                    {
                        heldKey = 0;
                    }
                    break;
                //up
                case 4:
                    if (Keyboard.GetState().IsKeyUp(Keys.Right))
                    {
                        heldKey = 0;
                    }
                    break;
                case 0:
                    break;
            }

            //general victory conditions
            if (redVictoryTiles.Contains((redPiece1[0], redPiece1[0])) && redVictoryTiles.Contains((redPiece2[0], redPiece2[0])) && redVictoryTiles.Contains((redPiece3[0], redPiece3[0])))
            {
                redVictory = true;
            }
            else if (blueVictoryTiles.Contains((bluePiece1[0], bluePiece1[0])) && blueVictoryTiles.Contains((bluePiece2[0], bluePiece2[0])) && blueVictoryTiles.Contains((bluePiece3[0], bluePiece3[0])))
            {
                blueVictory = true;
            }

        }

        public void Draw()
        {
            //sets up the playing board grid
            for (int i = 0; i < gridSize; ++i)
            {
                for (int j = 0; j < gridSize; ++j)
                {
                    switch (gameBoard[i, j])
                    {
                        //path sprite
                        case "0":
                            Globals.SpriteBatch.Draw(pathTile, new Vector2(i * tileSize, j * tileSize), Color.White);
                            break;

                        //hedge sprite
                        case "1":
                            Globals.SpriteBatch.Draw(hedgeTile, new Vector2(i * tileSize, j * tileSize), Color.White);
                            break;

                        //movable wall sprites
                        case "3N":
                            Globals.SpriteBatch.Draw(topWallTile, new Vector2(i * tileSize, j * tileSize), Color.White);
                            break;
                        case "3S":
                            Globals.SpriteBatch.Draw(bottomWallTile, new Vector2(i * tileSize, j * tileSize), Color.White);
                            break;
                        case "3E":
                            Globals.SpriteBatch.Draw(rightWallTile, new Vector2(i * tileSize, j * tileSize), Color.White);
                            break;
                        case "3W":
                            Globals.SpriteBatch.Draw(leftWallTile, new Vector2(i * tileSize, j * tileSize), Color.White);
                            break;

                        //player sprites

                        case "5R":
                        case "7R":
                            Globals.SpriteBatch.Draw(redTile, new Vector2(i * tileSize, j * tileSize), Color.White);
                            break;

                        case "5B":
                        case "7B":
                            Globals.SpriteBatch.Draw(blueTile, new Vector2(i * tileSize, j * tileSize), Color.White);
                            break;

                    }
                }
            }

            //sets-up player peices
            Globals.SpriteBatch.Draw(redPlayerPiece, new Vector2(redPiece1[0] * tileSize, redPiece1[1] * tileSize), Color.White);
            Globals.SpriteBatch.Draw(redPlayerPiece, new Vector2(redPiece2[0] * tileSize, redPiece2[1] * tileSize), Color.White);
            Globals.SpriteBatch.Draw(redPlayerPiece, new Vector2(redPiece3[0] * tileSize, redPiece3[1] * tileSize), Color.White);

            Globals.SpriteBatch.Draw(bluePlayerPiece, new Vector2(bluePiece1[0] * tileSize, bluePiece1[1] * tileSize), Color.White);
            Globals.SpriteBatch.Draw(bluePlayerPiece, new Vector2(bluePiece2[0] * tileSize, bluePiece2[1] * tileSize), Color.White);
            Globals.SpriteBatch.Draw(bluePlayerPiece, new Vector2(bluePiece3[0] * tileSize, bluePiece3[1] * tileSize), Color.White);

            Globals.SpriteBatch.DrawString(basicFont, "selected piece:" + selectedPiece, new Vector2(1465, 0), Color.Black);
            Globals.SpriteBatch.DrawString(basicFont, "moves:" + availableMoves, new Vector2(1465, 30), Color.Black);
            Globals.SpriteBatch.DrawString(basicFont, "Red Turn:" + isRedTurn, new Vector2(1465, 60), Color.Black);
            Globals.SpriteBatch.DrawString(basicFont, "Mouse on Grid:" + mPosX + " , " + mPosY, new Vector2(1465, 160), Color.Black);

            Globals.SpriteBatch.DrawString(basicFont, "Just Clicked: " + clicked, new Vector2(1465, 200), Color.Black);

            Globals.SpriteBatch.DrawString(basicFont, "Selected wall: {" + selectedWall[0, 0] + "," + selectedWall[0, 1] + "}, {" + selectedWall[1, 0] + "," + selectedWall[1, 1] + "}", new Vector2(1465, 260), Color.Black);

            Globals.SpriteBatch.Draw(leftWallTile, new Vector2(1500, 90), Color.Honeydew);

            Globals.SpriteBatch.DrawString(basicFont, "ROTATE WALL", new Vector2(1488, 384), Color.Black);
            Globals.SpriteBatch.Draw(pathTile, new Vector2(1488, 432), Color.Honeydew);

            Globals.SpriteBatch.DrawString(basicFont, "ADD WALL", new Vector2(1488, 576), Color.Black);
            Globals.SpriteBatch.Draw(leftWallTile, new Vector2(1488, 624), Color.White);
            Globals.SpriteBatch.Draw(rightWallTile, new Vector2(1536, 624), Color.White);

            Globals.SpriteBatch.DrawString(basicFont, "isBlackActive:" + isBlackActive, new Vector2(1488, 500), Color.Black);
            Globals.SpriteBatch.DrawString(basicFont, "isGreyActive:" + isGreyActive, new Vector2(1488, 540), Color.Black);
            Globals.SpriteBatch.DrawString(basicFont, "addWallActive:" + addWallActive, new Vector2(1488, 680), Color.Black);

            Globals.SpriteBatch.DrawString(basicFont, "blue1: {" + bluePiece1[0] + "," + bluePiece1[1] + "}", new Vector2(1488, 720), Color.Black);
            Globals.SpriteBatch.DrawString(basicFont, "blue2: {" + bluePiece2[0] + "," + bluePiece2[1] + "}", new Vector2(1488, 760), Color.Black);
            Globals.SpriteBatch.DrawString(basicFont, "blue3: {" + bluePiece3[0] + "," + bluePiece3[1] + "}", new Vector2(1488, 800), Color.Black);
            Globals.SpriteBatch.DrawString(basicFont, "last blue move: "+ LastBlueMove, new Vector2(1488, 840), Color.Black);
            Globals.SpriteBatch.DrawString(basicFont, "boardState: " + boardState, new Vector2(1488, 900), Color.Black);

            Globals.SpriteBatch.Draw(minotaur, minotuarPos[0] > 0 ? new Vector2(minotuarPos[0] * tileSize, minotuarPos[1] * tileSize) : 
                                                                    new Vector2((float)(14.5 * tileSize), (float)(14.5 * tileSize)), Color.White);

        }

        //rolls dice and sets the piece for grey & black
        public void rollDice()
        {
            diceRolled = true;
            Random rnd = new Random();
            int num = rnd.Next(1, 7);
            switch (num)
            {
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 1:
                    selectedPiece = "mino";
                    availableMoves = 8;
                    break;

                //case 1:
                //    availableMoves = 3;
                //    break;

                //case 1:
                //case 3:
                //case 4:
                //case 5:
                //case 6:
                //case 2:
                //    selectedPiece = "grey";
                //    isGreyActive = true;
                //    break;

                default:
                    availableMoves = num;
                    break;
            }
        }

        //moves player pieces or the Minotaur on black
        public void movePieceByKey(string direction)
        {

            int[] curPiece = {-1, -1};
            switch (selectedPiece)
            {
                case "red1":
                    curPiece = redPiece1;
                    break;

                case "red2":
                    curPiece = redPiece2;
                    break;

                case "red3":
                    curPiece = redPiece3;
                    break;


                case "blue1":
                    curPiece = bluePiece1;
                    break;

                case "blue2":
                    curPiece = bluePiece2;
                    break;

                case "blue3":
                    curPiece = bluePiece3;
                    break;

                case "mino":
                    switch (direction)
                    {
                        case "up":
                            if (minotuarPos[1] > 0 && passable.Contains(gameBoard[minotuarPos[0], minotuarPos[1] - 1]))
                            {
                                checkPieceWallCollision(minotuarPos[0], minotuarPos[1] - 1, (minotuarPos[0], minotuarPos[1]));
                                if (minoCollisionCheck(minotuarPos[0], minotuarPos[1] - 1))
                                {
                                    break;
                                }
                                gameBoard[minotuarPos[0], minotuarPos[1]] = "0";
                                minotuarPos[1]--;
                                gameBoard[minotuarPos[0], minotuarPos[1]] = "6";
                                availableMoves--;
                            }
                            break;
                        case "down":
                            if (minotuarPos[1] < 29 && passable.Contains(gameBoard[minotuarPos[0], minotuarPos[1] + 1]))
                            {
                                checkPieceWallCollision(minotuarPos[0], minotuarPos[1] + 1, (minotuarPos[0], minotuarPos[1]));
                                if (minoCollisionCheck(minotuarPos[0], minotuarPos[1] + 1))
                                {
                                    break;
                                }
                                gameBoard[minotuarPos[0], minotuarPos[1]] = "0";
                                minotuarPos[1]++;
                                gameBoard[minotuarPos[0], minotuarPos[1]] = "6";
                                availableMoves--;
                            }
                            break;
                        case "left":
                            if (minotuarPos[0] > 0 && passable.Contains(gameBoard[minotuarPos[0] - 1, minotuarPos[1]]))
                            {
                                checkPieceWallCollision(minotuarPos[0] - 1, minotuarPos[1], (minotuarPos[0], minotuarPos[1]));
                                if (minoCollisionCheck(minotuarPos[0] - 1, minotuarPos[1]))
                                {
                                    break;
                                }
                                gameBoard[minotuarPos[0], minotuarPos[1]] = "0";
                                minotuarPos[0]--;
                                gameBoard[minotuarPos[0], minotuarPos[1]] = "6";
                                availableMoves--;
                            }
                            break;
                        case "right":
                            if (minotuarPos[0] < 29 && passable.Contains(gameBoard[minotuarPos[0] + 1, minotuarPos[1]]))
                            {
                                checkPieceWallCollision(minotuarPos[0] + 1, minotuarPos[1], (minotuarPos[0], minotuarPos[1]));
                                if (minoCollisionCheck(minotuarPos[0] + 1, minotuarPos[1]))
                                {
                                    break;
                                }
                                gameBoard[minotuarPos[0], minotuarPos[1]] = "0";
                                minotuarPos[0]++;
                                gameBoard[minotuarPos[0], minotuarPos[1]] = "6";
                                availableMoves--;
                            }
                            break;
                    }
                    break;
            }

            if(selectedPiece != "mino")
            {
                switch (direction)
                {
                    case "up":
                        if (curPiece[1] > 0 && !impassableList.Contains(gameBoard[curPiece[0], curPiece[1] - 1]))
                        {
                            if (gameBoard[curPiece[0], curPiece[1]].Contains("4") && availableMoves > 0)
                            {
                                gameBoard[curPiece[0], curPiece[1]] = "0";
                            }
                            else if (availableMoves == 1 && gameBoard[curPiece[0], curPiece[1] - 1] == "0")
                            {
                                //was just gameBoard[curPiece[0], curPiece[1] - 1] = "4"; before!!!!
                                if (isRedTurn)
                                {
                                    gameBoard[curPiece[0], curPiece[1] - 1] = "4R";
                                }
                                else
                                {
                                    gameBoard[curPiece[0], curPiece[1] - 1] = "4B";
                                }
                            }
                            else if (availableMoves == 1 && gameBoard[curPiece[0], curPiece[1] - 1] == "4")
                            {
                                break;
                            }
                            checkPieceWallCollision(curPiece[0], curPiece[1] - 1, (curPiece[0], curPiece[1]));
                            curPiece[1]--;
                            availableMoves--;
                        }
                        break;
                    case "down":
                        if (curPiece[1] < 29 && !impassableList.Contains(gameBoard[curPiece[0], curPiece[1] + 1]))
                        {
                            if (gameBoard[curPiece[0], curPiece[1]].Contains("4") && availableMoves > 0)
                            {
                                gameBoard[curPiece[0], curPiece[1]] = "0";
                            }
                            else if (availableMoves == 1 && gameBoard[curPiece[0], curPiece[1] + 1] == "0")
                            {
                                if (isRedTurn)
                                {
                                    gameBoard[curPiece[0], curPiece[1] + 1] = "4R";
                                }
                                else
                                {
                                    gameBoard[curPiece[0], curPiece[1] + 1] = "4B";
                                }
                            }
                            else if (availableMoves == 1 && gameBoard[curPiece[0], curPiece[1] + 1] == "4")
                            {
                                break;
                            }
                            checkPieceWallCollision(curPiece[0], curPiece[1] + 1, (curPiece[0], curPiece[1]));
                            curPiece[1]++;
                            availableMoves--;
                        }
                        break;
                    case "left":
                        if (curPiece[0] > 0 && !impassableList.Contains(gameBoard[curPiece[0] - 1, curPiece[1]]))
                        {
                            if (gameBoard[curPiece[0], curPiece[1]].Contains("4") && availableMoves > 0)
                            {
                                gameBoard[curPiece[0], curPiece[1]] = "0";
                            }
                            else if (availableMoves == 1 && gameBoard[curPiece[0] - 1, curPiece[1]] == "0")
                            {
                                if (isRedTurn)
                                {
                                    gameBoard[curPiece[0] - 1, curPiece[1]] = "4R";
                                }
                                else
                                {
                                    gameBoard[curPiece[0] - 1, curPiece[1]] = "4B";
                                }
                            }
                            else if (availableMoves == 1 && gameBoard[curPiece[0] - 1, curPiece[1]] == "4")
                            {
                                break;
                            }
                            checkPieceWallCollision(curPiece[0] - 1, curPiece[1], (curPiece[0], curPiece[1]));
                            curPiece[0]--;
                            availableMoves--;
                        }
                        break;
                    case "right":
                        if (curPiece[0] < 29 && !impassableList.Contains(gameBoard[curPiece[0] + 1, curPiece[1]]))
                        {
                            if (gameBoard[curPiece[0], curPiece[1]].Contains("4") && availableMoves > 0)
                            {
                                gameBoard[curPiece[0], curPiece[1]] = "0";
                            }
                            else if (availableMoves == 1 && gameBoard[curPiece[0] + 1, curPiece[1]] == "0")
                            {
                                if (isRedTurn)
                                {
                                    gameBoard[curPiece[0] + 1, curPiece[1]] = "4R";
                                }
                                else
                                {
                                    gameBoard[curPiece[0] + 1, curPiece[1]] = "4B";
                                }
                            }
                            else if (availableMoves == 1 && gameBoard[curPiece[0] + 1, curPiece[1]] == "4")
                            {
                                break;
                            }
                            checkPieceWallCollision(curPiece[0] + 1, curPiece[1], (curPiece[0], curPiece[1]));
                            curPiece[0]++;
                            availableMoves--;
                        }
                        break;
                }
            }
        }

        //if player piece selected & availible moves > 0 & an open space is clicked
        //player piece will be moved to that location
        public void movePieceByClick(int xDest, int yDest)
        {
            if (gameBoard[xDest, yDest] == "0" || (selectedPiece == "mino" && gameBoard[xDest, yDest].Contains('4')) || gameBoard[xDest, yDest].Contains('7'))
            {
                BoardSearch ppbfs = new BoardSearch(this);
                int[] curPiece = { -1, -1 };
                switch (selectedPiece)
                {
                    case "red1":
                        curPiece = redPiece1;
                        break;

                    case "red2":
                        curPiece = redPiece2;
                        break;

                    case "red3":
                        curPiece = redPiece3;
                        break;

                    case "blue1":
                        curPiece = bluePiece1;
                        break;

                    case "blue2":
                        curPiece = bluePiece2;
                        break;

                    case "blue3":
                        curPiece = bluePiece3;
                        break;

                    case "mino":
                        curPiece = minotuarPos;
                        break;
                }

                int movesNeeded = ppbfs.getBfsToCoord(curPiece[0], curPiece[1], (xDest, yDest), availableMoves).Count;
                if (movesNeeded > 0 && movesNeeded <= availableMoves)
                {
                    availableMoves -= movesNeeded;
                    if (gameBoard[xDest, yDest] == "0")
                    {
                        checkPieceWallCollision(xDest, xDest, (curPiece[0], curPiece[1]));

                        if (selectedPiece != "mino")
                        {
                            if (gameBoard[curPiece[0], curPiece[1]].Contains('4'))
                            {
                                gameBoard[curPiece[0], curPiece[1]] = "0";
                            }
                            if (selectedPiece.Contains('r'))
                            {
                                gameBoard[xDest, yDest] = "4R";
                            }
                            else
                            {
                                gameBoard[xDest, yDest] = "4B";
                            }
                        }
                        else
                        {
                            
                            gameBoard[curPiece[0], curPiece[1]] = "0";
                            gameBoard[xDest, yDest] = "6";
                        }
                        
                    }
                    else if(selectedPiece == "mino")
                    {
                        minoCollisionCheck(xDest, yDest);
                        return;
                    }
                    else
                    {
                        gameBoard[curPiece[0], curPiece[1]] = "0";
                        if (selectedPiece.Contains('r') && gameBoard[curPiece[0], curPiece[1]] == "7R")
                        {
                            availableMoves = 0;
                        }
                        else if (gameBoard[curPiece[0], curPiece[1]] == "7B")
                        {
                            availableMoves = 0;
                        }
                    }
                    curPiece[0] = xDest;
                    curPiece[1] = yDest;
                }
            }
        }

        //gets the movable wall coordinates into the selected wall variable correctly
        public void selectWall(int selectX, int selectY, bool addWall)
        {
            if (addWall)
            {
                addWallActive = true;
                selectedWall[0, 0] = selectX;
                //for testing
                clicked = "Add Wall";
            }
            else if (selectX < 30 && gameBoard[selectX, selectY].Contains('3'))
            {
                if (gameBoard[selectX, selectY].Contains('N'))
                {
                    selectedWall[0, 0] = selectX;
                    selectedWall[0, 1] = selectY;
                    selectedWall[1, 0] = selectX;
                    selectedWall[1, 1] = selectY + 1;
                }
                else if (gameBoard[selectX, selectY].Contains('S'))
                {
                    selectedWall[0, 0] = selectX;
                    selectedWall[0, 1] = selectY - 1;
                    selectedWall[1, 0] = selectX;
                    selectedWall[1, 1] = selectY;
                }
                else if (gameBoard[selectX, selectY].Contains('E'))
                {
                    selectedWall[0, 0] = selectX - 1;
                    selectedWall[0, 1] = selectY;
                    selectedWall[1, 0] = selectX;
                    selectedWall[1, 1] = selectY;
                }
                else if (gameBoard[selectX, selectY].Contains('W'))
                {
                    selectedWall[0, 0] = selectX;
                    selectedWall[0, 1] = selectY;
                    selectedWall[1, 0] = selectX + 1;
                    selectedWall[1, 1] = selectY;
                };
            }
        }

        //if a wall on the board is selected this function will move that wall based on the input
        //direction of the arrow keys. If not possible, it will do nothing
        public void moveWallByKey(string direction)
        {
            int[,] originalPos = { { selectedWall[0, 0], selectedWall[0, 1] }, { selectedWall[1, 0], selectedWall[1, 1] } };

            switch (direction)
            {
                case "up":
                    if (selectedWall[0, 1] > 0 && gameBoard[selectedWall[0, 0], selectedWall[0, 1] - 1] == "0" && (gameBoard[selectedWall[1, 0], selectedWall[1, 1] - 1] == "0" || gameBoard[selectedWall[1, 0], selectedWall[1, 1] - 1] == "3N"))
                    {
                        //N then S
                        gameBoard[selectedWall[0, 0], selectedWall[0, 1] - 1] = gameBoard[selectedWall[0, 0], selectedWall[0, 1]];
                        gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";
                        gameBoard[selectedWall[1, 0], selectedWall[1, 1] - 1] = gameBoard[selectedWall[1, 0], selectedWall[1, 1]];
                        gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";

                        selectedWall[0, 1]--;
                        selectedWall[1, 1]--;
                    }
                    break;
                case "down":
                    if (selectedWall[1, 1] < 29 && gameBoard[selectedWall[1, 0], selectedWall[1, 1] + 1] == "0" && (gameBoard[selectedWall[0, 0], selectedWall[0, 1] + 1] == "0" || gameBoard[selectedWall[0, 0], selectedWall[0, 1] + 1] == "3S"))
                    {
                        //S then N
                        gameBoard[selectedWall[1, 0], selectedWall[1, 1] + 1] = gameBoard[selectedWall[1, 0], selectedWall[1, 1]];
                        gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";
                        gameBoard[selectedWall[0, 0], selectedWall[0, 1] + 1] = gameBoard[selectedWall[0, 0], selectedWall[0, 1]];
                        gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";

                        selectedWall[0, 1]++;
                        selectedWall[1, 1]++;
                    }
                    break;
                case "left":
                    if (selectedWall[0, 0] > 0 && gameBoard[selectedWall[0, 0] - 1, selectedWall[0, 1]] == "0" && (gameBoard[selectedWall[1, 0] - 1, selectedWall[1, 1]] == "0" || gameBoard[selectedWall[1, 0] - 1, selectedWall[1, 1]] == "3W"))
                    {
                        //W then E
                        gameBoard[selectedWall[0, 0] - 1, selectedWall[0, 1]] = gameBoard[selectedWall[0, 0], selectedWall[0, 1]];
                        gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";
                        gameBoard[selectedWall[1, 0] - 1, selectedWall[1, 1]] = gameBoard[selectedWall[1, 0], selectedWall[1, 1]];
                        gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";


                        selectedWall[0, 0]--;
                        selectedWall[1, 0]--;
                    }
                    break;
                case "right":
                    if (selectedWall[1, 0] < 29 && gameBoard[selectedWall[1, 0] + 1, selectedWall[1, 1]] == "0" && (gameBoard[selectedWall[0, 0] + 1, selectedWall[0, 1]] == "0" || gameBoard[selectedWall[0, 0] + 1, selectedWall[0, 1]] == "3E"))
                    {
                        //E then W
                        gameBoard[selectedWall[1, 0] + 1, selectedWall[1, 1]] = gameBoard[selectedWall[1, 0], selectedWall[1, 1]];
                        gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";
                        gameBoard[selectedWall[0, 0] + 1, selectedWall[0, 1]] = gameBoard[selectedWall[0, 0], selectedWall[0, 1]];
                        gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";

                        selectedWall[0, 0]++;
                        selectedWall[1, 0]++;
                    }
                    break;
            }
            checkWallPosition(originalPos);
        }

        //if any wall is selected this function will move that wall to the designated cordinates.
        //if there is inadequate space +- 1 will be checked too.
        //If still not possible, it will do nothing
        public void moveWallByClick(int xDest, int yDest)
        {
            int[,] originalPos = { { selectedWall[0, 0], selectedWall[0, 1] }, { selectedWall[1, 0], selectedWall[1, 1] } };
            //if statements for collision:

            //if wall is currently NS
            if (selectedWall[0, 0] == selectedWall[1, 0])
            {
                if (gameBoard[xDest, yDest + 1].Contains('0'))
                {
                    gameBoard[xDest, yDest] = "3N";
                    gameBoard[xDest, yDest + 1] = "3S";
                    gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";
                    gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";

                    selectedWall[0, 0] = xDest;
                    selectedWall[0, 1] = yDest;
                    selectedWall[1, 0] = xDest;
                    selectedWall[1, 1] = yDest + 1;
                }
                else if (gameBoard[xDest, yDest - 1].Contains('0'))
                {
                    gameBoard[xDest, yDest - 1] = "3N";
                    gameBoard[xDest, yDest] = "3S";
                    gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";
                    gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";

                    selectedWall[0, 0] = xDest;
                    selectedWall[0, 1] = yDest - 1;
                    selectedWall[1, 0] = xDest;
                    selectedWall[1, 1] = yDest;
                }
            }
            //if wall is currently WE
            else if (selectedWall[0, 1] == selectedWall[1, 1] || addWallActive)
            {
                if (gameBoard[xDest + 1, yDest].Contains('0'))
                {
                    gameBoard[xDest, yDest] = "3W";
                    gameBoard[xDest + 1, yDest] = "3E";
                    if (!addWallActive)
                    {
                        gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";
                        gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";
                    }
                    else
                    {
                        addWallActive = false;
                    }

                    selectedWall[0, 0] = xDest;
                    selectedWall[0, 1] = yDest;
                    selectedWall[1, 0] = xDest + 1;
                    selectedWall[1, 1] = yDest;
                }
                else if (gameBoard[xDest - 1, yDest].Contains('0'))
                {
                    gameBoard[xDest - 1, yDest] = "3W";
                    gameBoard[xDest, yDest] = "3E";
                    if (!addWallActive)
                    {
                        gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";
                        gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";
                    }
                    else
                    {
                        addWallActive = false;
                    }

                    selectedWall[0, 0] = xDest - 1;
                    selectedWall[0, 1] = yDest;
                    selectedWall[1, 0] = xDest;
                    selectedWall[1, 1] = yDest;
                }
            }

            checkWallPosition(originalPos);
        }
        public void rotateWall()
        {
            int[,] originalPos = { { selectedWall[0, 0], selectedWall[0, 1] }, { selectedWall[1, 0], selectedWall[1, 1] } };
            if (selectedWall[0, 0] != -2)
            {
                //if the wall is verical
                if (selectedWall[0, 0] == selectedWall[1, 0 ])
                {
                    if (gameBoard[selectedWall[0, 0] + 1, selectedWall[0, 1]] == "0")
                    {
                        gameBoard[selectedWall[0, 0] + 1, selectedWall[0, 1]] = "3E";
                        gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "3W";
                        gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";

                        selectedWall[1, 0] = selectedWall[0, 0] + 1;
                        selectedWall[1, 1] = selectedWall[0, 1];
                    }
                    else if (gameBoard[selectedWall[1, 0] - 1, selectedWall[1, 1]] == "0")
                    {
                        gameBoard[selectedWall[1, 0] - 1, selectedWall[1, 1]] = "3W";
                        gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "3E";
                        gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";

                        selectedWall[0, 0] = selectedWall[1, 0] - 1;
                        selectedWall[0, 1] = selectedWall[1, 1];
                    }
                }
                //if the wall is horizontal
                else if (selectedWall[0, 1] == selectedWall[1, 1])
                {
                    if (gameBoard[selectedWall[0, 0], selectedWall[0, 1] + 1] == "0")
                    {
                        gameBoard[selectedWall[0, 0], selectedWall[0, 1] + 1] = "3S";
                        gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "3N";
                        gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";

                        selectedWall[1, 0] = selectedWall[0, 0];
                        selectedWall[1, 1] = selectedWall[0, 1] + 1;
                    }
                    else if (gameBoard[selectedWall[1, 0], selectedWall[1, 1] - 1] == "0")
                    {
                        gameBoard[selectedWall[1, 0], selectedWall[1, 1] - 1] = "3N";
                        gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "3S";
                        gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";

                        selectedWall[0, 0] = selectedWall[1, 0];
                        selectedWall[0, 1] = selectedWall[1, 1] - 1;
                    }
                }
            }
            checkWallPosition(originalPos);
        }
        public void checkWallPosition(int[,] originalPos)
        {
            BoardSearch ppBFS = new BoardSearch(this);

            //if the wall was moved...
            //and blue pieces can reach the center...
            if (originalPos != selectedWall && !ppBFS.isOpponentTrapped(this, false))
            {
                //if wall was added
                if (originalPos[0, 0] > 30)
                {
                    wallPositionList.Add(((selectedWall[0, 0], selectedWall[0, 1]), (selectedWall[1, 0], selectedWall[1, 1])));
                }
                //if the wall was moved
                else
                {
                    wallPositionList.Remove(((originalPos[0, 0], originalPos[0, 1]), (originalPos[1, 0], originalPos[1, 1])));
                    wallPositionList.Add(((selectedWall[0, 0], selectedWall[0, 1]), (selectedWall[1, 0], selectedWall[1, 1])));
                }

                //if the wall was in a chokepoint but is now moved
                if (baseChokepointList.Contains(((originalPos[0, 0], originalPos[0, 1]), (originalPos[1, 0], originalPos[1, 1]))))
                {
                    availableChokepointList.Add(((originalPos[0, 0], originalPos[0, 1]), (originalPos[1, 0], originalPos[1, 1])));
                }

                if (availableChokepointList.Contains(((selectedWall[0, 0], selectedWall[0, 1]), (selectedWall[1, 0], selectedWall[1, 1]))))
                {
                    availableChokepointList.Remove(((selectedWall[0, 0], selectedWall[0, 1]), (selectedWall[1, 0], selectedWall[1, 1])));
                }
            }
            else if (!ppBFS.isOpponentTrapped(this, false))
            {
                gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";
                gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";
                if (originalPos[0, 0] > 30)
                {
                    selectedWall[0, 0] = originalPos[0, 0];
                }
                //if wall was NS
                else if (originalPos[0, 0] == originalPos[1, 0])
                {
                    gameBoard[originalPos[0, 0], originalPos[0, 1]] = "3N";
                    gameBoard[originalPos[1, 0], originalPos[1, 1]] = "3S";
                }
                //if wall was WE
                else if (selectedWall[0, 1] == selectedWall[1, 1] || addWallActive)
                {
                    gameBoard[originalPos[0, 0], originalPos[0, 1]] = "3W";
                    gameBoard[originalPos[1, 0], originalPos[1, 1]] = "3E";
                }
                selectedWall[0, 0] = originalPos[0, 0];
                selectedWall[0, 1] = originalPos[0, 1];
                selectedWall[1, 0] = originalPos[1, 0];
                selectedWall[1, 1] = originalPos[1, 1];
            }
        }
        public void checkPieceWallCollision(int xPos, int yPos, (int, int) prevPos)
        {
            //if a piece moves out of a chokepoint
            if (nwChokepointList.Contains(prevPos))
            {
                availableChokepointList.Add((prevPos, seChokepointList[nwChokepointList.IndexOf(prevPos)]));
            }
            else if (seChokepointList.Contains(prevPos))
            {
                availableChokepointList.Add((prevPos, nwChokepointList[seChokepointList.IndexOf(prevPos)]));
            }

            //if the new position is in a chokepoint
            if (xPos != -1)
            {
                if (nwChokepointList.Contains((xPos, yPos)))
                {
                    availableChokepointList.Remove(((xPos, yPos), seChokepointList[nwChokepointList.IndexOf((xPos, yPos))]));
                }
                else if (seChokepointList.Contains((xPos, yPos)))
                {
                    availableChokepointList.Remove(((xPos, yPos), nwChokepointList[seChokepointList.IndexOf((xPos, yPos))]));
                }
            }
        }
        //return true if mino hits a player piece false otherwise
        public bool minoCollisionCheck(int xDest, int yDest)
        {
            if (gameBoard[xDest, yDest].Contains('4'))
            {
                if (gameBoard[xDest, yDest] == "4R") 
                {
                    if ((xDest, yDest) == (redPiece1[0], redPiece1[1]))
                    {
                        redPiece1[0] = 1;
                        redPiece1[1] = 0;
                    }
                    else if ((xDest, yDest) == (redPiece2[0], redPiece2[1]))
                    {
                        redPiece2[0] = 0;
                        redPiece2[1] = 0;

                    }
                    else if ((xDest, yDest) == (redPiece3[0], redPiece3[1]))
                    {
                        redPiece3[0] = 0;
                        redPiece3[1] = 1;
                    }
                }
                else
                {
                    if ((xDest, yDest) == (bluePiece1[0], bluePiece1[1]))
                    {
                        bluePiece1[0] = 29;
                        bluePiece1[1] = 28;
                    }
                    else if ((xDest, yDest) == (bluePiece2[0], bluePiece2[1]))
                    {
                        bluePiece2[0] = 29;
                        bluePiece2[1] = 29;
                    }
                    else if ((xDest, yDest) == (bluePiece3[0], bluePiece3[1]))
                    {
                        bluePiece3[0] = 28;
                        bluePiece3[1] = 29;
                    }
                }
                checkPieceWallCollision(-1, -1, (minotuarPos[0], minotuarPos[0]));
                gameBoard[minotuarPos[0], minotuarPos[1]] = "0";
                availableMoves = 0;
                minotuarPos = new int[] { -5, -5};
                isBlackActive = false;
                return true;
            }
            return false;
        }
        public void endTurn()
        {
            if (diceRolled && availableMoves == 0)
            {
                isRedTurn = !isRedTurn;
                diceRolled = false;
                selectedPiece = "";
                isGreyActive = false;
                selectedWall[0, 0] = -2;
                selectedWall[0, 1] = -2;
            }
        }

        //deep-copy
        public GameBoard deepCopy()
        {
            bool diceRoll = this.diceRolled;
            List<int[]> playerPieceList = new List<int[]>()
            {
                new int[] { redPiece1[0], redPiece1[1] },
                new int[] { redPiece2[0], redPiece2[1] },
                new int[] { redPiece3[0], redPiece3[1] },
                new int[] { bluePiece1[0], bluePiece1[1] },
                new int[] { bluePiece2[0], bluePiece2[1] },
                new int[] { bluePiece3[0], bluePiece3[1] },
            };
            int[] minoPos = { minotuarPos[0], minotuarPos[1] };
            int[,] selWall = { { selectedWall[0,0] , selectedWall[0, 1] }, { selectedWall[1, 0], selectedWall[1, 1] } };
            string[,] newGameBoard = new string[30, 30];
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    newGameBoard[i,j] = gameBoard[i,j].ToString();
                }
            }
            List<((int, int), (int, int))> newCPList = new List<((int, int), (int, int))>();
            foreach(((int, int), (int, int)) chokepoint in availableChokepointList)
            {
                newCPList.Add(chokepoint);
            }
            List<((int, int), (int, int))> newWallPosList = new List<((int, int), (int, int))>();
            for (int l = 0; l < wallPositionList.Count; l++)
            {
                newWallPosList.Add(wallPositionList[l]);
            }
            bool irt = isRedTurn;
            bool vicR = redVictory;
            bool vicB = blueVictory;
            bool iga = isGreyActive;
            bool iba = isBlackActive;

            string selPiece = selectedPiece;
            int am = availableMoves;

            GameBoard CoppiedBoard = new GameBoard(diceRoll, playerPieceList, minoPos, selWall, newGameBoard, newCPList,
                                                   newWallPosList, irt, vicR, vicB, iga, iba, selPiece, am);
            return CoppiedBoard;
        }
    }
}
