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
    public class TurnBasedAI
    {
        public GameBoard currentGameBoard;

        public TurnBasedAI() { }
        public TurnBasedAI(GameBoard gB)
        {
            currentGameBoard = gB;
        }
        ///variables used for the tracking of how many times different functions run.
        int minMaxDecCount = 0;
        int getUtilCount = 0;
        int getUtilNoPathsCount = 0;
        int getDistToCenterCount = 0;
        int getAllPathLengthsCount = 0;
        int getAllPossibleCount = 0;
        int genGreyMovesCount = 0;
        int genPlayerMovesCount = 0;
        int botMoveCount = 0;


        public void Update()
        {
            currentGameBoard.boardState = getUtility(currentGameBoard);
            if (!currentGameBoard.isRedTurn && !currentGameBoard.diceRolled)
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

            //what to do based on roll  \\//

            /////////////////////////////////////////////logic for rolling a "grey"/////////////////////////////////////////////
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

            /////////////////////////////////////////////logic for rolling a "black"////////////////////////////////////////////
            else if (currentGameBoard.selectedPiece == "mino")
            {
                if (!currentGameBoard.isBlackActive)
                {
                    currentGameBoard.minotuarPos[0] = 12;
                    currentGameBoard.minotuarPos[1] = 12;
                    currentGameBoard.gameBoard[currentGameBoard.minotuarPos[0], currentGameBoard.minotuarPos[1]] = "6";
                    currentGameBoard.isBlackActive = true;
                    currentGameBoard.availableMoves = 7;
                }
                else if (currentGameBoard.availableMoves > 0)
                {
                    BoardSearch movementPath = new BoardSearch(currentGameBoard);

                    List<(int, int)> minoPath = movementPath.BFS(currentGameBoard.minotuarPos[0], currentGameBoard.minotuarPos[1], "4R");

                    //assuming piece selected
                    if (minoPath.Count <= 8)
                    {
                        botPieceMovement(minoPath.LastOrDefault().Item1, minoPath.LastOrDefault().Item2, currentGameBoard, 6);
                    }
                    else
                    {
                        botPieceMovement(minoPath[currentGameBoard.availableMoves-1].Item1, 
                                         minoPath[currentGameBoard.availableMoves - 1].Item2, 
                                         currentGameBoard, 6);
                    }
                    
                    currentGameBoard.availableMoves = 0;
                    currentGameBoard.endTurn();
                }
                else
                {
                    currentGameBoard.endTurn();
                }
            }

            /////////////////////////////////////////////logic for moving game player piece ///////////////////////////////////
            else
            {
                //might not need extra if-statment
                if (currentGameBoard.selectedPiece == "")
                {
                    ((int, int),(int, int)) playerPieceMove = getPlayerPieceDecision(currentGameBoard, 1);
                    //error catcher (will be removed)/////////////////////////////////////////////////////////////////////////////////
                    if (playerPieceMove == ((0,0),(0,0)))
                    {
                        Console.WriteLine("move fail");
                    }
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (playerPieceMove.Item2 == (currentGameBoard.bluePiece1[0], currentGameBoard.bluePiece1[1]))
                    {
                        botPieceMovement(playerPieceMove.Item1.Item1, playerPieceMove.Item1.Item2, currentGameBoard, 1, true);
                    }
                    else if (playerPieceMove.Item2 == (currentGameBoard.bluePiece2[0], currentGameBoard.bluePiece2[1]))
                    {
                        botPieceMovement(playerPieceMove.Item1.Item1, playerPieceMove.Item1.Item2, currentGameBoard, 2, true);
                    }
                    else if (playerPieceMove.Item2 == (currentGameBoard.bluePiece3[0], currentGameBoard.bluePiece3[1]))
                    {
                        botPieceMovement(playerPieceMove.Item1.Item1, playerPieceMove.Item1.Item2, currentGameBoard, 3, true);
                    }
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
            minMaxDecCount++;
            if (depth == 0 || (minmaxNode.minmaxGameboard.redVictory || minmaxNode.minmaxGameboard.blueVictory))
            {
                return getUtility(minmaxNode.minmaxGameboard);
            }



            //for maxing (currentPlayerBlue == true)
            if (currentPLayerBlue)
            {
                int bestValue = -10000;//<--value to beat
                getAllPossibleMoves(minmaxNode, true, minmaxNode.currentMove.Count != 2);
                foreach (MinMaxNode child in minmaxNode.children)
                {
                    int childValue = minMaxDecision(child, !currentPLayerBlue, (depth - 1));
                    bestValue = Math.Max(bestValue, childValue);
                    
                }
                return bestValue;
            }
            //for minimizing (currentPlayerBlue == false)
            else
            {
                int worstValue = 10000;//<--value to beat
                getAllPossibleMoves(minmaxNode, false, minmaxNode.currentMove.Count != 2);
                foreach (MinMaxNode child in minmaxNode.children)
                {
                    int childValue = minMaxDecision(child, !currentPLayerBlue, (depth - 1));
                    worstValue = Math.Min(worstValue, childValue);
                }
                return worstValue;
            }
        }

        public int getUtility(GameBoard gameState)
        {
            getUtilCount++;
            BoardSearch pieceProximity = new BoardSearch(gameState);
            //proximity of all pieces to center (30 moves?)(minimum number of moves)
            (int, List<List<(int, int)>>) redDistanceToCenter = getDistanceToCenterValue(false, gameState, pieceProximity);

            (int, List<List<(int, int)>>) blueDistanceToCenter = getDistanceToCenterValue(true, gameState, pieceProximity);

            //player pieces within 8 of minotaur
            //[0] = number of red pieces in minotaur range
            //[1] = number of blue in minotaur range
            int[] inMinotaurRange = { 0, 0 };
            if (gameState.isBlackActive)
            {
                inMinotaurRange = pieceProximity.getMinotaurZoneBFS();
            }

            //length of possible paths
            int pathValues = getPathLengthValue(gameState, pieceProximity, blueDistanceToCenter.Item2, redDistanceToCenter.Item2);

            int utility = (blueDistanceToCenter.Item1 - redDistanceToCenter.Item1) * 15 + ((inMinotaurRange[1] - inMinotaurRange[0])*-125) + (pathValues * 5);
            return utility;
        }

        //Utility method that assuumes the path value are already caluculated
        public int getUtilitySansPaths(GameBoard gameState, int allPathValues)
        {
            getUtilNoPathsCount++;
            BoardSearch pieceProximity = new BoardSearch(gameState);
            //proximity of all pieces to center (30 moves?)(minimum number of moves)
            (int, List<List<(int, int)>>) redDistanceToCenter = getDistanceToCenterValue(false, gameState, pieceProximity);

            (int, List<List<(int, int)>>) blueDistanceToCenter = getDistanceToCenterValue(true, gameState, pieceProximity);

            //player pieces within 8 of minotaur
            //[0] = number of red pieces in minotaur range
            //[1] = number of blue in minotaur range
            int[] inMinotaurRange = { 0, 0 };
            if (gameState.isBlackActive)
            {
                inMinotaurRange = pieceProximity.getMinotaurZoneBFS();
            }

            int utility = ((blueDistanceToCenter.Item1 - redDistanceToCenter.Item1) * 15) + ((inMinotaurRange[1] - inMinotaurRange[0]) * -100) + allPathValues * 5;
            return utility;
        }

        public (int, List<List<(int, int)>>) getDistanceToCenterValue(bool isBlueTurn, GameBoard gamestate, BoardSearch ppBFS)
        {
            getDistToCenterCount++;
            //List <(int, Dictionary<(int, int), (int, int)>)> PathsAndDistances = ppBFS.getFastestPlayerPiecePathsLengths(gamestate, isBlueTurn);
            //List<Dictionary<(int, int), (int, int)>> piecePaths = new List<Dictionary<(int, int), (int, int)>>();
            List<List<(int, int)>> piecePaths = new List<List<(int, int)>>();
            List<int[]> pieceList;
            string objective;
            if (!isBlueTurn)
            {
                pieceList = new List<int[]>() { gamestate.redPiece1, gamestate.redPiece2, gamestate.redPiece3 };
                objective = "7R";
            }
            else
            {
                pieceList = new List<int[]>() { gamestate.bluePiece1, gamestate.bluePiece2, gamestate.bluePiece3 };
                objective = "7B";
            }
            int sum = 0;
            foreach (int[] piece in pieceList)
            {
                List<(int, int)> piecePath = ppBFS.BFS(piece[0], piece[1], objective);
                if (gamestate.gameBoard[piece[0], piece[1]] == objective)
                {
                    sum += 70;
                }
                else if(piecePath.Count > 0)
                {
                    sum += 35 - piecePath.Count;
                }else if(piecePath.Count == 0 && gamestate.gameBoard[piece[0], piece[1]] != objective)
                {
                    sum -= 250;
                }
                piecePaths.Add(piecePath);
            }
            return (sum, piecePaths);
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

        public int getPathLengthValue(GameBoard gamestate, BoardSearch pathOptions, List<List<(int, int)>> BlueFastPaths, List<List<(int, int)>> RedFastPaths)
        {
            getAllPathLengthsCount++;
            ////Can just pass the first route (fastest from the dist to center method!!!!!!\\\///
            ///!!!!!!
            int blueSum = ((35 - (pathOptions.getPathLengthsMinusFirst(gamestate.bluePiece1[0], gamestate.bluePiece1[1], "7B", BlueFastPaths[0]).Sum() / 3)) +
                           (35 - (pathOptions.getPathLengthsMinusFirst(gamestate.bluePiece2[0], gamestate.bluePiece2[1], "7B", BlueFastPaths[1]).Sum() / 3)) +
                           (35 - (pathOptions.getPathLengthsMinusFirst(gamestate.bluePiece3[0], gamestate.bluePiece3[1], "7B", BlueFastPaths[2]).Sum() / 3)));

            int redSum = ((35 - (pathOptions.getPathLengthsMinusFirst(gamestate.redPiece1[0], gamestate.redPiece1[1], "7R", RedFastPaths[0]).Sum() / 3)) +
                          (35 - (pathOptions.getPathLengthsMinusFirst(gamestate.redPiece2[0], gamestate.redPiece2[1], "7R", RedFastPaths[1]).Sum() / 3)) +
                          (35 - (pathOptions.getPathLengthsMinusFirst(gamestate.redPiece3[0], gamestate.redPiece3[1], "7R", RedFastPaths[2]).Sum() / 3)));
            return blueSum - redSum;
        }

        public void getAllPossibleMoves(MinMaxNode minmaxNode, bool isBlueTurn, bool includeGrey)
        {
            getAllPossibleCount++;
            //List<MinMaxNode> possibleMoves = new List<MinMaxNode>();

            /*if (isBlueTurn)
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
            }*/
            minmaxNode.children.AddRange(generatePlayerPieceMoves(6, isBlueTurn, minmaxNode));

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
            _path = possibleMovesPath.BFS(minoMinmaxNode.minmaxGameboard.minotuarPos[0], minoMinmaxNode.minmaxGameboard.minotuarPos[1], isBlueTurn ? "4R" : "4B");

            if (_path.Count <= 8)
            {
                botPieceMovement(_path.LastOrDefault().Item1, _path.LastOrDefault().Item2, minoMinmaxNode.minmaxGameboard, 6);
            }
            else
            {
                botPieceMovement(_path[7].Item1, _path[7].Item2, minoMinmaxNode.minmaxGameboard, 6);
            }
            minmaxNode.children.Add(minoMinmaxNode);

            //return possibleMoves;
        }

        public List<MinMaxNode> generateGreyMoves(GameBoard gameState, bool isBlueTurn)
        {
            genGreyMovesCount++;
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
                if (ppBFS.isOpponentTrapped(addWallMove.minmaxGameboard, isBlueTurn))
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
                        if (wallMove.minmaxGameboard.baseChokepointList.ContainsKey(boardWall.Key))
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

        public List<MinMaxNode> generatePlayerPieceMoves(int numMoves, bool isBlueTurn, MinMaxNode parentNode)
        {
            genPlayerMovesCount++;
            GameBoard _gameState = parentNode.minmaxGameboard;
            List<MinMaxNode> boards = new List<MinMaxNode>();
            BoardSearch playerMovement = new BoardSearch(_gameState);
            if (parentNode.bluePathVals.Sum() == 0 && parentNode.redPathVals.Sum() == 0)
            {
                parentNode.redPathVals[0] = (35 - (playerMovement.getPathLengths(_gameState.redPiece1[0], _gameState.redPiece1[1], "7R").Sum() / 3));
                parentNode.redPathVals[1] = (35 - (playerMovement.getPathLengths(_gameState.redPiece2[0], _gameState.redPiece2[1], "7R").Sum() / 3));
                parentNode.redPathVals[2] = (35 - (playerMovement.getPathLengths(_gameState.redPiece3[0], _gameState.redPiece3[1], "7R").Sum() / 3));
                parentNode.bluePathVals[0] = (35 - (playerMovement.getPathLengths(_gameState.bluePiece1[0], _gameState.bluePiece1[1], "7B").Sum() / 3));
                parentNode.bluePathVals[1] = (35 - (playerMovement.getPathLengths(_gameState.bluePiece2[0], _gameState.bluePiece2[1], "7B").Sum() / 3));
                parentNode.bluePathVals[2] = (35 - (playerMovement.getPathLengths(_gameState.bluePiece3[0], _gameState.bluePiece3[1], "7B").Sum() / 3));
            }
            int redPathSum = (parentNode.redPathVals.Sum()) * 5;
            int bluePathSum = (parentNode.bluePathVals.Sum()) * 5;

            _gameState.availableMoves = numMoves;
            int utilComparison = getUtilitySansPaths(_gameState, bluePathSum - redPathSum) + (isBlueTurn ? -150 : 150);
            if (!isBlueTurn)
            {
                List<int[]> pieceList = new List<int[]>()
                {
                    _gameState.redPiece1, _gameState.redPiece2, _gameState.redPiece3
                };

                for (int i = 0; i < 3; i++)
                {
                    foreach ((int, int) destination in playerMovement.getSpecificDistBFS(pieceList[i][0], pieceList[i][1], numMoves))
                    {
                        MinMaxNode mmNode = new MinMaxNode(_gameState, parentNode.bluePathVals, parentNode.redPathVals);
                        botPieceMovement(destination.Item1, destination.Item2, mmNode.minmaxGameboard, i+1, isBlueTurn);
                        mmNode.redPathVals[i] = (35 - (playerMovement.getPathLengths(pieceList[i][0], pieceList[i][1], "7R").Sum() / 3));
                        if (getUtilitySansPaths(mmNode.minmaxGameboard, (bluePathSum - mmNode.redPathVals.Sum() * 5)) < utilComparison)
                        {
                            mmNode.currentMove.Add((destination, (pieceList[i][0], pieceList[i][1])));
                            boards.Add(mmNode);
                        }
                    }
                }
                /*foreach ((int, int) destination in playerMovement.getSpecificDistBFS(_gameState.redPiece1[0], _gameState.redPiece1[1], numMoves))
                {
                    MinMaxNode mmNode = new MinMaxNode(_gameState, parentNode.bluePathVals, parentNode.redPathVals);
                    botPieceMovement(destination.Item1, destination.Item2, mmNode.minmaxGameboard, mmNode.minmaxGameboard.redPiece1, isBlueTurn);
                    mmNode.redPathVals[0] = (35 - (playerMovement.getPathLengths(_gameState.redPiece1[0], _gameState.redPiece1[1], "7R").Sum() / 3));
                    if (getUtilitySansPaths(mmNode.minmaxGameboard, (bluePathSum - mmNode.redPathVals.Sum()*5)) < comparisonUtil)
                    {
                        mmNode.currentMove.Add((destination, (_gameState.redPiece1[0], _gameState.redPiece1[1])));
                        boards.Add(mmNode);
                    }
                }
                foreach ((int, int) destination in playerMovement.getSpecificDistBFS(_gameState.redPiece2[0], _gameState.redPiece2[1], numMoves))
                {
                    MinMaxNode mmNode = new MinMaxNode(_gameState, parentNode.bluePathVals, parentNode.redPathVals);
                    botPieceMovement(destination.Item1, destination.Item2, mmNode.minmaxGameboard, mmNode.minmaxGameboard.redPiece2, isBlueTurn);
                    mmNode.redPathVals[1] = (35 - (playerMovement.getPathLengths(_gameState.redPiece2[0], _gameState.redPiece2[1], "7R").Sum() / 3));
                    if (getUtilitySansPaths(mmNode.minmaxGameboard, (bluePathSum - mmNode.redPathVals.Sum() * 5)) < comparisonUtil)
                    {
                        mmNode.currentMove.Add((destination, (_gameState.redPiece2[0], _gameState.redPiece2[1])));
                        boards.Add(mmNode);
                    }
                }
                foreach ((int, int) destination in playerMovement.getSpecificDistBFS(_gameState.redPiece3[0], _gameState.redPiece3[1], numMoves))
                {
                    MinMaxNode mmNode = new MinMaxNode(_gameState, parentNode.bluePathVals, parentNode.redPathVals);
                    botPieceMovement(destination.Item1, destination.Item2, mmNode.minmaxGameboard, mmNode.minmaxGameboard.redPiece3, isBlueTurn);
                    mmNode.redPathVals[2] = (35 - (playerMovement.getPathLengths(_gameState.redPiece3[0], _gameState.redPiece3[1], "7R").Sum() / 3));
                    if (getUtilitySansPaths(mmNode.minmaxGameboard, (bluePathSum - mmNode.redPathVals.Sum() * 5)) < comparisonUtil)
                    {
                        mmNode.currentMove.Add((destination, (_gameState.redPiece3[0], _gameState.redPiece3[1])));
                        boards.Add(mmNode);
                    }
                }*/
            }

            else if (isBlueTurn)
            {
                List<int[]> pieceList = new List<int[]>()
                {
                    _gameState.bluePiece1, _gameState.bluePiece2, _gameState.bluePiece3
                };

                for (int i = 0; i < 3; i++)
                {
                    foreach ((int, int) destination in playerMovement.getSpecificDistBFS(pieceList[i][0], pieceList[i][1], numMoves))
                    {
                        MinMaxNode mmNode = new MinMaxNode(_gameState, parentNode.bluePathVals, parentNode.redPathVals);
                        botPieceMovement(destination.Item1, destination.Item2, mmNode.minmaxGameboard, i+1, isBlueTurn);
                        mmNode.bluePathVals[i] = (35 - (playerMovement.getPathLengths(pieceList[i][0], pieceList[i][1], "7R").Sum() / 3));
                        if (getUtilitySansPaths(mmNode.minmaxGameboard, (mmNode.bluePathVals.Sum() * 5) - redPathSum) > utilComparison)
                        { 
                            mmNode.currentMove.Add((destination, (pieceList[i][0], pieceList[i][1])));
                            boards.Add(mmNode);
                        }
                    }
                }

                /*foreach ((int, int) destination in playerMovement.getSpecificDistBFS(_gameState.bluePiece1[0], _gameState.bluePiece1[1], numMoves))
                {
                    MinMaxNode mmNode = new MinMaxNode(_gameState, parentNode.bluePathVals, parentNode.redPathVals);
                    botPieceMovement(destination.Item1, destination.Item2, mmNode.minmaxGameboard, mmNode.minmaxGameboard.bluePiece1, isBlueTurn);
                    mmNode.bluePathVals[0] = (35 - (playerMovement.getPathLengths(_gameState.bluePiece1[0], _gameState.bluePiece1[1], "7R").Sum() / 3));
                    if (getUtilitySansPaths(mmNode.minmaxGameboard, (mmNode.bluePathVals.Sum() * 5) - redPathSum) > comparisonUtil)
                    {
                        mmNode.currentMove.Add((destination, (_gameState.bluePiece1[0], _gameState.bluePiece1[1])));
                        boards.Add(mmNode);
                    }
                }
                foreach ((int, int) destination in playerMovement.getSpecificDistBFS(_gameState.bluePiece2[0], _gameState.bluePiece2[1], numMoves))
                {
                    MinMaxNode mmNode = new MinMaxNode(_gameState, parentNode.bluePathVals, parentNode.redPathVals);
                    botPieceMovement(destination.Item1, destination.Item2, mmNode.minmaxGameboard, mmNode.minmaxGameboard.bluePiece2, isBlueTurn);
                    mmNode.bluePathVals[1] = (35 - (playerMovement.getPathLengths(_gameState.bluePiece2[0], _gameState.bluePiece2[1], "7R").Sum() / 3));
                    if (getUtilitySansPaths(mmNode.minmaxGameboard, (mmNode.bluePathVals.Sum() * 5) - redPathSum) > comparisonUtil)
                    {
                        mmNode.currentMove.Add((destination, (_gameState.bluePiece2[0], _gameState.bluePiece2[1])));
                        boards.Add(mmNode);
                    }
                }
                foreach ((int, int) destination in playerMovement.getSpecificDistBFS(_gameState.bluePiece3[0], _gameState.bluePiece3[1], numMoves))
                {
                    MinMaxNode mmNode = new MinMaxNode(_gameState, parentNode.bluePathVals, parentNode.redPathVals);
                    botPieceMovement(destination.Item1, destination.Item2, mmNode.minmaxGameboard, mmNode.minmaxGameboard.bluePiece3, isBlueTurn);
                    mmNode.bluePathVals[2] = (35 - (playerMovement.getPathLengths(_gameState.bluePiece3[0], _gameState.bluePiece3[1], "7R").Sum() / 3));
                    if (getUtilitySansPaths(mmNode.minmaxGameboard, (mmNode.bluePathVals.Sum() * 5) - redPathSum) > comparisonUtil)
                    {
                        mmNode.currentMove.Add((destination, (_gameState.bluePiece3[0], _gameState.bluePiece3[1])));
                        boards.Add(mmNode);
                    }
                }*/
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

            foreach(MinMaxNode openingMove in moveList)
            {
                //since wall move will always be short term
                int moveValue = minMaxDecision(openingMove, false, 0);
                if (moveValue >= highestValue)
                {
                    highestValue = moveValue;
                    bestMove = openingMove.currentMove;
                }
            }

            return bestMove;
        }

        //returns (destination, which-piece)
        public ((int, int), (int, int)) getPlayerPieceDecision(GameBoard _gameState, int depth)
        {
            int highestValue = -1000;
            int avMovs = _gameState.availableMoves;
            //PlayerPieceBFS movementOptions = new PlayerPieceBFS(_gameState);
            //list contatining the destination and what piece to move there
            ((int, int), (int, int)) bestMove = ((0,0),(0,0));
            List<MinMaxNode> moveList = generatePlayerPieceMoves(_gameState.availableMoves, true, new MinMaxNode(_gameState));
            _gameState.availableMoves = 0;
            if (moveList.Count == 0)
            {
                Console.WriteLine("moveList Failure");
            }
            
            foreach (MinMaxNode move in moveList)
            {
                int moveValue = minMaxDecision(move, false, depth);
                if (moveValue > highestValue)
                {
                    highestValue = moveValue;
                    bestMove = move.currentMove.FirstOrDefault();
                }
            }
            if(bestMove == ((0, 0), (0, 0)))
            {
                Console.WriteLine("minMaxDec Failure");
            }

            return bestMove;
        }

        //moves the piece under the assumption that the move is valid (only works for player pieces ATM)
        public void botPieceMovement(int xDest, int yDest, GameBoard gamestate, int pieceNum, bool isBlueTurn = true)
        {
            botMoveCount++;
            int[] curPiece = new int[2];
            switch (pieceNum)
            {
                case 1:
                    curPiece = isBlueTurn ? gamestate.bluePiece1 : gamestate.redPiece1;
                    break;

                case 2:
                    curPiece = isBlueTurn ? gamestate.bluePiece2 : gamestate.redPiece2;
                    break;

                case 3:
                    curPiece = isBlueTurn ? gamestate.bluePiece3 : gamestate.redPiece3;
                    break;

                case 6:
                    curPiece = gamestate.minotuarPos;
                    break;
            }

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
                    gamestate.checkPieceWallCollision(xDest, yDest, (curPiece[0], curPiece[1]));
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
                    gamestate.checkPieceWallCollision(xDest, yDest, (curPiece[0], curPiece[1]));
                    curPiece[0] = xDest;
                    curPiece[1] = yDest;
                }
            }
        }
    }
}
