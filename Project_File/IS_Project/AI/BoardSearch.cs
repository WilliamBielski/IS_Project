﻿using IS_Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace IS_Project.AI
{
    public class BoardSearch : Constants
    {
        const int rowBound = 30;
        const int colBound = 30;


        //directional vectors
        public int[] dirVectRow = { -1, 0, 1, 0 };
        public int[] dirVectCol = { 0, 1, 0, -1 };

        public GameBoard _gameBoard;

        public BoardSearch(GameBoard gB)
        {
            _gameBoard = gB;
        }

        // Function to check if a cell
        // is be visited or not
        public bool isValid(int row, int col, Dictionary<(int, int), int> visited)
        {
            // If cell lies out of bounds
            if (row < 0 || col < 0 || row >= rowBound || col >= colBound)
            {
                return false;
            }

            // If cell is already visited
            //(int, int) pos = ;
            if (visited.ContainsKey((row, col)) || _gameBoard.impassableList.ContainsKey(_gameBoard.gameBoard[row, col]))
            {
                return false;
            }

            // Otherwise
            return true;
        }

        // Function to perform the BFS traversal, returns linked list of directions for shorted path.
        public List<(int, int)> BFS(int row, int col, string objective)
        {
            // Stores indices of the matrix cells
            Queue<Node> queue = new Queue<Node>();
            Dictionary<(int, int), int> visited = new Dictionary<(int, int), int>();

            //linked thing for later
            List<(int, int)> pathList = new List<(int, int)>();
            Node currentNode;

            // Mark the starting cell as visited
            // and push it into the queue
            Node n = new Node((row, col));
            queue.Enqueue(n);
            visited.Add(n.data, n.depth);



            // Iterate while the queue
            // is not empty
            while (queue.Count != 0)
            {
                (int, int) cell = queue.FirstOrDefault().data;
                int x = cell.Item1;
                int y = cell.Item2;


                currentNode = queue.Dequeue();

                //for piece movement i am just looking for, assuming I have _gameboard
                if (_gameBoard.gameBoard[x, y] == objective)
                {
                    while (currentNode.next != null)
                    {
                        pathList.Insert(0, currentNode.data);
                        currentNode = currentNode.next;
                    }

                    return pathList;
                }



                // Goes to the adjacent cells
                for (int i = 0; i < 4; i++)
                {

                    int adjX = x + dirVectRow[i];
                    int adjY = y + dirVectCol[i];

                    if (isValid(adjX, adjY, visited))
                    {
                        if (objective == "5R" && _gameBoard.gameBoard[adjX, adjY] == "5R")
                        {
                            while (currentNode.next != null)
                            {
                                pathList.Insert(0, currentNode.data);
                                currentNode = currentNode.next;
                            }

                            return pathList;

                        }
                        else if (objective == "5B" && _gameBoard.gameBoard[adjX, adjY] == "5B")
                        {
                            while (currentNode.next != null)
                            {
                                pathList.Insert(0, currentNode.data);
                                currentNode = currentNode.next;
                            }

                            return pathList;
                        }
                        else if (_gameBoard.selectedPiece == "mino" &&  _gameBoard.playerBases.ContainsKey(_gameBoard.gameBoard[adjX, adjY]))
                        {
                            continue;
                        }
                        
                        Node adjNode = new Node((adjX, adjY));
                        adjNode.next = currentNode;
                        adjNode.depth = currentNode.depth + 1;

                        queue.Enqueue(adjNode);

                        visited.Add((adjX, adjY), currentNode.depth + 1);
                        

                    }
                }
            }
            return pathList;
        }

        public List<int> getPathLengths(int row, int col, string objective)
        {
            List<int> pathLengthList = new List<int>();
            Dictionary<(int, int), int> traveledList = new Dictionary<(int, int), int>();

            //includes destination at this point
            List<(int, int)> fastestPath = BFS(row, col, objective);
            if (fastestPath.Count != 0)
            {
                pathLengthList.Add(fastestPath.Count);
            }
            else
            {
                pathLengthList.Add(55);
            }

            foreach((int, int) coord in fastestPath)
            {
                if (!traveledList.ContainsKey(coord))
                {
                    if (nwChokepointList.ContainsKey(coord))
                    {
                        traveledList.Add(coord, 0);
                        traveledList.Add(nwChokepointList[coord].Item2, 0);
                    }
                    else if (seChokepointList.ContainsKey(coord))
                    {
                        traveledList.Add(coord, 0);
                        traveledList.Add(seChokepointList[coord].Item1, 0);
                    }
                }

            }

            List<(int, int)> secondFastestPath = findDifferentPlayerRoute(row, col, objective, traveledList);
            if (secondFastestPath.Count != 0)
            {
                pathLengthList.Add(secondFastestPath.Count);
            }
            else
            {
                pathLengthList.Add(60);
            }

            foreach (var coord in secondFastestPath)
            {
                if (!traveledList.ContainsKey(coord))
                {
                    if (nwChokepointList.ContainsKey(coord))
                    {
                        traveledList.Add(coord, 0);
                        traveledList.Add(nwChokepointList[coord].Item2, 0);
                    }
                    else if (seChokepointList.ContainsKey(coord))
                    {
                        traveledList.Add(coord, 0);
                        traveledList.Add(seChokepointList[coord].Item1, 0);
                    }
                }
                
            }

            List<(int, int)> thirdFastestPath = findDifferentPlayerRoute(row, col, objective, traveledList);
            if (thirdFastestPath.Count != 0)
            {
                pathLengthList.Add(thirdFastestPath.Count);
            }
            else
            {
                pathLengthList.Add(50);
            }


            //tester
            /*foreach ((int, int) item in fastestPath.dataList)
            {
                _gameBoard.gameBoard[item.Item1, item.Item2] = objective;
            }
            foreach ((int, int) item in secondFastestPath.dataList)
            {
                _gameBoard.gameBoard[item.Item1, item.Item2] = objective;
            }
            foreach ((int, int) item in thirdFastestPath.dataList)
            {
                _gameBoard.gameBoard[item.Item1, item.Item2] = objective;
            }*/

            return pathLengthList;
        }
        public List<int> getPathLengthsMinusFirst(int row, int col, string objective, List<(int, int)> fastestRoute)
        {
            List<int> pathLengthList = new List<int>();
            Dictionary<(int, int), int> traveledList = new Dictionary<(int, int), int>();

            //includes destination at this point
            List<(int, int)> fastestPath = fastestRoute;
            if (fastestPath.Count != 0)
            {
                pathLengthList.Add(fastestPath.Count);
            }
            else
            {
                pathLengthList.Add(55);
            }

            foreach (var cord in fastestPath)
            {
                if (!traveledList.ContainsKey(cord))
                {
                    traveledList.Add(cord, 0);
                }
            }

            List<(int, int)> secondFastestPath = findDifferentPlayerRoute(row, col, objective, traveledList);
            if (secondFastestPath.Count != 0)
            {
                pathLengthList.Add(secondFastestPath.Count);
            }
            else
            {
                pathLengthList.Add(60);
            }

            foreach (var cord in secondFastestPath)
            {
                if (!traveledList.ContainsKey(cord))
                {
                    traveledList.Add(cord, 0);
                }
            }

            List<(int, int)> thirdFastestPath = findDifferentPlayerRoute(row, col, objective, traveledList);
            if (thirdFastestPath.Count != 0)
            {
                pathLengthList.Add(thirdFastestPath.Count);
            }
            else
            {
                pathLengthList.Add(50);
            }


            //tester
            /*foreach ((int, int) item in fastestPath.dataList)
            {
                _gameBoard.gameBoard[item.Item1, item.Item2] = objective;
            }
            foreach ((int, int) item in secondFastestPath.dataList)
            {
                _gameBoard.gameBoard[item.Item1, item.Item2] = objective;
            }
            foreach ((int, int) item in thirdFastestPath.dataList)
            {
                _gameBoard.gameBoard[item.Item1, item.Item2] = objective;
            }*/

            return pathLengthList;
        }
        //if given a list of traversed locations can find a route that uses none of the same chokepoints
        public List<(int, int)> findDifferentPlayerRoute(int row, int col, string objective, Dictionary<(int, int), int> traversedChokepoints)
        {
            // Stores indices of the matrix cells
            Queue<Node> queue = new Queue<Node>();
            Dictionary<(int, int), int> visited = new Dictionary<(int, int), int>();
            //Dictionary<(int, int), int> offLimitCoords = new Dictionary<(int, int), int>();
            List<(int, int)> pathList = new List<(int, int)>();
            Node currentNode;

            //efficiency? |
            //            |
            //            |
            //            \/
            /*foreach (var coords in dataList)
            {
                if (nwChokepointList.ContainsKey(coords.Key) && !offLimitCoords.ContainsKey(coords.Key))
                {
                    offLimitCoords.Add(coords.Key, 1);
                    offLimitCoords.Add(nwChokepointList[coords.Key].Item2, 1);
                }
                else if (seChokepointList.ContainsKey(coords.Key) && !offLimitCoords.ContainsKey(coords.Key))
                {
                    offLimitCoords.Add(coords.Key, 1);
                    offLimitCoords.Add(seChokepointList[coords.Key].Item1, 1);
                }
            }*/

            // Mark the starting cell as visited
            // and push it into the queue
            Node n = new Node((row, col));
            queue.Enqueue(n);
            visited.Add(n.data, n.depth);

            currentNode = queue.Dequeue();

            for (int k = 0; k < 4; k++)
            {

                int adjX = currentNode.data.Item1 + dirVectRow[k];
                int adjY = currentNode.data.Item2 + dirVectCol[k];
                (int, int) adjTuple = (adjX, adjY);

                if (isValid(adjX, adjY, visited))
                {
                    Node adjNode = new Node(adjTuple);
                    adjNode.next = currentNode;
                    adjNode.depth = currentNode.depth + 1;

                    queue.Enqueue(adjNode);

                    visited.Add(adjTuple, currentNode.depth + 1);
                }
            }


            // Iterate while the queue
            // is not empty
            while (queue.Count != 0)
            {
                (int, int) cell = queue.FirstOrDefault().data;
                int x = cell.Item1;
                int y = cell.Item2;


                currentNode = queue.Dequeue();

                //for piece movement i am just looking for, assuming I have _gameboard
                if (_gameBoard.gameBoard[x, y] == objective)
                {
                    while (currentNode.next != null)
                    {
                        pathList.Insert(0, currentNode.data);
                        currentNode = currentNode.next;
                    }

                    return pathList;
                }



                // Goes to the adjacent cells
                for (int i = 0; i < 4; i++)
                {

                    int adjX = x + dirVectRow[i];
                    int adjY = y + dirVectCol[i];
                    (int, int) adjTuple = (adjX, adjY);

                    if (isValid(adjX, adjY, visited) && !traversedChokepoints.ContainsKey(adjTuple))
                    {
                        Node adjNode = new Node(adjTuple);
                        adjNode.next = currentNode;
                        adjNode.depth = currentNode.depth + 1;
                        queue.Enqueue(adjNode);

                        visited.Add(adjTuple, currentNode.depth + 1);
                    }
                }
            }
            return pathList;
        }

        //method gets the fastest path for 1 of a given sides pieces then 
        //slightly more efficient method for determining distance to center (vs 3 BFS's)? (roughly 2.75 BFS's)
        /*not efficient enough-----------------------------------------------------------------------------
        public List<(int, Dictionary<(int, int), (int,int)>)> getFastestPlayerPiecePathsLengths(GameBoard gamestate, bool isBlueTurn)
        {
            //method first gets 
            List<(int, Dictionary<(int, int), (int, int)>)> pathLengths = new List<(int, Dictionary<(int, int), (int, int)>)>();
            //to check against if the second or third paths piggyback
            List<Dictionary<(int, int), (int, int)>> piecePathList = new List<Dictionary<(int, int), (int, int)>>()
            {
                new Dictionary<(int, int), (int, int)>(),
                new Dictionary<(int, int), (int, int)>(),
                new Dictionary<(int, int), (int, int)>()
            };

            //Dictionary of tiles part of a "fastest path" + that tile's distance to the objective
            Dictionary<(int, int), (Node,int)> traveledList = new Dictionary<(int, int), (Node, int)>();
            //Dictionary of chokepoint tiles that complement ones present in traveled list
            Dictionary<(int, int), int> chokepointComplementList = new Dictionary<(int, int), int>();

            Node currentNode;

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

            for (int j = 0; j < 3; j++)
            {
                // Stores indices of the matrix cells
                Queue<Node> queue = new Queue<Node>();
                Dictionary<(int, int), int> visited = new Dictionary<(int, int), int>();

                Node n = new Node((pieceList[j][0], pieceList[j][1]));
                queue.Enqueue(n);
                visited.Add(n.data, n.depth);

                //depth that has to be beaten
                int depthLimit = 100;
                //Node that contacts a node on a fastest path
                Node connectionNode = new Node(n.data);

                // Iterate while the queue
                // is not empty
                while (queue.Count != 0)
                {
                    (int, int) cell = queue.FirstOrDefault().data;
                    int x = cell.Item1;
                    int y = cell.Item2;


                    currentNode = queue.Dequeue();

                    //if current node == objective, the pathList will be sent back
                    if (_gameBoard.gameBoard[x, y] == objective)
                    {
                        int movesFromGoal = 0;//moves from goal
                        Dictionary<(int, int), (int,int)> pathTaken = new Dictionary<(int, int), (int, int)>();
                        (int, int) objectiveLocation = currentNode.data;
                        while (currentNode.next != null)
                        {
                            traveledList.Add(currentNode.data, (currentNode, movesFromGoal));
                            piecePathList[j].Add(currentNode.data, objectiveLocation);
                            pathTaken.Add(currentNode.data, objectiveLocation);
                            if (!chokepointComplementList.ContainsKey(currentNode.data))
                            {
                                //adds the complementary coordinates to chokepointComplementList if currentNode is a chokepoint tile
                                if (nwChokepointList.ContainsKey(currentNode.data))
                                {
                                    chokepointComplementList.Add(nwChokepointList[currentNode.data].Item2, movesFromGoal + 1);
                                }
                                else if (seChokepointList.ContainsKey(currentNode.data))
                                {
                                    chokepointComplementList.Add(seChokepointList[currentNode.data].Item1, movesFromGoal + 1);
                                }
                            }
                            movesFromGoal++;
                            currentNode = currentNode.next;
                        }

                        pathLengths.Add((pathTaken.Count, pathTaken));
                        break;
                    }
                    //if current node hits a previous fastest path, it will calculate assuming that path will be followed

                    //if current "path" doesn't pass through a traveled chokepoint
                    if (currentNode.complementDir == 0)
                    {
                        // Goes to the adjacent cells
                        for (int i = 0; i < 4; i++)
                        {

                            int adjX = x + dirVectRow[i];
                            int adjY = y + dirVectCol[i];

                            //node expansion hits previously taken route
                            if (traveledList.ContainsKey((adjX, adjY)))
                            {
                                if (traveledList[(adjX, adjY)].Item2 + currentNode.depth < depthLimit)
                                {
                                    //new # moves to beat
                                    depthLimit = traveledList[(adjX, adjY)].Item2 + currentNode.depth;
                                    connectionNode = currentNode;
                                    connectionNode.connectionLoc = (adjX, adjY);
                                }
                            }
                            else if (isValid(adjX, adjY, visited) && currentNode.depth + 1 < depthLimit)
                            {
                                Node adjNode = new Node((adjX, adjY));
                                adjNode.next = currentNode;
                                adjNode.depth = currentNode.depth + 1;
                                if (chokepointComplementList.ContainsKey(adjNode.data))
                                {
                                    //1 = N, 2 = S, 3 = W, 4 = E -x = left -y = up
                                    if (nwChokepointList.ContainsKey(adjNode.data))
                                    {
                                        //if its N
                                        if (nwChokepointList[adjNode.data].Item1.Item1 == nwChokepointList[adjNode.data].Item2.Item1)
                                        {
                                            adjNode.complementDir = 1 * dirVectRow[i];
                                        }
                                        else
                                        {
                                            adjNode.complementDir = 3 * dirVectCol[i];
                                        }
                                    }
                                    else
                                    {
                                        if (seChokepointList[adjNode.data].Item1.Item1 == seChokepointList[adjNode.data].Item2.Item1)
                                        {
                                            adjNode.complementDir = 2 * dirVectRow[i];
                                        }
                                        else
                                        {
                                            adjNode.complementDir = 4 * dirVectCol[i];
                                        }
                                    }
                                }
                                queue.Enqueue(adjNode);
                                
                                visited.Add((adjX, adjY), currentNode.depth + 1);
                            }
                        }
                    }
                    //if current "path" passes through a traveled ChokePoint (will travel in one direction until it doent match a previous fastest path)
                    else
                    {
                        int adjX = x;
                        int adjY = y;
                        (int, int) checkDir = (0, 0);
                        switch (currentNode.complementDir)
                        {
                            //North side going left
                            case -1:
                                adjX = x - 1;
                                checkDir = (0, -1);
                                break;

                            //North side going right
                            case 1:
                                adjX = x + 1;
                                checkDir = (0, -1);
                                break;

                            //South side going left
                            case -2:
                                adjX = x - 1;
                                checkDir = (0, 1);
                                break;

                            //South side going right
                            case 2:
                                adjX = x + 1;
                                checkDir = (0, 1);
                                break;

                            //West side going left
                            case -3:
                                adjY = y - 1;
                                checkDir = (-1, 0);
                                break;

                            //West side going right
                            case 3:
                                adjY = y + 1;
                                checkDir = (-1, 0);
                                break;

                            //East side going left
                            case -4:
                                adjY = y - 1;
                                checkDir = (1, 0);
                                break;

                            //East side going right
                            case 4:
                                adjY = y + 1;
                                checkDir = (1, 0);
                                break;
                        }
                        //node expansion hits previously taken route
                        if (traveledList.ContainsKey((adjX, adjY)))
                        {
                            if (traveledList[(adjX, adjY)].Item2 + currentNode.depth < depthLimit)
                            {
                                //new # moves to beat
                                depthLimit = traveledList[(adjX, adjY)].Item2 + currentNode.depth;
                                connectionNode = currentNode;
                                connectionNode.connectionLoc = (adjX, adjY);
                            }
                        }
                        else if (isValid(adjX, adjY, visited) && currentNode.depth + 1 < depthLimit && traveledList.ContainsKey((adjX + checkDir.Item1, adjY + checkDir.Item1)))
                        {
                            Node adjNode = new Node((adjX, adjY));
                            adjNode.next = currentNode;
                            adjNode.depth = currentNode.depth + 1;
                            queue.Enqueue(adjNode);

                            visited.Add((adjX, adjY), currentNode.depth + 1);
                        }
                    }
                }
                //if depth limit was hit before objective reached (no trap rule would ensure reaching the center was possible)
                //if the while loop completes without adding a pathlength
                if (pathLengths.Count <= j)
                {
                    //Dictionary<(int, int), int> pathTaken = new Dictionary<(int, int), int>();
                    Node objectiveNode;
                    (int, int) objectiveNodeData;
                    if (piecePathList[0].ContainsKey(connectionNode.connectionLoc))
                    {
                        objectiveNode = traveledList[piecePathList[0][connectionNode.connectionLoc]].Item1;
                        objectiveNodeData = objectiveNode.data;
                    }
                    else
                    {
                        objectiveNode = traveledList[piecePathList[1][connectionNode.connectionLoc]].Item1;
                        objectiveNodeData = objectiveNode.data;
                    }
                    if(j < 2)
                    {
                        while (objectiveNode.next.data != connectionNode.connectionLoc)
                        {
                            piecePathList[j].Add(objectiveNode.data, objectiveNodeData);
                            objectiveNode = objectiveNode.next;
                        }
                        while (connectionNode.next != null)
                        {
                            piecePathList[j].Add(connectionNode.data, objectiveNodeData);
                            traveledList.Add(connectionNode.data, (connectionNode, depthLimit - (connectionNode.depth - 1)));
                            connectionNode = connectionNode.next;
                        }
                    }
                    else
                    {
                        while (objectiveNode.next != null)
                        {
                            piecePathList[j].Add(objectiveNode.data, objectiveNodeData);
                            objectiveNode = objectiveNode.next;
                        }
                    }
                    pathLengths.Add((piecePathList[j].Count, piecePathList[j]));
                }
            }
            return pathLengths;
        }
        */
        //returns all reachable locations for a given distance
        public List<(int, int)> getSpecificDistBFS(int row, int col, int dist, bool isBlueTurn)
        {
            List<(int, int)> possibleDestinations = new List<(int, int)>();
            Dictionary<(int,int), int> pieceList;
            if(isBlueTurn)
            {
                pieceList = new Dictionary<(int, int), int>()
                {
                    { (_gameBoard.bluePiece1[0], _gameBoard.bluePiece1[1]), 0},
                    { (_gameBoard.bluePiece2[0], _gameBoard.bluePiece2[1]), 0},
                    { (_gameBoard.bluePiece3[0], _gameBoard.bluePiece3[1]), 0},
                };
            }
            else
            {
                pieceList = new Dictionary<(int, int), int>()
                {
                    { (_gameBoard.redPiece1[0], _gameBoard.redPiece1[1]), 0},
                    { (_gameBoard.redPiece2[0], _gameBoard.redPiece2[1]), 0},
                    { (_gameBoard.redPiece3[0], _gameBoard.redPiece3[1]), 0},
                };
            }
            // Stores indices of the matrix cells
            Queue<Node> queue = new Queue<Node>();
            Dictionary<(int, int), int> visited = new Dictionary<(int, int), int>();

            Node currentNode;

            // Mark the starting cell as visited
            // and push it into the queue
            Node n = new Node((row, col));
            queue.Enqueue(n);
            visited.Add(n.data, n.depth);



            // Iterate while the queue
            // is not empty
            while (queue.Count != 0)
            {
                (int, int) cell = queue.FirstOrDefault().data;
                int x = cell.Item1;
                int y = cell.Item2;


                currentNode = queue.Dequeue();

                //for piece movement i am just looking for, assuming I have _gameboard
                if ((currentNode.depth == dist 
                    || _gameBoard.gameBoard[currentNode.data.Item1, currentNode.data.Item2] == (isBlueTurn ? "7B" : "7R")) 
                    && !pieceList.ContainsKey(currentNode.data))
                {
                    possibleDestinations.Add((x, y));
                    continue;
                }



                // Goes to the adjacent cells
                for (int i = 0; i < 4; i++)
                {

                    int adjX = x + dirVectRow[i];
                    int adjY = y + dirVectCol[i];

                    if (isValid(adjX, adjY, visited) && currentNode.depth < dist)
                    {
                        Node adjNode = new Node((adjX, adjY));
                        adjNode.next = currentNode;
                        adjNode.depth = currentNode.depth + 1;

                        queue.Enqueue(adjNode);
                        
                        visited.Add((adjX, adjY), currentNode.depth + 1);
                    }
                }
            }
            return possibleDestinations;
        }

        //finds how object can make it to position fastest maxes out 

        public List<(int, int)> getBfsToCoord(int xPos, int yPos, (int, int) dest, int maxDist)
        {
            // Stores indices of the matrix cells
            Queue<Node> queue = new Queue<Node>();
            Dictionary<(int, int), int> visited = new Dictionary<(int, int), int>();

            //linked thing for later
            List<(int, int)> pathList = new List<(int, int)>();
            Node currentNode;

            // Mark the starting cell as visited
            // and push it into the queue
            Node n = new Node((xPos, yPos));
            queue.Enqueue(n);
            visited.Add(n.data, n.depth);



            // Iterate while the queue
            // is not empty
            while (queue.Count != 0)
            {
                (int, int) cell = queue.FirstOrDefault().data;
                int x = cell.Item1;
                int y = cell.Item2;


                currentNode = queue.Dequeue();

                //if current node matches desired destination
                if (x == dest.Item1 && y == dest.Item2)
                {
                    while (currentNode.next != null)
                    {
                        pathList.Add(currentNode.data);
                        currentNode = currentNode.next;
                    }

                    return pathList;
                }
                else if (currentNode.depth >= maxDist)
                {
                    continue;
                }



                // Goes to the adjacent cells
                for (int i = 0; i < 4; i++)
                {

                    int adjX = x + dirVectRow[i];
                    int adjY = y + dirVectCol[i];

                    if (isValid(adjX, adjY, visited))
                    {
                        Node adjNode = new Node((adjX, adjY));
                        adjNode.next = currentNode;
                        adjNode.depth = currentNode.depth + 1;

                        queue.Enqueue(adjNode);
                        
                        visited.Add((adjX, adjY), currentNode.depth + 1);
                    }
                }
            }
            return pathList;
        }

        //returns true if objective found in zone with radius of dist (no longer in use)
        public bool isObjInZoneBFS(int row, int col, string objective, int dist)
        {
            // Stores indices of the matrix cells
            Queue<Node> queue = new Queue<Node>();
            Dictionary<(int, int), int> visited = new Dictionary<(int, int), int>();

            Node currentNode;

            // Mark the starting cell as visited
            // and push it into the queue
            Node n = new Node((row, col));
            queue.Enqueue(n);
            visited.Add(n.data, n.depth);



            // Iterate while the queue
            // is not empty
            while (queue.Count != 0)
            {
                (int, int) cell = queue.FirstOrDefault().data;
                int x = cell.Item1;
                int y = cell.Item2;


                currentNode = queue.Dequeue();

                if (_gameBoard.gameBoard[x, y] == objective)
                {
                    return true;
                }

                //for piece movement i am just looking for, assuming I have _gameboard
                else if (currentNode.depth == dist)
                {
                    continue;
                }



                // Goes to the adjacent cells
                for (int i = 0; i < 4; i++)
                {

                    int adjX = x + dirVectRow[i];
                    int adjY = y + dirVectCol[i];

                    if (isValid(adjX, adjY, visited))
                    {
                        Node adjNode = new Node((adjX, adjY));
                        adjNode.next = currentNode;
                        adjNode.depth = currentNode.depth + 1;

                        queue.Enqueue(adjNode);
                        
                        visited.Add((adjX, adjY), currentNode.depth + 1);

                    }
                }
            }
            return false;
        }

        //finds number of instances of player pieces around the minotaur
        public int[] getMinotaurZoneBFS()
        {
            // Stores indices of the matrix cells
            Queue<Node> queue = new Queue<Node>();
            Dictionary<(int, int), int> visited = new Dictionary<(int, int), int>();
            int[] pieceList = { 0, 0 };

            Node currentNode;

            // Mark the starting cell as visited
            // and push it into the queue
            Node n = new Node(((int)_gameBoard.minotuarPos[0], (int)_gameBoard.minotuarPos[1]));
            queue.Enqueue(n);
            visited.Add(n.data, n.depth);



            // Iterate while the queue
            // is not empty
            while (queue.Count != 0)
            {
                (int, int) cell = queue.FirstOrDefault().data;
                int x = cell.Item1;
                int y = cell.Item2;


                currentNode = queue.Dequeue();

                if (_gameBoard.gameBoard[x, y] == "4R")
                {
                    //adds 1 to blue counter
                    pieceList[0] += 1;
                }
                else if (_gameBoard.gameBoard[x, y] == "4B")
                {
                    //adds 1 to red counter
                    pieceList[1] += 1;
                }

                //for piece movement i am just looking for, assuming I have _gameboard
                //else if ()
                //{
                //    continue;
                //}



                // Goes to the adjacent cells
                for (int i = 0; i < 4; i++)
                {

                    int adjX = x + dirVectRow[i];
                    int adjY = y + dirVectCol[i];

                    if (isValid(adjX, adjY, visited) && currentNode.depth < 8)
                    {
                        Node adjNode = new Node((adjX, adjY));
                        adjNode.next = currentNode;
                        adjNode.depth = currentNode.depth + 1;

                        queue.Enqueue(adjNode);
                        
                        visited.Add((adjX, adjY), currentNode.depth + 1);
                    }
                }
            }
            return pieceList;
        }

        //determines is an opponent's player pieces can make it to an end zone
        public bool isOpponentTrapped(GameBoard gamestate, bool isBlueTurn)
        {
            Dictionary<(int, int), int> clearedPathsList = new Dictionary<(int, int), int>();
            Node currentNode;

            List<int[]> pieceList;
            string objective;
            if (isBlueTurn)
            {
                pieceList = new List<int[]>(){gamestate.redPiece1, gamestate.redPiece2, gamestate.redPiece3};
                objective = "7R";
            }
            else
            {
                pieceList = new List<int[]>() { gamestate.bluePiece1, gamestate.bluePiece2, gamestate.bluePiece3 };
                objective = "7B";
            }
            
            foreach (int[] piece in pieceList)
            {
                // Stores indices of the matrix cells
                Queue<Node> queue = new Queue<Node>();
                Dictionary<(int, int), int> visited = new Dictionary<(int, int), int>();
                Node n = new Node((piece[0], piece[1]));
                queue.Enqueue(n);
                visited.Add(n.data, n.depth);



                // Iterate while the queue
                // is not empty
                while (queue.Count != 0)
                {
                    (int, int) cell = queue.FirstOrDefault().data;
                    int x = cell.Item1;
                    int y = cell.Item2;


                    currentNode = queue.Dequeue();

                    //if current node == objective or part of a cleared path, add it to clearedPieceList
                    if (clearedPathsList.ContainsKey((x, y)) || gamestate.gameBoard[x, y] == objective)
                    {
                        break;
                    }

                    // Goes to the adjacent cells
                    for (int i = 0; i < 4; i++)
                    {

                        int adjX = x + dirVectRow[i];
                        int adjY = y + dirVectCol[i];

                        if (isValid(adjX, adjY, visited))
                        {
                            if (gamestate.gameBoard[adjX, adjY] == "0" || (isBlueTurn ? gamestate.gameBoard[adjX, adjY].Contains('R') :
                                                                                        gamestate.gameBoard[adjX, adjY].Contains('B')))
                            {
                                Node adjNode = new Node((adjX, adjY));
                                adjNode.next = currentNode;
                                adjNode.depth = currentNode.depth + 1;

                                queue.Enqueue(adjNode);
                            }
                            visited.Add((adjX, adjY), currentNode.depth + 1);
                            if (!clearedPathsList.ContainsKey((adjX, adjY)))
                            {
                                clearedPathsList.Add((adjX, adjY), adjX);
                            }
                            else
                            {
                                Console.WriteLine("Clear path redundancy: (" + adjX + ", " + adjY + ")");
                            }
                        }
                    }

                    if(queue.Count == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //visualizes the fastest routes of a given piece/pieces
        public void visualizeFitnessFunction(GameBoard gamestate)
        {
            //minotaur shit//////////
            if (gamestate.isBlackActive)
            {
                Queue<Node> queue = new Queue<Node>();
                Dictionary<(int, int), int> visited = new Dictionary<(int, int), int>();

                Node currentNode;

                // Mark the starting cell as visited
                // and push it into the queue
                Node n = new Node((gamestate.minotuarPos[0], gamestate.minotuarPos[1]));
                queue.Enqueue(n);
                visited.Add(n.data, n.depth);



                // Iterate while the queue
                // is not empty
                while (queue.Count != 0)
                {
                    (int, int) cell = queue.FirstOrDefault().data;
                    int x = cell.Item1;
                    int y = cell.Item2;


                    currentNode = queue.Dequeue();
                    gamestate.gameBoard[currentNode.data.Item1, currentNode.data.Item2] = "mR";

                    // Goes to the adjacent cells
                    for (int i = 0; i < 4; i++)
                    {

                        int adjX = x + dirVectRow[i];
                        int adjY = y + dirVectCol[i];

                        if (isValid(adjX, adjY, visited) && currentNode.depth < 8 && gamestate.gameBoard[adjX, adjY] != "7R")
                        {
                            Node adjNode = new Node((adjX, adjY));
                            adjNode.next = currentNode;
                            adjNode.depth = currentNode.depth + 1;

                            queue.Enqueue(adjNode);

                            visited.Add((adjX, adjY), currentNode.depth + 1);
                        }
                    }
                }
            }
            /////////////////////////

            List<(int, int)> bluePaths = new List<(int, int)> ();
            List<(int, int)> blueAltPaths = new List<(int, int)> ();
            List<(int, int)> redPaths = new List<(int, int)>();
            List<(int, int)> redAltPaths = new List<(int, int)>();
            Dictionary<(int, int), int> offLimitCoords = new Dictionary<(int, int), int>();

            /*foreach (var coords in dataList)
            {
                if (nwChokepointList.ContainsKey(coords.Key) && !offLimitCoords.ContainsKey(coords.Key))
                {
                    offLimitCoords.Add(coords.Key, 1);
                    offLimitCoords.Add(nwChokepointList[coords.Key].Item2, 1);
                }
                else if (seChokepointList.ContainsKey(coords.Key) && !offLimitCoords.ContainsKey(coords.Key))
                {
                    offLimitCoords.Add(coords.Key, 1);
                    offLimitCoords.Add(seChokepointList[coords.Key].Item1, 1);
                }
            }*/

            List<(int, int)> bP1 = BFS(gamestate.bluePiece1[0], gamestate.bluePiece1[1], "7B");
            foreach (var coords in bP1)
            {
                if (nwChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(nwChokepointList[coords].Item2, 1);
                }
                else if (seChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(seChokepointList[coords].Item1, 1);
                }
            
            }
            List<(int, int)> apB1 = findDifferentPlayerRoute(gamestate.bluePiece1[0], gamestate.bluePiece1[1], "7B", offLimitCoords);
            foreach (var coords in apB1)
            {
                if (nwChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(nwChokepointList[coords].Item2, 1);
                }
                else if (seChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(seChokepointList[coords].Item1, 1);
                }

            }
            List<(int, int)> apB2 = findDifferentPlayerRoute(gamestate.bluePiece1[0], gamestate.bluePiece1[1], "7B", offLimitCoords);
            foreach (var coords in apB2)
            {
                if (nwChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(nwChokepointList[coords].Item2, 1);
                }
                else if (seChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(seChokepointList[coords].Item1, 1);
                }

            }
            List<(int, int)> apB3 = findDifferentPlayerRoute(gamestate.bluePiece1[0], gamestate.bluePiece1[1], "7B", offLimitCoords);

            bP1.RemoveRange((bP1.Count - 1), 1);
            apB1.RemoveRange((apB1.Count - 1), 1);
            //apB2.RemoveRange((apB2.Count - 1), 1);
            //apB3.RemoveRange((apB3.Count - 1), 1);
            blueAltPaths.AddRange(apB1);
            //blueAltPaths.AddRange(apB2);
            //blueAltPaths.AddRange(apB3);
            
            foreach (var path in bP1)
            {
                gamestate.gameBoard[path.Item1, path.Item2] = "iB";
            }

            foreach (var path in blueAltPaths)
            {
                gamestate.gameBoard[path.Item1, path.Item2] = "apB";
            }
            /*offLimitCoords.Clear();

            List<(int, int)> bP2 = BFS(gamestate.bluePiece2[0], gamestate.bluePiece2[1], "7B");
            foreach (var coords in bP2)
            {
                if (nwChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(nwChokepointList[coords].Item2, 1);
                }
                else if (seChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(seChokepointList[coords].Item1, 1);
                }
            }
            List<(int, int)> apB2 = findDifferentPlayerRoute(gamestate.bluePiece2[0], gamestate.bluePiece2[1], "7B", offLimitCoords);
            offLimitCoords.Clear();

            List<(int, int)> bP3 = BFS(gamestate.bluePiece3[0], gamestate.bluePiece3[1], "7B");
            foreach (var coords in bP3)
            {
                if (nwChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(nwChokepointList[coords].Item2, 1);
                }
                else if (seChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(seChokepointList[coords].Item1, 1);
                }
            }
            List<(int, int)> apB3 = findDifferentPlayerRoute(gamestate.bluePiece3[0], gamestate.bluePiece3[1], "7B", offLimitCoords);
            offLimitCoords.Clear();

            bP1.RemoveRange((bP1.Count - 1), 1);
            apB1.RemoveRange((apB1.Count - 1), 1);
            bP2.RemoveRange(bP2.Count - 1, 1);
            apB2.RemoveRange((apB2.Count - 1), 1);
            bP3.RemoveRange(bP3.Count - 1, 1);
            apB3.RemoveRange((apB3.Count - 1), 1);
            bluePaths.AddRange(bP1);
            bluePaths.AddRange(bP2);
            bluePaths.AddRange(bP3);
            blueAltPaths.AddRange(apB1);
            blueAltPaths.AddRange(apB2);
            blueAltPaths.AddRange(apB3);

            List<(int, int)> rP1 = BFS(gamestate.redPiece1[0], gamestate.redPiece1[1], "7R");
            foreach (var coords in rP1)
            {
                if (nwChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(nwChokepointList[coords].Item2, 1);
                }
                else if (seChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(seChokepointList[coords].Item1, 1);
                }
            }
            List<(int, int)> apR1 = findDifferentPlayerRoute(gamestate.redPiece1[0], gamestate.redPiece1[1], "7R", offLimitCoords);
            offLimitCoords.Clear();

            List<(int, int)> rP2 = BFS(gamestate.redPiece2[0], gamestate.redPiece2[1], "7R");
            foreach (var coords in rP2)
            {
                if (nwChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(nwChokepointList[coords].Item2, 1);
                }
                else if (seChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(seChokepointList[coords].Item1, 1);
                }
            }
            List<(int, int)> apR2 = findDifferentPlayerRoute(gamestate.redPiece2[0], gamestate.redPiece2[1], "7R", offLimitCoords);
            offLimitCoords.Clear();

            List<(int, int)> rP3 = BFS(gamestate.redPiece3[0], gamestate.redPiece3[1], "7R");
            foreach (var coords in rP3)
            {
                if (nwChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(nwChokepointList[coords].Item2, 1);
                }
                else if (seChokepointList.ContainsKey(coords) && !offLimitCoords.ContainsKey(coords))
                {
                    offLimitCoords.Add(coords, 1);
                    offLimitCoords.Add(seChokepointList[coords].Item1, 1);
                }
            }
            List<(int, int)> apR3 = findDifferentPlayerRoute(gamestate.redPiece3[0], gamestate.redPiece3[1], "7R", offLimitCoords);

            rP1.RemoveRange(rP1.Count - 1, 1);
            apR1.RemoveRange(apR1.Count - 1, 1);
            rP2.RemoveRange(rP2.Count - 1, 1);
            apR2.RemoveRange(apR2.Count - 1, 1);
            rP3.RemoveRange(rP3.Count - 1, 1);
            apR3.RemoveRange(apR3.Count - 1, 1);
            redPaths.AddRange(rP1);
            redPaths.AddRange(rP2);
            redPaths.AddRange(rP3);
            redAltPaths.AddRange(apR1);
            redAltPaths.AddRange(apR3);
            redAltPaths.AddRange(apR3);
            foreach (var path in blueAltPaths)
            {
                gamestate.gameBoard[path.Item1, path.Item2] = "apB";
            }
            foreach (var path in redAltPaths)
            {
                gamestate.gameBoard[path.Item1, path.Item2] = "apR";
            }

            foreach (var path in bluePaths)
            {
                gamestate.gameBoard[path.Item1, path.Item2] = "iB";
            }
            foreach (var path in redPaths)
            {
                gamestate.gameBoard[path.Item1, path.Item2] = "iR";
            }
            */
        }

        //visualizes the paths for testing purposes
        public List<int> altRouteTester(int row, int col, string objective)
        {
            List<int> pathLengthList = new List<int>();
            Dictionary<(int, int), int> traveledList = new Dictionary<(int, int), int>();

            //includes destination at this point
            List<(int, int)> fastestPath = BFS(row, col, objective);
            if (fastestPath.Count != 0)
            {
                pathLengthList.Add(fastestPath.Count);
            }
            else
            {
                pathLengthList.Add(55);
            }

            //for testing

            foreach (var cord in fastestPath)
            {
                if (!traveledList.ContainsKey(cord))
                {
                    traveledList.Add(cord, 0);
                }
            }

            List<(int, int)> secondFastestPath = findDifferentPlayerRoute(row, col, objective, traveledList);
            if (secondFastestPath.Count != 0)
            {
                pathLengthList.Add(secondFastestPath.Count);
            }
            else
            {
                pathLengthList.Add(60);
            }

            foreach (var cord in secondFastestPath)
            {
                if (!traveledList.ContainsKey(cord))
                {
                    traveledList.Add(cord, 0);
                }
            }

            List<(int, int)> thirdFastestPath = findDifferentPlayerRoute(row, col, objective, traveledList);
            if (thirdFastestPath.Count != 0)
            {
                pathLengthList.Add(thirdFastestPath.Count);
            }
            else
            {
                pathLengthList.Add(50);
            }


            //tester
            foreach ((int, int) item in fastestPath)
            {
                _gameBoard.gameBoard[item.Item1, item.Item2] = objective;
            }
            foreach ((int, int) item in secondFastestPath)
            {
                _gameBoard.gameBoard[item.Item1, item.Item2] = objective;
            }
            foreach ((int, int) item in thirdFastestPath)
            {
                _gameBoard.gameBoard[item.Item1, item.Item2] = objective;
            }

            return pathLengthList;
        }
    }
}
