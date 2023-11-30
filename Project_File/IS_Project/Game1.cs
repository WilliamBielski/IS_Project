using IS_Project.Managers;
using System;
using System.Linq;

namespace IS_Project
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private GameManager _gameManager;

        //const int gridSize = 30;
        //const int tileSize = 48;

        //MouseState mouseState;
        //bool mReleased = true;
        //bool diceRolled = false;
        //int heldKey = 0;

        ////Texture2D sprites;
        //Texture2D hedgeTile; //48x48 tile
        //Texture2D pathTile; //48x48 tile
        //Texture2D topWallTile;
        //Texture2D bottomWallTile;
        //Texture2D leftWallTile;
        //Texture2D rightWallTile;
        //Texture2D redTile;
        //Texture2D redPlayerPiece;
        //Texture2D blueTile;
        //Texture2D bluePlayerPiece;
        //Texture2D minotaur;

        //SpriteFont basicFont;

        //int[] redPiece1 = {4, 13};
        //int[] redPiece2 = { 4, 4 };
        //int[] redPiece3 = { 10, 8 };

        //int[] bluePiece1 = { 24, 11 };
        //int[] bluePiece2 = { 21, 18 };
        //int[] bluePiece3 = { 20, 25 };

        //double[] minotuarPos = { 14.5, 14.5 };

        //int[,] selectedWall = { { -2, -2 },{ -2, -2 } };

        //string[,] gameBoard = { { "5R", "5R", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "1"},
        //                        { "5R", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1"},
        //                        { "0", "0", "0", "0", "0", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "0", "0", "0", "0", "0"},
        //                        { "0", "0", "3W", "0", "0", "1", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "1", "0", "0", "3W", "0", "0"},
        //                        { "0", "0", "3E", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "3E", "0", "0"},
        //                        { "0", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "0", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "0", "0", "0", "0", "0", "0", "0", "0"},
        //                        { "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0"},
        //                        { "0", "0", "1", "1", "1", "1", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "1", "0", "0"},
        //                        { "0", "0", "0", "0", "0", "1", "0", "0", "3N", "3S", "0", "0", "1", "1", "0", "0", "1", "1", "0", "0", "3N", "3S", "0", "0", "1", "0", "0", "0", "0", "0"},
        //                        { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
        //                        { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
        //                        { "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0"},
        //                        { "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0"},
        //                        { "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "5R", "5R", "1", "1", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0"},
        //                        { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "5R", "G", "G", "1", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
        //                        { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "1", "G", "G", "5B", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
        //                        { "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "1", "1", "5B", "5B", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0"},
        //                        { "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "1", "0", "0"},
        //                        { "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0"},
        //                        { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
        //                        { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
        //                        { "0", "0", "0", "0", "0", "1", "0", "0", "3N", "3S", "0", "0", "1", "1", "0", "0", "1", "1", "0", "0", "3N", "3S", "0", "0", "1", "0", "0", "0", "0", "0"},
        //                        { "0", "0", "1", "1", "1", "1", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "1", "0", "0"},
        //                        { "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0"},
        //                        { "0", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "0", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "0", "0", "0", "0", "0", "0", "0", "0"},
        //                        { "0", "0", "3W", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "3W", "0", "0"},
        //                        { "0", "0", "3E", "0", "0", "1", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "1", "0", "0", "3E", "0", "0"},
        //                        { "0", "0", "0", "0", "0", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "1", "0", "0", "1", "1", "1", "0", "0", "0", "0", "0"},
        //                        { "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "5B"},
        //                        { "1", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "5B", "5B"}
        //                    };
        //string[] impassableList = { "1", "3N", "3S", "3E", "3W", "6" };
        //string[] passable = { "0", "4" };

        //bool isRedTurn = true;

        //bool isGreyActive = false;
        //bool isBlackActive = false;
        //bool addWallActive = false;

        //string selectedPiece = "";

        //int availableMoves = 0;

        //int mPosX;
        //int mPosY;

        ////for testing
        //string clicked = "";

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1940;
            _graphics.PreferredBackBufferHeight = 1440;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Globals.Content = Content;

            _gameManager = new();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.SpriteBatch = _spriteBatch;

            // TODO: use this.Content to load your game content here
            //hedgeTile = Content.Load<Texture2D>("hedge-tile");
            //pathTile = Content.Load<Texture2D>("path-tile");
            //topWallTile = Content.Load<Texture2D>("wall-tile-top");
            //bottomWallTile = Content.Load<Texture2D>("wall-tile-bottom");
            //leftWallTile = Content.Load<Texture2D>("wall-tile-left");
            //rightWallTile = Content.Load<Texture2D>("wall-tile-right");
            //redTile = Content.Load<Texture2D>("red-player-tile");
            //redPlayerPiece = Content.Load<Texture2D>("red-player-piece");
            //blueTile = Content.Load<Texture2D>("blue-player-tile");
            //bluePlayerPiece = Content.Load<Texture2D>("blue-player-piece");
            //minotaur = Content.Load<Texture2D>("minotuar-tile");

            //basicFont = Content.Load<SpriteFont>("galleryFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Globals.Update(gameTime);
            _gameManager.Update();
            // TODO: Add your update logic here
            //mouseState = Mouse.GetState();
            //mPosX = mouseState.Position.X / 48;
            //mPosY = mouseState.Position.Y / 48;

            ////if the mouse selects a piece
            //if (mouseState.LeftButton == ButtonState.Pressed && mReleased)
            //{
            //    clicked = mPosX < 30 ? gameBoard[mPosX, mPosY] : "";
            //    //if dice roll selected 
            //    if (Vector2.Distance(new Vector2(1524, 114), mouseState.Position.ToVector2()) < 24 && !diceRolled)
            //    {
            //        diceRolled = true;
            //        Random rnd = new Random();
            //        int num = rnd.Next(1,7);
            //        switch (num)
            //        {
            //            //case 1:
            //            //case 2:
            //            //case 3:
            //            //case 4:
            //            //case 5:
            //            case 1:
            //                selectedPiece = "mino";
            //                availableMoves = 8;
            //                break;


            //            //case 1:
            //            case 3:
            //            case 4:
            //            case 5:
            //            case 6:
            //            case 2:
            //                selectedPiece = "grey";
            //                isGreyActive = true;
            //                break;

            //            default:
            //                availableMoves = num;
            //                break;
            //        }

            //    }

            //    //if black rolled
            //    else if (!isBlackActive && selectedPiece == "mino")
            //    {
            //        if(((mPosX == 12 || mPosX == 17) && (mPosY > 11 && mPosY < 18)) || ((mPosX > 12 && mPosX < 17) && (mPosY == 12 || mPosY == 17)))
            //        {
            //            minotuarPos[0] = mPosX;
            //            minotuarPos[1] = mPosY;
            //            gameBoard[(int)minotuarPos[0], (int)minotuarPos[1]] = "6";
            //            isBlackActive = true;
            //        }
            //    }

            //    //if a movable wall is selected after a Grey has been "rolled" the selectedWall variable will be set to the possition of the two wall-tiles
            //    //N and W will always be the first wall in selected wall
            //    else if (isGreyActive && selectedWall[0,0] == -2)
            //    {
            //        //gets the movable wall coordinates into the selected wall variable correctly
            //        if ((mPosX, mPosY) == (31, 13) || (mPosX, mPosY) == (32, 13))
            //        {
            //            addWallActive = true;
            //            selectedWall[0,0] = mPosX;
            //            clicked = "Add Wall";
            //        }
            //        else if(mPosX < 30 && gameBoard[mPosX, mPosY].Contains('3'))
            //        {
            //            if (gameBoard[mPosX, mPosY].Contains('N'))
            //            {
            //                selectedWall[0, 0] = mPosX;
            //                selectedWall[0, 1] = mPosY;
            //                selectedWall[1, 0] = mPosX;
            //                selectedWall[1, 1] = mPosY + 1;
            //            }
            //            else if (gameBoard[mPosX, mPosY].Contains('S'))
            //            {
            //                selectedWall[0, 0] = mPosX;
            //                selectedWall[0, 1] = mPosY - 1;
            //                selectedWall[1, 0] = mPosX;
            //                selectedWall[1, 1] = mPosY;
            //            }
            //            else if (gameBoard[mPosX, mPosY].Contains('E'))
            //            {
            //                selectedWall[0, 0] = mPosX - 1;
            //                selectedWall[0, 1] = mPosY;
            //                selectedWall[1, 0] = mPosX;
            //                selectedWall[1, 1] = mPosY;
            //            }
            //            else if (gameBoard[mPosX, mPosY].Contains('W'))
            //            {
            //                selectedWall[0, 0] = mPosX;
            //                selectedWall[0, 1] = mPosY;
            //                selectedWall[1, 0] = mPosX + 1;
            //                selectedWall[1, 1] = mPosY;
            //            };
            //        }
                    
            //    }

            //    //if a wall is selected and the mouse clicks the rotate wall "button" @ 31,9
            //    //the wall will try to rotate (hopefully)
            //    else if (mPosX == 31 && mPosY == 9 && selectedWall[0,1] != -2 && selectedWall[0,0] < 30)
            //    {
            //        rotateWall();
            //    }

            //    //if a wall is selected (implying isGreyActive is true) and the user clicks to move
            //    else if (selectedWall[0, 0] != -2 && (mPosX < 30 && gameBoard[mPosX, mPosY].Contains('0')))
            //    {
            //        //if statements for collision:

            //        //if wall is currently NS
            //        if (selectedWall[0,0] == selectedWall[1,0])
            //        {
            //            if (gameBoard[mPosX, mPosY+1].Contains('0'))
            //            {
            //                gameBoard[mPosX, mPosY] = "3N";
            //                gameBoard[mPosX, mPosY + 1] = "3S";
            //                gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";
            //                gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";

            //                selectedWall[0,0] = mPosX;
            //                selectedWall[0, 1] = mPosY;
            //                selectedWall[1, 0] = mPosX;
            //                selectedWall[1, 1] = mPosY + 1;
            //            }
            //            else if (gameBoard[mPosX, mPosY-1].Contains('0'))
            //            {
            //                gameBoard[mPosX, mPosY - 1] = "3N";
            //                gameBoard[mPosX, mPosY] = "3S";
            //                gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";
            //                gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";

            //                selectedWall[0, 0] = mPosX;
            //                selectedWall[0, 1] = mPosY - 1;
            //                selectedWall[1, 0] = mPosX;
            //                selectedWall[1, 1] = mPosY;
            //            }
            //        }
            //        //if wall is currently WE
            //        else if (selectedWall[0,1] == selectedWall[1,1] || addWallActive)
            //        {
            //            if (gameBoard[mPosX + 1, mPosY].Contains('0'))
            //            {
            //                gameBoard[mPosX, mPosY] = "3W";
            //                gameBoard[mPosX + 1, mPosY] = "3E";
            //                if (!addWallActive)
            //                {
            //                    gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";
            //                    gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";
            //                }
            //                else
            //                {
            //                    addWallActive = false;
            //                }

            //                selectedWall[0, 0] = mPosX;
            //                selectedWall[0, 1] = mPosY;
            //                selectedWall[1, 0] = mPosX + 1;
            //                selectedWall[1, 1] = mPosY;
            //            }
            //            else if (gameBoard[mPosX - 1, mPosY].Contains('0'))
            //            {
            //                gameBoard[mPosX - 1, mPosY] = "3W";
            //                gameBoard[mPosX, mPosY] = "3E";
            //                if (!addWallActive)
            //                {
            //                    gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";
            //                    gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";
            //                }
            //                else
            //                {
            //                    addWallActive = false;
            //                }

            //                selectedWall[0, 0] = mPosX - 1;
            //                selectedWall[0, 1] = mPosY;
            //                selectedWall[1, 0] = mPosX;
            //                selectedWall[1, 1] = mPosY;
            //            }
            //        }

            //    }

            //    //for player piece selection (not mino)
            //    else if (selectedPiece == "")
            //    {
            //        if (Vector2.Distance(new Vector2((redPiece1[0] * tileSize) + 24, (redPiece1[1] * tileSize) + 24), mouseState.Position.ToVector2()) < 24 && isRedTurn)
            //        {
            //            selectedPiece = "red1";
            //        }
            //        else if (Vector2.Distance(new Vector2((redPiece2[0] * tileSize) + 24, (redPiece2[1] * tileSize) + 24), mouseState.Position.ToVector2()) < 24 && isRedTurn)
            //        {
            //            selectedPiece = "red2";
            //        }
            //        else if (Vector2.Distance(new Vector2((redPiece3[0] * tileSize) + 24, (redPiece3[1] * tileSize) + 24), mouseState.Position.ToVector2()) < 24 && isRedTurn)
            //        {
            //            selectedPiece = "red3";
            //        }
            //        else if (Vector2.Distance(new Vector2((bluePiece1[0] * tileSize) + 24, (bluePiece1[1] * tileSize) + 24), mouseState.Position.ToVector2()) < 24 && !isRedTurn)
            //        {
            //            selectedPiece = "blue1";
            //        }
            //        else if (Vector2.Distance(new Vector2((bluePiece2[0] * tileSize) + 24, (bluePiece2[1] * tileSize) + 24), mouseState.Position.ToVector2()) < 24 && !isRedTurn)
            //        {
            //            selectedPiece = "blue2";
            //        }
            //        else if (Vector2.Distance(new Vector2((bluePiece3[0] * tileSize) + 24, (bluePiece3[1] * tileSize) + 24), mouseState.Position.ToVector2()) < 24 && !isRedTurn)
            //        {
            //            selectedPiece = "blue3";
            //        }
            //    }
            //    mReleased = false;
            //}

            ////if there is a selected piece (movement)
            //if (selectedPiece != "" && availableMoves > 0 && heldKey == 0)
            //{
            //    if (Keyboard.GetState().IsKeyDown(Keys.Up))
            //    {
            //        if (applyMove("up"))
            //        {
            //            availableMoves--;
            //        }
            //        heldKey = 1;
            //    }
            //    else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            //    {
            //        if (applyMove("down"))
            //        {
            //            availableMoves--;
            //        }
            //        heldKey = 2;
            //    }
            //    else if (Keyboard.GetState().IsKeyDown(Keys.Left))
            //    {
            //        if (applyMove("left"))
            //        {
            //            availableMoves--;
            //        }
            //        heldKey = 3;
            //    }
            //    else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            //    {
            //        if (applyMove("right"))
            //        {
            //            availableMoves--;
            //        }
            //        heldKey = 4;
            //    }

            //    //mino collision conditions
            //    if(selectedPiece == "mino")
            //    {
            //        if ((minotuarPos[0], minotuarPos[1]) == (redPiece1[0],redPiece1[1]))
            //        {
            //            redPiece1[0] = 1;
            //            redPiece1[1] = 0;
            //            minotuarPos[0] = 14.5;
            //            minotuarPos[1] = 14.5;
            //            isBlackActive = false;
            //        }
            //        else if ((minotuarPos[0], minotuarPos[1]) == (redPiece2[0], redPiece2[1]))
            //        {
            //            redPiece2[0] = 0;
            //            redPiece2[1] = 0;
            //            minotuarPos[0] = 14.5;
            //            minotuarPos[1] = 14.5;
            //            isBlackActive = false;

            //        }
            //        else if ((minotuarPos[0], minotuarPos[1]) == (redPiece3[0], redPiece3[1]))
            //        {
            //            redPiece3[0] = 0;
            //            redPiece3[1] = 1;
            //            minotuarPos[0] = 14.5;
            //            minotuarPos[1] = 14.5;
            //            isBlackActive = false;
            //        }
            //        else if ((minotuarPos[0], minotuarPos[1]) == (bluePiece1[0], bluePiece1[1]))
            //        {
            //            bluePiece1[0] = 29;
            //            bluePiece1[1] = 28;
            //            minotuarPos[0] = 14.5;
            //            minotuarPos[1] = 14.5;
            //            isBlackActive = false;
            //        }
            //        else if ((minotuarPos[0], minotuarPos[1]) == (bluePiece2[0], bluePiece2[1]))
            //        {
            //            bluePiece2[0] = 29;
            //            bluePiece2[1] = 29;
            //            minotuarPos[0] = 14.5;
            //            minotuarPos[1] = 14.5;
            //            isBlackActive = false;
            //        }
            //        else if ((minotuarPos[0], minotuarPos[1]) == (bluePiece3[0], bluePiece3[1]))
            //        {
            //            bluePiece3[0] = 28;
            //            bluePiece3[1] = 29;
            //            minotuarPos[0] = 14.5;
            //            minotuarPos[1] = 14.5;
            //            isBlackActive = false;
            //        }
            //    }
            //}

            ////movement for wall (arrow keys)
            //if(isGreyActive && !addWallActive && heldKey == 0)
            //{
            //    if (Keyboard.GetState().IsKeyDown(Keys.Up))
            //    {
            //        moveWall("up");
            //        heldKey = 1;
            //    }
            //    else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            //    {
            //        moveWall("down");
            //        heldKey = 2;
            //    }
            //    else if (Keyboard.GetState().IsKeyDown(Keys.Left))
            //    {
            //        moveWall("left");
            //        heldKey = 3;
            //    }
            //    else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            //    {
            //        moveWall("right");
            //        heldKey = 4;
            //    }
            //}

            ////logic for end-turn key (enter key)
            //if (selectedPiece != "" && availableMoves == 0)
            //{
            //    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            //    {
            //        isRedTurn = !isRedTurn;
            //        selectedPiece = "";
            //        diceRolled = false;
            //        selectedWall[0, 0] = -2;
            //        selectedWall[0, 1] = -2;

            //    }
            //}

            ////to ensure mouse 1 is released
            //if (mouseState.LeftButton == ButtonState.Released)
            //{
            //    mReleased = true;
            //}

            ////logic to prevent a held input (keyboard)
            //switch (heldKey)
            //{
            //    //up
            //    case 1:
            //        if (Keyboard.GetState().IsKeyUp(Keys.Up))
            //        {
            //            heldKey = 0;
            //        }
            //        break;
            //    //udown
            //    case 2:
            //        if (Keyboard.GetState().IsKeyUp(Keys.Down))
            //        {
            //            heldKey = 0;
            //        }
            //        break;
            //    //up
            //    case 3:
            //        if (Keyboard.GetState().IsKeyUp(Keys.Left))
            //        {
            //            heldKey = 0;
            //        }
            //        break;
            //    //up
            //    case 4:
            //        if (Keyboard.GetState().IsKeyUp(Keys.Right))
            //        {
            //            heldKey = 0;
            //        }
            //        break;
            //    case 0:
            //        break;
            //}



            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _gameManager.Draw();
            //_spriteBatch.Draw(spriteName, new Vector2(0,0), Color.Wheat);

            //sets up the playing board grid
            //for (int i = 0; i < gridSize; ++i)
            //{
            //    for (int j = 0; j < gridSize; ++j)
            //    {
            //        switch (gameBoard[i, j])
            //        {
            //            //path sprite
            //            case "0":
            //                _spriteBatch.Draw(pathTile, new Vector2(i * tileSize, j * tileSize), Color.White);
            //                break;

            //            //hedge sprite
            //            case "1":
            //                _spriteBatch.Draw(hedgeTile, new Vector2(i * tileSize, j * tileSize), Color.White);
            //                break;

            //            //movable wall sprites
            //            case "3N":
            //                _spriteBatch.Draw(topWallTile, new Vector2(i * tileSize, j * tileSize), Color.White);
            //                break;
            //            case "3S":
            //                _spriteBatch.Draw(bottomWallTile, new Vector2(i * tileSize, j * tileSize), Color.White);
            //                break;
            //            case "3E":
            //                _spriteBatch.Draw(rightWallTile, new Vector2(i * tileSize, j * tileSize), Color.White);
            //                break;
            //            case "3W":
            //                _spriteBatch.Draw(leftWallTile, new Vector2(i * tileSize, j * tileSize), Color.White);
            //                break;

            //            //player sprites

            //            case "5R":
            //                _spriteBatch.Draw(redTile, new Vector2(i * tileSize, j * tileSize), Color.White);
            //                break;

            //            case "5B":
            //                _spriteBatch.Draw(blueTile, new Vector2(i * tileSize, j * tileSize), Color.White);
            //                break;

            //        }
            //    }
            //}

            ////sets-up player peices
            //_spriteBatch.Draw(redPlayerPiece, new Vector2(redPiece1[0]*tileSize, redPiece1[1]*tileSize), Color.White);
            //_spriteBatch.Draw(redPlayerPiece, new Vector2(redPiece2[0] * tileSize, redPiece2[1] * tileSize), Color.White);
            //_spriteBatch.Draw(redPlayerPiece, new Vector2(redPiece3[0] * tileSize, redPiece3[1] * tileSize), Color.White);

            //_spriteBatch.Draw(bluePlayerPiece, new Vector2(bluePiece1[0] * tileSize, bluePiece1[1] * tileSize), Color.White);
            //_spriteBatch.Draw(bluePlayerPiece, new Vector2(bluePiece2[0] * tileSize, bluePiece2[1] * tileSize), Color.White);
            //_spriteBatch.Draw(bluePlayerPiece, new Vector2(bluePiece3[0] * tileSize, bluePiece3[1] * tileSize), Color.White);

            //_spriteBatch.DrawString(basicFont, "selected piece:" + selectedPiece, new Vector2(1465, 0), Color.Black);
            //_spriteBatch.DrawString(basicFont, "moves:" + availableMoves, new Vector2(1465, 30), Color.Black);
            //_spriteBatch.DrawString(basicFont, "Red Turn:" + isRedTurn, new Vector2(1465, 60), Color.Black);
            //_spriteBatch.DrawString(basicFont, "Mouse on Grid:" + mPosX +" , "+ mPosY, new Vector2(1465, 160), Color.Black);

            //_spriteBatch.DrawString(basicFont, "Just Clicked: " + clicked, new Vector2(1465, 200), Color.Black);

            //_spriteBatch.DrawString(basicFont, "Selected wall: {" + selectedWall[0, 0] + "," + selectedWall[0,1]+"}, {" + selectedWall[1, 0] + "," + selectedWall[1, 1] + "}", new Vector2(1465, 260), Color.Black);

            //_spriteBatch.Draw(leftWallTile, new Vector2(1500, 90), Color.Honeydew);

            //_spriteBatch.DrawString(basicFont, "ROTATE WALL", new Vector2(1488, 384), Color.Black);
            //_spriteBatch.Draw(pathTile, new Vector2(1488, 432), Color.Honeydew);

            //_spriteBatch.DrawString(basicFont, "ADD WALL", new Vector2(1488, 576), Color.Black);
            //_spriteBatch.Draw(leftWallTile, new Vector2(1488, 624), Color.White);
            //_spriteBatch.Draw(rightWallTile, new Vector2(1536, 624), Color.White);

            //_spriteBatch.DrawString(basicFont, "isBlackActive:"+ isBlackActive, new Vector2(1488, 500), Color.Black);
            //_spriteBatch.DrawString(basicFont, "isGreyActive:" + isGreyActive, new Vector2(1488, 540), Color.Black);
            //_spriteBatch.DrawString(basicFont, "addWallActive:" + addWallActive, new Vector2(1488, 680), Color.Black);

            //_spriteBatch.Draw(minotaur, new Vector2((float)minotuarPos[0] * tileSize, (float)minotuarPos[1] * tileSize), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        //bool applyMove(string direction)
        //{

        //    int[] curPiece = redPiece1;
        //    switch (selectedPiece)
        //    {
        //        case "red1":
        //            curPiece = redPiece1; 
        //            break;

        //        case "red2":
        //            curPiece = redPiece2;
        //            break;

        //        case "red3":
        //            curPiece = redPiece3;
        //            break;


        //        case "blue1":
        //            curPiece = bluePiece1;
        //            break;

        //        case "blue2":
        //            curPiece = bluePiece2;
        //            break;

        //        case "blue3":
        //            curPiece = bluePiece3;
        //            break;

        //        case "mino":
        //            switch (direction)
        //            {
        //                case "up":
        //                    if (minotuarPos[1] > 0 && passable.Contains(gameBoard[(int)minotuarPos[0], (int)minotuarPos[1] - 1]))
        //                    {
        //                        gameBoard[(int)minotuarPos[0], (int)minotuarPos[1] - 1] = "6";
        //                        gameBoard[(int)minotuarPos[0], (int)minotuarPos[1]] = "0";
        //                        minotuarPos[1]--;
        //                        return true;
        //                    }
        //                    break;
        //                case "down":
        //                    if (minotuarPos[1] < 29 && passable.Contains(gameBoard[(int)minotuarPos[0], (int)minotuarPos[1] + 1]))
        //                    {
        //                        gameBoard[(int)minotuarPos[0], (int)minotuarPos[1] + 1] = "6";
        //                        gameBoard[(int)minotuarPos[0], (int)minotuarPos[1]] = "0";
        //                        minotuarPos[1]++;
        //                        return true;
        //                    }
        //                    break;
        //                case "left":
        //                    if (minotuarPos[0] > 0 && passable.Contains(gameBoard[(int)minotuarPos[0] - 1, (int)minotuarPos[1]]))
        //                    {
        //                        gameBoard[(int)minotuarPos[0] - 1, (int)minotuarPos[1]] = "6";
        //                        gameBoard[(int)minotuarPos[0], (int)minotuarPos[1]] = "0";
        //                        minotuarPos[0]--;
        //                        return true;
        //                    }
        //                    break;
        //                case "right":
        //                    if (minotuarPos[0] < 29 && passable.Contains(gameBoard[(int)minotuarPos[0] - 1, (int)minotuarPos[1]]))
        //                    {
        //                        gameBoard[(int)minotuarPos[0] + 1, (int)minotuarPos[1]] = "6";
        //                        gameBoard[(int)minotuarPos[0], (int)minotuarPos[1]] = "0";
        //                        minotuarPos[0]++;
        //                        return true;
        //                    }
        //                    break;
        //            }
        //            break;
        //    }

        //    switch (direction)
        //    {
        //        case "up":
        //            //used to do it this way
        //            //if (gameBoard[curPiece[0] + 1, curPiece[1]] == "5R" && gameBoard[curPiece[0], curPiece[1]] == "4")
        //            //{
        //            //    gameBoard[curPiece[0], curPiece[1]] = "0";
        //            //}
        //            //else if (gameBoard[curPiece[0] + 1, curPiece[1]] == "0")
        //            //{
        //            //    gameBoard[curPiece[0] + 1, curPiece[1]] = "4";
        //            //    gameBoard[curPiece[0], curPiece[1]] = "0";
        //            //}

        //            if (curPiece[1] > 0 && !impassableList.Contains(gameBoard[curPiece[0], curPiece[1] - 1]))
        //            {
        //                if (gameBoard[curPiece[0], curPiece[1]] == "4" && availableMoves > 0)
        //                {
        //                    gameBoard[curPiece[0], curPiece[1]] = "0";
        //                }
        //                else if (availableMoves == 1 && gameBoard[curPiece[0], curPiece[1] - 1] == "0")
        //                {
        //                    gameBoard[curPiece[0], curPiece[1] - 1] = "4";
        //                }
        //                else if (availableMoves == 1 && gameBoard[curPiece[0], curPiece[1] - 1] == "4")
        //                {
        //                    break;
        //                }

        //                curPiece[1]--;
        //                return true;
        //            }
        //            break;
        //        case "down":
        //            if (curPiece[1] < 29 && !impassableList.Contains(gameBoard[curPiece[0], curPiece[1] + 1]))
        //            {
        //                if (gameBoard[curPiece[0], curPiece[1]] == "4" && availableMoves > 0)
        //                {
        //                    gameBoard[curPiece[0], curPiece[1]] = "0";
        //                }
        //                else if (availableMoves == 1 && gameBoard[curPiece[0], curPiece[1] + 1] == "0")
        //                {
        //                    gameBoard[curPiece[0], curPiece[1] + 1] = "4";
        //                }
        //                else if (availableMoves == 1 && gameBoard[curPiece[0], curPiece[1] + 1] == "4")
        //                {
        //                    break;
        //                }

        //                curPiece[1]++;
        //                return true;
        //            }
        //            break;
        //        case "left":
        //            if (curPiece[0] > 0 && !impassableList.Contains(gameBoard[curPiece[0] - 1, curPiece[1]]))
        //            {
        //                if (gameBoard[curPiece[0], curPiece[1]] == "4" && availableMoves > 0)
        //                {
        //                    gameBoard[curPiece[0], curPiece[1]] = "0";
        //                }
        //                else if (availableMoves == 1 && gameBoard[curPiece[0] - 1, curPiece[1]] == "0")
        //                {
        //                    gameBoard[curPiece[0] - 1, curPiece[1]] = "4";
        //                }
        //                else if (availableMoves == 1 && gameBoard[curPiece[0] - 1, curPiece[1]] == "4")
        //                {
        //                    break;
        //                }
        //                curPiece[0]--;
        //                return true;
        //            }
        //            break;
        //        case "right":
        //            if (curPiece[0] < 29 && !impassableList.Contains(gameBoard[curPiece[0] + 1, curPiece[1]]))
        //            {
        //                if (gameBoard[curPiece[0], curPiece[1]] == "4" && availableMoves > 0)
        //                {
        //                    gameBoard[curPiece[0], curPiece[1]] = "0";
        //                }
        //                else if (availableMoves == 1 && gameBoard[curPiece[0] + 1, curPiece[1]] == "0")
        //                {
        //                    gameBoard[curPiece[0] + 1, curPiece[1]] = "4";
        //                }
        //                else if (availableMoves == 1 && gameBoard[curPiece[0] + 1, curPiece[1]] == "4")
        //                {
        //                    break;
        //                }
        //                curPiece[0]++;
        //                return true;
        //            }
        //            break;
        //    }

        //    return false;
        //}
        //void moveWall(string direction)
        //{
        //    switch (direction)
        //    {
        //        case "up":
        //            if (selectedWall[0, 1] > 0 && gameBoard[selectedWall[0, 0], selectedWall[0, 1] - 1] == "0" && (gameBoard[selectedWall[1, 0], selectedWall[1, 1] - 1] == "0" || gameBoard[selectedWall[1, 0], selectedWall[1, 1] - 1] == "3N"))
        //            {
        //                //N then S
        //                gameBoard[selectedWall[0, 0], selectedWall[0, 1] - 1] = gameBoard[selectedWall[0, 0], selectedWall[0, 1]];
        //                gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";
        //                gameBoard[selectedWall[1, 0], selectedWall[1, 1] - 1] = gameBoard[selectedWall[1, 0], selectedWall[1, 1]];
        //                gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";
      
        //                selectedWall[0, 1]--;
        //                selectedWall[1, 1]--;
        //            }
        //            break;
        //        case "down":
        //            if (selectedWall[1, 1] < 29 && gameBoard[selectedWall[1, 0], selectedWall[1, 1] + 1] == "0" && (gameBoard[selectedWall[0, 0], selectedWall[0, 1] + 1] == "0" || gameBoard[selectedWall[0, 0], selectedWall[0, 1] + 1] == "3S"))
        //            {
        //                //S then N
        //                gameBoard[selectedWall[1, 0], selectedWall[1, 1] + 1] = gameBoard[selectedWall[1, 0], selectedWall[1, 1]];
        //                gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";
        //                gameBoard[selectedWall[0, 0], selectedWall[0, 1] + 1] = gameBoard[selectedWall[0, 0], selectedWall[0, 1]];
        //                gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";

        //                selectedWall[0, 1]++;
        //                selectedWall[1, 1]++;
        //            }
        //            break;
        //        case "left":
        //            if (selectedWall[0, 0] > 0 && gameBoard[selectedWall[0, 0] - 1, selectedWall[0, 1]] == "0" && (gameBoard[selectedWall[1, 0] - 1, selectedWall[1, 1]] == "0" || gameBoard[selectedWall[1, 0] - 1, selectedWall[1, 1]] == "3W"))
        //            {
        //                //W then E
        //                gameBoard[selectedWall[0, 0] - 1, selectedWall[0, 1]] = gameBoard[selectedWall[0, 0], selectedWall[0, 1]];
        //                gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";
        //                gameBoard[selectedWall[1, 0] - 1, selectedWall[1, 1]] = gameBoard[selectedWall[1, 0], selectedWall[1, 1]];
        //                gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";
                        

        //                selectedWall[0, 0]--;
        //                selectedWall[1, 0]--;
        //            }
        //            break;
        //        case "right":
        //            if(selectedWall[1, 0] < 29 && gameBoard[selectedWall[1, 0] + 1, selectedWall[1, 1]] == "0" && (gameBoard[selectedWall[0, 0] + 1, selectedWall[0, 1]] == "0" || gameBoard[selectedWall[0, 0] + 1, selectedWall[0, 1]] == "3E"))
        //            {
        //                //E then W
        //                gameBoard[selectedWall[1, 0] + 1, selectedWall[1, 1]] = gameBoard[selectedWall[1, 0], selectedWall[1, 1]];
        //                gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";
        //                gameBoard[selectedWall[0, 0] + 1, selectedWall[0, 1]] = gameBoard[selectedWall[0, 0], selectedWall[0, 1]];
        //                gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";

        //                selectedWall[0, 0]++;
        //                selectedWall[1, 0]++;
        //            }
        //            break;
        //    }
        //}
        //void rotateWall()
        //{
        //    if (selectedWall[0,0] != -2)
        //    {
        //        //if the wall is verical
        //        if (gameBoard[selectedWall[0, 0], selectedWall[0, 1]] == "3N")
        //        {
        //            if(gameBoard[selectedWall[0, 0] + 1, selectedWall[0, 1]] == "0")
        //            {
        //                gameBoard[selectedWall[0, 0] + 1, selectedWall[0, 1]] = "3E";
        //                gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "3W";
        //                gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";

        //                selectedWall[1, 0] = selectedWall[0, 0] + 1;
        //                selectedWall[1, 1] = selectedWall[0, 1];
        //            }
        //            else if(gameBoard[selectedWall[1, 0] - 1, selectedWall[1, 1]] == "0")
        //            {
        //                gameBoard[selectedWall[1, 0] - 1, selectedWall[1, 1]] = "3W";
        //                gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "3E";
        //                gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";

        //                selectedWall[0, 0] = selectedWall[1, 0] - 1;
        //                selectedWall[0, 1] = selectedWall[1, 1];
        //            }
        //        }
        //        //if the wall is horizontal
        //        else if(gameBoard[selectedWall[0, 0], selectedWall[0, 1]] == "3W")
        //        {
        //            if (gameBoard[selectedWall[0, 0], selectedWall[0, 1] + 1] == "0")
        //            {
        //                gameBoard[selectedWall[0, 0], selectedWall[0, 1] + 1] = "3S";
        //                gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "3N";
        //                gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "0";

        //                selectedWall[1, 0] = selectedWall[0, 0];
        //                selectedWall[1, 1] = selectedWall[0, 1] + 1;
        //            }
        //            else if (gameBoard[selectedWall[1, 0], selectedWall[1, 1] - 1] == "0")
        //            {
        //                gameBoard[selectedWall[1, 0], selectedWall[1, 1] - 1] = "3N";
        //                gameBoard[selectedWall[1, 0], selectedWall[1, 1]] = "3S";
        //                gameBoard[selectedWall[0, 0], selectedWall[0, 1]] = "0";

        //                selectedWall[0, 0] = selectedWall[1, 0];
        //                selectedWall[0, 1] = selectedWall[1, 1] - 1;
        //            }
        //        }
        //    }
        //}
    }
}