using IS_Project.Models;
using Microsoft.Xna.Framework.Content;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace IS_Project.AI
{
    public class turnBasedAI
    {
        public GameBoard currentGameBoard;

        public turnBasedAI() { }
        public turnBasedAI(GameBoard gB)
        {
            currentGameBoard = gB;
        }
        public void Update()
        {
            if (!currentGameBoard.diceRolled)
            {
                currentGameBoard.rollDice();
                if(currentGameBoard.selectedPiece != "")
                {
                    currentGameBoard.LastBlueMove = currentGameBoard.selectedPiece;
                }
                else
                {
                    currentGameBoard.LastBlueMove = currentGameBoard.availableMoves.ToString();
                }
            }
            
            //what to do if the G or B is rolled
            //logic for rolling a "grey"
            if (currentGameBoard.selectedPiece == "grey")
            {
                List<((int, int), (int, int))> greyMove = getGreyDecision(currentGameBoard, 1);
                if (greyMove[0].Item1.Item1 == greyMove[0].Item2.Item1)
                {
                    currentGameBoard.gameBoard[greyMove[0].Item1.Item1, greyMove[0].Item1.Item2] = "3N";
                    currentGameBoard.gameBoard[greyMove[0].Item2.Item1, greyMove[0].Item2.Item2] = "3S";
                }
                //for WE
                else
                {
                    currentGameBoard.gameBoard[greyMove[0].Item1.Item1, greyMove[0].Item1.Item2] = "3W";
                    currentGameBoard.gameBoard[greyMove[0].Item2.Item1, greyMove[0].Item2.Item2] = "3E";
                }

                //if wall was moved not added
                if ((greyMove[1].Item1.Item1, greyMove[1].Item1.Item2) != (-2, -2))
                {
                    currentGameBoard.gameBoard[greyMove[1].Item1.Item1, greyMove[1].Item1.Item2] = "0";
                    currentGameBoard.gameBoard[greyMove[1].Item2.Item1, greyMove[1].Item2.Item2] = "0";
                    currentGameBoard.wallPositionList.Remove(greyMove[1]);
                }

                currentGameBoard.availableChokepointList.Remove(greyMove[0]);
                if (currentGameBoard.baseChokepointList.ContainsKey(greyMove[1]))
                {
                    currentGameBoard.availableChokepointList.Add(greyMove[1], greyMove[1]);
                }

                currentGameBoard.wallPositionList.Add(greyMove[0], greyMove[0]);
                //PlayerPieceBFS movementPath = new PlayerPieceBFS(currentGameBoard);
                //List<int> lengths = movementPath.pTester(currentGameBoard.bluePiece2[0], currentGameBoard.bluePiece2[1], "7B");
                currentGameBoard.endTurn();
            }
            //logic for rolling a "black"
            else if (currentGameBoard.selectedPiece == "mino")
            {
                if (!currentGameBoard.isBlackActive)
                {
                    currentGameBoard.minotuarPos[0] = 12;
                    currentGameBoard.minotuarPos[1] = 12;
                    currentGameBoard.gameBoard[currentGameBoard.minotuarPos[0], currentGameBoard.minotuarPos[1]] = "6";
                    currentGameBoard.isBlackActive = true;
                    currentGameBoard.availableMoves--;
                }
                else if (currentGameBoard.availableMoves > 0)
                {
                    BoardSearch movementPath = new BoardSearch(currentGameBoard);

                    List<(int, int)> minoPath = movementPath.BFS(currentGameBoard.minotuarPos[0], currentGameBoard.minotuarPos[1], "4R").dataList;

                    //assuming piece selected
                    if (minoPath.Count <= 8)
                    {
                        botPieceMovement(minoPath.LastOrDefault().Item1, minoPath.LastOrDefault().Item2, currentGameBoard, currentGameBoard.minotuarPos);
                    }
                    else
                    {
                        botPieceMovement(minoPath[7].Item1, minoPath[7].Item2, currentGameBoard, currentGameBoard.minotuarPos);
                    }
                    
                    currentGameBoard.availableMoves = 0;
                    currentGameBoard.endTurn();
                }
                else
                {
                    currentGameBoard.endTurn();
                    currentGameBoard.availableMoves = getUtility(currentGameBoard);
                }
            }
            //logic for moving game player piece
            else
            {
                if (currentGameBoard.selectedPiece == "")
                {
                    ((int, int),(int, int)) playerPieceMove = getPlayerPieceDecision(currentGameBoard, 0);
                    if (playerPieceMove.Item2 == (currentGameBoard.bluePiece1[0], currentGameBoard.bluePiece1[1]))
                    {
                        botPieceMovement(playerPieceMove.Item1.Item1, playerPieceMove.Item1.Item2, currentGameBoard, currentGameBoard.bluePiece1, true);
                    }
                    else if (playerPieceMove.Item2 == (currentGameBoard.bluePiece2[0], currentGameBoard.bluePiece2[1]))
                    {
                        botPieceMovement(playerPieceMove.Item1.Item1, playerPieceMove.Item1.Item2, currentGameBoard, currentGameBoard.bluePiece2, true);
                    }
                    else if (playerPieceMove.Item2 == (currentGameBoard.bluePiece3[0], currentGameBoard.bluePiece3[1]))
                    {
                        botPieceMovement(playerPieceMove.Item1.Item1, playerPieceMove.Item1.Item2, currentGameBoard, currentGameBoard.bluePiece3, true);
                    }
                    //currentGameBoard.movePieceByClick(playerPieceMove.Item1.Item1, playerPieceMove.Item1.Item2);
                    currentGameBoard.endTurn();
                }
                else
                {
                    currentGameBoard.endTurn();
                }
            }
            currentGameBoard.boardState = getUtility(currentGameBoard);
        }

        public int minMaxDecision(MinMaxNode minmaxNode, bool currentPLayerBlue, int depth)
        {
            if (depth == 0 || (minmaxNode.minmaxGameboard.redVictory || minmaxNode.minmaxGameboard.blueVictory))
            {
                return getUtility(minmaxNode.minmaxGameboard);
            }



            //for maxing
            if (currentPLayerBlue)
            {
                int bestValue = 0;
                getAllPossibleMoves(minmaxNode, true, minmaxNode.currentMove.Count != 2);
                foreach (MinMaxNode child in minmaxNode.children)
                {
                    int childValue = minMaxDecision(child, !currentPLayerBlue, (depth - 1));
                    bestValue = Math.Max(bestValue, childValue);
                    
                }
                return bestValue;
            }
            //for minimizing
            else
            {
                int bestValue = 0;
                getAllPossibleMoves(minmaxNode, false, minmaxNode.currentMove.Count != 2);
                foreach (MinMaxNode child in minmaxNode.children)
                {
                    int childValue = minMaxDecision(child, !currentPLayerBlue, (depth - 1));
                    bestValue = Math.Min(bestValue, childValue);
                }
                return bestValue;
            }
        }

        public int getUtility(GameBoard gameState)
        {
            BoardSearch pieceProximity = new BoardSearch(gameState);
            //proximity of all pieces to center (30 moves?)(minimum number of moves)
            int redDistanceToCenter = (getDistanceToCenterValue(false, gameState, pieceProximity) * 15);

            int blueDistanceToCenter = (getDistanceToCenterValue(true, gameState, pieceProximity) * 15);

            //player pieces within 8 of minotaur
            //[0] = number of red pieces in minotaur range
            //[1] = number of blue in minotaur range
            int[] inMinotaurRange = { 0, 0 };
            if (gameState.isBlackActive)
            {
                inMinotaurRange = pieceProximity.getMinotaurZoneBFS();
            }

            //length of possible paths
            int pathValues = getPathLengthValue(gameState, pieceProximity);

            int utility = (blueDistanceToCenter - redDistanceToCenter) + ((inMinotaurRange[1] - inMinotaurRange[0])*-100) + pathValues;
            return utility;
        }

        public int getUtilitySansPaths(GameBoard gameState, int allPathValues)
        {
            BoardSearch pieceProximity = new BoardSearch(gameState);
            //proximity of all pieces to center (30 moves?)(minimum number of moves)
            int redDistanceToCenter = getDistanceToCenterValue(false, gameState, pieceProximity);

            int blueDistanceToCenter = getDistanceToCenterValue(true, gameState, pieceProximity);

            //player pieces within 8 of minotaur
            //[0] = number of red pieces in minotaur range
            //[1] = number of blue in minotaur range
            int[] inMinotaurRange = { 0, 0 };
            if (gameState.isBlackActive)
            {
                inMinotaurRange = pieceProximity.getMinotaurZoneBFS();
            }

            int utility = ((blueDistanceToCenter - redDistanceToCenter) * 15) + ((inMinotaurRange[1] - inMinotaurRange[0]) * -100) + allPathValues * 5;
            return utility;
        }

        public int getDistanceToCenterValue(bool isBlueTurn, GameBoard gamestate, BoardSearch ppBFS)
        {
            int sum = 0;
            if (!isBlueTurn)
            {
                List<int[]> pieceList = new List<int[]>() { gamestate.redPiece1, gamestate.redPiece2, gamestate.redPiece3 };
                foreach (int[] piece in pieceList)
                {
                    if (gamestate.redVictoryTiles.Contains((piece[0], piece[1])))
                    {
                        sum += 70;
                    }
                    else
                    {
                        sum += 35 - ppBFS.BFS(piece[0], piece[1], "7R").dataList.Count;
                    }
                }
            }
            else
            {
                List<int[]> pieceList = new List<int[]>() { gamestate.bluePiece1, gamestate.bluePiece2, gamestate.bluePiece3 };
                foreach (int[] piece in pieceList)
                {
                    if (gamestate.blueVictoryTiles.Contains((piece[0], piece[1])))
                    {
                        sum += 70;
                    }
                    else
                    {
                        sum += 35 - ppBFS.BFS(piece[0], piece[1], "7B").dataList.Count;
                    }
                }
            }
            return sum;
            /*List<int> distances = ppBFS.getFastestPlayerPiecePathLengths(gamestate, isBlueTurn);
            if (distances.Contains(1))
            {
                foreach (int distance in distances)
                {
                    if (distance == 1)
                    {
                        sum += 70;
                    }
                    else
                    {
                        sum += 35 - distance;
                    }
                }
            }
            //35 - path length for each*/
            /*double maxDist = Math.Sqrt(Math.Pow(14.5, 2) + Math.Pow(14.5, 2));
            double distanceToCenter = Math.Sqrt(Math.Pow((curPiece[0] - 14.5), 2) + Math.Pow((curPiece[1] - 14.5), 2));
            return maxDist - distanceToCenter;*/
        }

        /*public int inMinotaurRange(int[] curPiece, PlayerPieceBFS minoProximity)
        {
            if (minoProximity._gameBoard.blueVictoryTiles.Contains((curPiece[0], curPiece[1])) || minoProximity._gameBoard.redVictoryTiles.Contains((curPiece[0], curPiece[1])))
            {
                return 0;
            }
            else if (minoProximity._gameBoard.isBlackActive && minoProximity.isObjInZoneBFS(curPiece[0], curPiece[1], "6", 8))
            {
                return -100;
            }
            else
            {
                return 0;
            }
        }*/

        public int getPathLengthValue(GameBoard gamestate, BoardSearch pathOptions)
        {
            int blueSum = ((35 - (pathOptions.getPathLength(gamestate.bluePiece1[0], gamestate.bluePiece1[1], "7B").Sum() / 3)) +
                           (35 - (pathOptions.getPathLength(gamestate.bluePiece2[0], gamestate.bluePiece2[1], "7B").Sum() / 3)) +
                           (35 - (pathOptions.getPathLength(gamestate.bluePiece3[0], gamestate.bluePiece3[1], "7B").Sum() / 3)));

            int redSum = ((35 - (pathOptions.getPathLength(gamestate.redPiece1[0], gamestate.redPiece1[1], "7R").Sum() / 3)) +
                          (35 - (pathOptions.getPathLength(gamestate.redPiece2[0], gamestate.redPiece2[1], "7R").Sum() / 3)) +
                          (35 - (pathOptions.getPathLength(gamestate.redPiece3[0], gamestate.redPiece3[1], "7R").Sum() / 3)));
            return blueSum - redSum;
        }


        public void getAllPossibleMoves(MinMaxNode minmaxNode, bool isBlueTurn, bool includeGrey)
        {
            //List<MinMaxNode> possibleMoves = new List<MinMaxNode>();

            if (isBlueTurn)
            {
                //adds player moves to the mix for 3 to 6
                for (int i = 3; i <= 6; i++)
                {
                    //List<MinMaxNode> j = generatePlayerPieceMoves(i, isBlueTurn, minmaxNode.minmaxGameboard);
                    minmaxNode.children.AddRange(generatePlayerPieceMoves(i, isBlueTurn, minmaxNode.minmaxGameboard));
                }
            }
            else
            {
                minmaxNode.children.AddRange(generatePlayerPieceMoves(6, isBlueTurn, minmaxNode.minmaxGameboard));
            }

            if (includeGrey)
            {
                //adds all realistic wall moves
                foreach (MinMaxNode board in generateGreyMoves(minmaxNode.minmaxGameboard, isBlueTurn))
                {
                    minmaxNode.children.Add(board);
                }
            }

            //adds Minotaur move
            MinMaxNode minoMinmaxNode = new MinMaxNode(minmaxNode.minmaxGameboard);
            BoardSearch possibleMovesPath = new BoardSearch(minoMinmaxNode.minmaxGameboard);
            List<(int, int)> _path;
            minoMinmaxNode.minmaxGameboard.availableMoves = 8;
            if (!minoMinmaxNode.minmaxGameboard.isBlackActive)
            {
                int startPos = isBlueTurn ? 12 : 17;
                minoMinmaxNode.minmaxGameboard.minotuarPos[0] = startPos;
                minoMinmaxNode.minmaxGameboard.minotuarPos[1] = startPos;
            }
            _path = possibleMovesPath.BFS(minoMinmaxNode.minmaxGameboard.minotuarPos[0], minoMinmaxNode.minmaxGameboard.minotuarPos[1], isBlueTurn ? "4R" : "4B").dataList;

            if (_path.Count <= 8)
            {
                botPieceMovement(_path.LastOrDefault().Item1, _path.LastOrDefault().Item2, minoMinmaxNode.minmaxGameboard, minoMinmaxNode.minmaxGameboard.minotuarPos);
            }
            else
            {
                botPieceMovement(_path[7].Item1, _path[7].Item2, minoMinmaxNode.minmaxGameboard, minoMinmaxNode.minmaxGameboard.minotuarPos);
            }
            minmaxNode.children.Add(minoMinmaxNode);

            //return possibleMoves;
        }

        public List<MinMaxNode> generateGreyMoves(GameBoard gameState, bool isBlueTurn)
        {
            List<MinMaxNode> minmaxNodeList = new List<MinMaxNode>();
            int baseBoardUtil = getUtility(gameState);

            foreach (var chokepoint in gameState.availableChokepointList)
            {
                //Add a wall at chokepoint
                MinMaxNode addWallMove = new MinMaxNode(gameState);
                BoardSearch ppBFS = new BoardSearch(addWallMove.minmaxGameboard);
                //for NS
                if (chokepoint.Value.Item1.Item1 == chokepoint.Value.Item2.Item1)
                {
                    addWallMove.minmaxGameboard.gameBoard[chokepoint.Value.Item1.Item1, chokepoint.Value.Item1.Item2] = "3N";
                    addWallMove.minmaxGameboard.gameBoard[chokepoint.Value.Item2.Item1, chokepoint.Value.Item2.Item2] = "3S";
                }
                //for WE
                else
                {
                    addWallMove.minmaxGameboard.gameBoard[chokepoint.Value.Item1.Item1, chokepoint.Value.Item1.Item2] = "3W";
                    addWallMove.minmaxGameboard.gameBoard[chokepoint.Value.Item2.Item1, chokepoint.Value.Item2.Item2] = "3E";
                }
                //no-trap rule
                //if adding a wall to this position would trap a piece, then the same aplies to moving a wall this position
                if (isBlueTurn ? !ppBFS.isOpponentTrapped(addWallMove.minmaxGameboard, isBlueTurn) : !ppBFS.isOpponentTrapped(addWallMove.minmaxGameboard, isBlueTurn))
                {
                    continue;
                }

                if (isBlueTurn ? getUtility(addWallMove.minmaxGameboard) > baseBoardUtil : getUtility(addWallMove.minmaxGameboard) < baseBoardUtil)
                {
                    addWallMove.minmaxGameboard.availableChokepointList.Remove(chokepoint.Key);
                    addWallMove.minmaxGameboard.wallPositionList.Add(chokepoint.Key, chokepoint.Value);
                    addWallMove.currentMove.Add(chokepoint.Value);
                    addWallMove.currentMove.Add(((-2, -2), (-2, -2)));
                    minmaxNodeList.Add(addWallMove);
                }
                //end of addWall
                //-----------------------------------------------------------------------------------------------//

                //for each wall to be moved to that chokepoint
                foreach (var boardWall in gameState.wallPositionList)
                {
                    MinMaxNode wallMove = new MinMaxNode(gameState);
                    //for NS
                    if (chokepoint.Value.Item1.Item1 == chokepoint.Value.Item2.Item1)
                    {
                        wallMove.minmaxGameboard.gameBoard[chokepoint.Value.Item1.Item1, chokepoint.Value.Item1.Item2] = "3N";
                        wallMove.minmaxGameboard.gameBoard[chokepoint.Value.Item2.Item1, chokepoint.Value.Item2.Item2] = "3S";
                    }
                    //for WE
                    else
                    {
                        wallMove.minmaxGameboard.gameBoard[chokepoint.Value.Item1.Item1, chokepoint.Value.Item1.Item2] = "3W";
                        wallMove.minmaxGameboard.gameBoard[chokepoint.Value.Item2.Item1, chokepoint.Value.Item2.Item2] = "3E";
                    }

                    wallMove.minmaxGameboard.gameBoard[boardWall.Value.Item1.Item1, boardWall.Value.Item1.Item2] = "0";
                    wallMove.minmaxGameboard.gameBoard[boardWall.Value.Item2.Item1, boardWall.Value.Item2.Item2] = "0";

                    if (isBlueTurn ? getUtility(wallMove.minmaxGameboard) > baseBoardUtil : getUtility(wallMove.minmaxGameboard) < baseBoardUtil)
                    {
                        wallMove.minmaxGameboard.availableChokepointList.Remove(chokepoint.Key);
                        if (wallMove.minmaxGameboard.baseChokepointList.Contains(boardWall))
                        {
                            wallMove.minmaxGameboard.availableChokepointList.Add(boardWall.Key, boardWall.Value);
                        }
                        wallMove.minmaxGameboard.wallPositionList.Remove(boardWall.Key);
                        wallMove.minmaxGameboard.wallPositionList.Add(chokepoint.Key, chokepoint.Value);
                        wallMove.currentMove.Add(chokepoint.Value);
                        wallMove.currentMove.Add(boardWall.Value);
                        minmaxNodeList.Add(wallMove);
                    }
                }
            }
            return minmaxNodeList;
        }

        public List<MinMaxNode> generatePlayerPieceMoves(int numMoves, bool isBlueTurn, GameBoard _gameState)
        {
            List<MinMaxNode> boards = new List<MinMaxNode>();
            BoardSearch playerMovement = new BoardSearch(_gameState);

            int red1PathVal = (35 - (playerMovement.getPathLength(_gameState.redPiece1[0], _gameState.redPiece1[1], "7R").Sum() / 3));
            int red2PathVal = (35 - (playerMovement.getPathLength(_gameState.redPiece2[0], _gameState.redPiece2[1], "7R").Sum() / 3));
            int red3PathVal = (35 - (playerMovement.getPathLength(_gameState.redPiece3[0], _gameState.redPiece3[1], "7R").Sum() / 3));
            int redPathSum = (red1PathVal + red2PathVal + red3PathVal) * 5;

            int blue1PathVal = (35 - (playerMovement.getPathLength(_gameState.bluePiece1[0], _gameState.bluePiece1[1], "7B").Sum() / 3));
            int blue2PathVal = (35 - (playerMovement.getPathLength(_gameState.bluePiece2[0], _gameState.bluePiece2[1], "7B").Sum() / 3));
            int blue3PathVal = (35 - (playerMovement.getPathLength(_gameState.bluePiece3[0], _gameState.bluePiece3[1], "7B").Sum() / 3));
            int bluePathSum = (blue1PathVal + blue2PathVal + blue3PathVal) * 5;

            _gameState.availableMoves = numMoves;
            if (!isBlueTurn)
            {
                int redComparisonUtil = getUtility(_gameState) + 150;
                foreach ((int, int) destination in playerMovement.getSpecificDistBFS(_gameState.redPiece1[0], _gameState.redPiece1[1], _gameState.availableMoves))
                {
                    MinMaxNode mmNode = new MinMaxNode(_gameState);
                    /*mmNode.minmaxGameboard.selectedPiece = "blue2";
                    mmNode.minmaxGameboard.movePieceByClick(destination.Item1, destination.Item2);*/
                    botPieceMovement(destination.Item1, destination.Item2, mmNode.minmaxGameboard, mmNode.minmaxGameboard.redPiece1, isBlueTurn);
                    if (getUtilitySansPaths(mmNode.minmaxGameboard, 
                                            bluePathSum - ((35 - (playerMovement.getPathLength(_gameState.redPiece1[0], _gameState.redPiece1[1], "7R").Sum() / 3)) + red2PathVal + red3PathVal )*5) 
                                                                                                                                                                               < redComparisonUtil)
                    {
                        mmNode.currentMove.Add((destination, (_gameState.redPiece1[0], _gameState.redPiece1[1])));
                        boards.Add(mmNode);
                    }
                }
                foreach ((int, int) destination in playerMovement.getSpecificDistBFS(_gameState.redPiece2[0], _gameState.redPiece2[1], _gameState.availableMoves))
                {
                    MinMaxNode mmNode = new MinMaxNode(_gameState);
                    botPieceMovement(destination.Item1, destination.Item2, mmNode.minmaxGameboard, mmNode.minmaxGameboard.redPiece1, isBlueTurn);
                    if (getUtilitySansPaths(mmNode.minmaxGameboard, 
                                            bluePathSum - (red1PathVal + (35 - (playerMovement.getPathLength(_gameState.redPiece2[0], _gameState.redPiece2[1], "7R").Sum() / 3)) + red3PathVal) * 5) 
                                                                                                                                                                                < redComparisonUtil)
                    {
                        mmNode.currentMove.Add((destination, (_gameState.redPiece2[0], _gameState.redPiece2[1])));
                        boards.Add(mmNode);
                    }
                }
                foreach ((int, int) destination in playerMovement.getSpecificDistBFS(_gameState.redPiece3[0], _gameState.redPiece3[1], _gameState.availableMoves))
                {
                    MinMaxNode mmNode = new MinMaxNode(_gameState);
                    botPieceMovement(destination.Item1, destination.Item2, mmNode.minmaxGameboard, mmNode.minmaxGameboard.redPiece1, isBlueTurn);
                    if (getUtilitySansPaths(mmNode.minmaxGameboard,
                                            bluePathSum - (red1PathVal + red2PathVal + (35 - (playerMovement.getPathLength(_gameState.redPiece3[0], _gameState.redPiece3[1], "7R").Sum() / 3))) * 5)
                                                                                                                                                                                < redComparisonUtil)
                    {
                        mmNode.currentMove.Add((destination, (_gameState.redPiece3[0], _gameState.redPiece3[1])));
                        boards.Add(mmNode);
                    }
                }
            }

            else if (isBlueTurn)
            {
                int blueComparisonUtil = getUtility(_gameState) - 150;
                foreach ((int, int) destination in playerMovement.getSpecificDistBFS(_gameState.bluePiece1[0], _gameState.bluePiece1[1], _gameState.availableMoves))
                {
                    MinMaxNode mmNode = new MinMaxNode(_gameState);
                    botPieceMovement(destination.Item1, destination.Item2, mmNode.minmaxGameboard, mmNode.minmaxGameboard.bluePiece1, isBlueTurn);
                    if (getUtilitySansPaths(mmNode.minmaxGameboard, 
                                           ((35 - (playerMovement.getPathLength(_gameState.bluePiece1[0], _gameState.bluePiece1[1], "7B").Sum() / 3)) + blue2PathVal + blue3PathVal) * 5 - redPathSum) 
                                                                                                                                                                                 > blueComparisonUtil)
                    {
                        mmNode.currentMove.Add((destination, (_gameState.bluePiece1[0], _gameState.bluePiece1[1])));
                        boards.Add(mmNode);
                    }
                }
                foreach ((int, int) destination in playerMovement.getSpecificDistBFS(_gameState.bluePiece2[0], _gameState.bluePiece2[1], _gameState.availableMoves))
                {
                    MinMaxNode mmNode = new MinMaxNode(_gameState);
                    botPieceMovement(destination.Item1, destination.Item2, mmNode.minmaxGameboard, mmNode.minmaxGameboard.bluePiece2, isBlueTurn);
                    if (getUtilitySansPaths(mmNode.minmaxGameboard,
                                           (blue1PathVal + (35 - (playerMovement.getPathLength(_gameState.bluePiece2[0], _gameState.bluePiece2[1], "7B").Sum() / 3)) + blue3PathVal) * 5 - redPathSum)
                                                                                                                                                                                 > blueComparisonUtil)
                    {
                        mmNode.currentMove.Add((destination, (_gameState.bluePiece2[0], _gameState.bluePiece2[1])));
                        boards.Add(mmNode);
                    }
                }
                foreach ((int, int) destination in playerMovement.getSpecificDistBFS(_gameState.bluePiece3[0], _gameState.bluePiece3[1], _gameState.availableMoves))
                {
                    MinMaxNode mmNode = new MinMaxNode(_gameState);
                    botPieceMovement(destination.Item1, destination.Item2, mmNode.minmaxGameboard, mmNode.minmaxGameboard.bluePiece3, isBlueTurn);
                    if (getUtilitySansPaths(mmNode.minmaxGameboard,
                                           (blue1PathVal + blue2PathVal + (35 - (playerMovement.getPathLength(_gameState.bluePiece3[0], _gameState.bluePiece3[1], "7B").Sum() / 3))) * 5 - redPathSum)
                                                                                                                                                                                 > blueComparisonUtil)
                    {
                        mmNode.currentMove.Add((destination, (_gameState.bluePiece3[0], _gameState.bluePiece3[1])));
                        boards.Add(mmNode);
                    }
                }

            }
            _gameState.availableMoves = 0;
            return boards;
        }

        //returns (chokepoint, which-wall) \/
        public List<((int, int), (int, int))> getGreyDecision(GameBoard _gameState, int depth)
        {
            //list contatining the chokepoint and what wall to move there
            List<((int, int), (int, int))> bestMove = new List<((int, int), (int, int))> ();
            int highestValue = -1000;
            List<MinMaxNode> moveList = generateGreyMoves(_gameState, true);

            //tester to track number of moves left to check
            int cep = moveList.Count;
            foreach(MinMaxNode openingMove in moveList)
            {
                //since wall move will always be short term
                int moveValue = minMaxDecision(openingMove, false, 0);
                if (moveValue >= highestValue)
                {
                    highestValue = moveValue;
                    bestMove = openingMove.currentMove;
                }
                cep--;
            }

            return bestMove;
        }

        //returns (destination, which-piece)
        public ((int, int), (int, int)) getPlayerPieceDecision(GameBoard _gameState, int depth)
        {
            int highestValue = -1000;
            //PlayerPieceBFS movementOptions = new PlayerPieceBFS(_gameState);
            //list contatining the destination and what piece to move there
            ((int, int), (int, int)) bestMove = ((0,0),(0,0));
            List<MinMaxNode> moveList = generatePlayerPieceMoves(_gameState.availableMoves, true, _gameState);

            foreach (MinMaxNode move in moveList)
            {
                int moveValue = minMaxDecision(move, false, depth);
                if (moveValue >= highestValue)
                {
                    highestValue = moveValue;
                    bestMove = move.currentMove.FirstOrDefault();
                }
            }

            return bestMove;
        }

        //moves the piece under the assumption that the move is valid (only works for player pieces ATM)
        public void botPieceMovement(int xDest, int yDest, GameBoard gamestate, int[] curPiece, bool isBlueTurn = true)
        {
            if (curPiece != gamestate.minotuarPos)
            {
                //bot movement for Player pieces
                //if current location not a base
                if (gamestate.gameBoard[curPiece[0], curPiece[1]] == (isBlueTurn ? "4B" : "4R"))
                {
                    gamestate.gameBoard[curPiece[0], curPiece[1]] = "0";
                }
                //if future location not a base
                if (gamestate.gameBoard[xDest, yDest] == "0")
                {
                    gamestate.gameBoard[xDest, yDest] = (isBlueTurn ? "4B" : "4R");
                    gamestate.checkPieceWallCollision(xDest, xDest, (curPiece[0], curPiece[1]));
                }
                curPiece[0] = xDest;
                curPiece[1] = yDest;
            }
            //movement for minotaur
            else
            {
                gamestate.gameBoard[curPiece[0], curPiece[1]] = "0";
                if (!gamestate.minoCollisionCheck(xDest, yDest))
                {
                    gamestate.gameBoard[xDest, yDest] = "6";
                    gamestate.checkPieceWallCollision(xDest, xDest, (curPiece[0], curPiece[1]));
                    curPiece[0] = xDest;
                    curPiece[1] = yDest;
                }
            }
        }
    }
}
