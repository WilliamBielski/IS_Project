using IS_Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public bool isValid(int row, int col, List<(int, int)> visited)
        {
            // If cell lies out of bounds
            if (row < 0 || col < 0 || row >= rowBound || col >= colBound)
            {
                return false;
            }

            // If cell is already visited
            //(int, int) pos = ;
            if (visited.Contains((row, col)))
            {
                return false;
            }

            // Otherwise
            return true;
        }

        // Function to perform the BFS traversal, returns linked list of directions for shorted path.
        public bfsRespose BFS(int row, int col, string objective)
        {
            // Stores indices of the matrix cells
            Queue<Node> queue = new Queue<Node>();
            List<(int, int)> visited = new List<(int, int)>();

            //linked thing for later
            bfsRespose pathList = new bfsRespose();
            Node currentNode;

            // Mark the starting cell as visited
            // and push it into the queue
            Node n = new Node((row, col));
            queue.Enqueue(n);
            visited.Add(n.data);



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
                        pathList.dataList.Insert(0, currentNode.data);
                        pathList.nodeList.Insert(0, currentNode);
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
                        if (!_gameBoard.impassableList.Contains(_gameBoard.gameBoard[adjX, adjY]))
                        {
                            Node adjNode = new Node((adjX, adjY));
                            adjNode.next = currentNode;
                            adjNode.depth = currentNode.depth + 1;

                            queue.Enqueue(adjNode);
                        }
                        visited.Add((adjX, adjY));

                    }
                }
            }
            if (objective == "4R")
            {
                return BFS(row, col, "5R");
            }
            else if (objective == "4B")
            {
                return BFS(row, col, "5B");
            }
            else
            {
                return pathList;
            }
        }

        public List<(int, int)> findPlayerRoute(int row, int col, string objective, List<(int, int)> dataList)
        {
            // Stores indices of the matrix cells
            Queue<Node> queue = new Queue<Node>();
            List<(int, int)> visited = new List<(int, int)>();
            List<(int, int)> offLimitCoords = new List<(int, int)>();
            List<(int, int)> pathList = new List<(int, int)>();
            Node currentNode;

            foreach ((int, int) coords in dataList)
            {
                if (nwChokepointList.Contains(coords))
                {
                    offLimitCoords.Add(coords);
                    offLimitCoords.Add(seChokepointList[nwChokepointList.IndexOf(coords)]);
                }
                else if (seChokepointList.Contains(coords))
                {
                    offLimitCoords.Add(coords);
                    offLimitCoords.Add(nwChokepointList[seChokepointList.IndexOf(coords)]);
                }
            }

            // Mark the starting cell as visited
            // and push it into the queue
            Node n = new Node((row, col));
            queue.Enqueue(n);
            visited.Add(n.data);

            currentNode = queue.Dequeue();

            for (int k = 0; k < 4; k++)
            {

                int adjX = currentNode.data.Item1 + dirVectRow[k];
                int adjY = currentNode.data.Item2 + dirVectCol[k];
                (int, int) adjTuple = (adjX, adjY);

                if (isValid(adjX, adjY, visited))
                {
                    if (!_gameBoard.impassableList.Contains(_gameBoard.gameBoard[adjX, adjY]))
                    {
                        Node adjNode = new Node(adjTuple);
                        adjNode.next = currentNode;
                        adjNode.depth = currentNode.depth + 1;

                        queue.Enqueue(adjNode);
                    }
                    visited.Add(adjTuple);

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

                    if (isValid(adjX, adjY, visited) && !offLimitCoords.Contains(adjTuple))
                    {
                        if (!_gameBoard.impassableList.Contains(_gameBoard.gameBoard[adjX, adjY]))
                        {
                            Node adjNode = new Node(adjTuple);
                            adjNode.next = currentNode;
                            adjNode.depth = currentNode.depth + 1;

                            queue.Enqueue(adjNode);
                        }
                        visited.Add(adjTuple);

                    }
                }
            }
            return pathList;
        }

        public List<int> getPathLength(int row, int col, string objective)
        {
            List<int> pathLengthList = new List<int>();
            List<(int, int)> traveledList = new List<(int, int)>();

            //includes destination at this point
            bfsRespose fastestPath = BFS(row, col, objective);
            if (fastestPath.dataList.Count != 0)
            {
                pathLengthList.Add(fastestPath.dataList.Count);
            }
            else
            {
                pathLengthList.Add(55);
            }

            //for testing

            traveledList.AddRange(fastestPath.dataList);

            List<(int, int)> secondFastestPath = findPlayerRoute(row, col, objective, traveledList);
            if (secondFastestPath.Count != 0)
            {
                pathLengthList.Add(secondFastestPath.Count);
            }
            else
            {
                pathLengthList.Add(60);
            }

            traveledList.AddRange(secondFastestPath);

            List<(int, int)> thirdFastestPath = findPlayerRoute(row, col, objective, traveledList);
            if (thirdFastestPath.Count != 0)
            {
                pathLengthList.Add(thirdFastestPath.Count);
            }
            else
            {
                pathLengthList.Add(50);
            }


            //tester
            //foreach ((int, int) item in fastestPath.dataList)
            //{
            //    _gameBoard.gameBoard[item.Item1, item.Item2] = objective;
            //}
            //foreach ((int, int) item in secondFastestPath.dataList)
            //{
            //    _gameBoard.gameBoard[item.Item1, item.Item2] = objective;
            //}
            //foreach ((int, int) item in thirdFastestPath.dataList)
            //{
            //    _gameBoard.gameBoard[item.Item1, item.Item2] = objective;
            //}

            return pathLengthList;
        }

        //slightly more efficient method for determining distance to center (vs 3 BFS's)?
        // roughly 2.5 BFS's
        public List<int> getFastestPlayerPiecePathLengths(GameBoard gamestate, bool isBlueTurn)
        {
            List<int> pathLengths = new List<int>();

            //x, y, moves till end-zone
            List<(int, int)> pathList = new List<(int, int)>();
            List<int> depthList = new List<int>();
            Node currentNode;

            List<int[]> pieceList;
            string objective;
            if (isBlueTurn)
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
                List<(int, int)> visited = new List<(int, int)>();
                Node n = new Node((pieceList[j][0], pieceList[j][1]));
                queue.Enqueue(n);
                visited.Add(n.data);

                int remainingMoves = 100;
                int nodesInQueue = 100;
                int numToBeat = 100;

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
                        int counter = 0;
                        while (currentNode.next != null)
                        {
                            pathList.Insert(0, (currentNode.data.Item1, currentNode.data.Item2));
                            depthList.Insert(0, counter);
                            counter++;
                            currentNode = currentNode.next;
                        }
                        pathLengths.Add(pathList.Count - pathLengths.Sum());
                        break;
                    }
                    //if current node hits a previous fastest path, it will calculate assuming that path will be followed


                    // Goes to the adjacent cells
                    for (int i = 0; i < 4; i++)
                    {

                        int adjX = x + dirVectRow[i];
                        int adjY = y + dirVectCol[i];

                        if (pathList.Contains((adjX, adjY)))
                        {
                            if (remainingMoves > 0 && depthList[pathList.IndexOf((adjX, adjY))] < remainingMoves)
                            {
                                remainingMoves = depthList[pathList.IndexOf((adjX, adjY))];
                                nodesInQueue = queue.Count;
                            }
                        }
                        else if (isValid(adjX, adjY, visited))
                        {
                            if (!_gameBoard.impassableList.Contains(_gameBoard.gameBoard[adjX, adjY]))
                            {
                                Node adjNode = new Node((adjX, adjY));
                                adjNode.next = currentNode;
                                adjNode.depth = currentNode.depth + 1;
                                queue.Enqueue(adjNode);
                            }
                            visited.Add((adjX, adjY));
                        }
                    }
                    if (nodesInQueue > 0)
                    {
                        nodesInQueue--;
                        if (remainingMoves > 0 && nodesInQueue == 0)
                        {
                            remainingMoves--;
                            nodesInQueue = queue.Count;
                        }
                    }
                    if (remainingMoves == 0)
                    {
                        pathLengths.Add(numToBeat);
                        break;
                    }
                     
                }
            }
            return pathLengths;
        }

        public List<(int, int)> getSpecificDistBFS(int row, int col, int dist)
        {
            List<(int, int)> possibleDestinations = new List<(int, int)>();

            // Stores indices of the matrix cells
            Queue<Node> queue = new Queue<Node>();
            List<(int, int)> visited = new List<(int, int)>();

            Node currentNode;

            // Mark the starting cell as visited
            // and push it into the queue
            Node n = new Node((row, col));
            queue.Enqueue(n);
            visited.Add(n.data);



            // Iterate while the queue
            // is not empty
            while (queue.Count != 0)
            {
                (int, int) cell = queue.FirstOrDefault().data;
                int x = cell.Item1;
                int y = cell.Item2;


                currentNode = queue.Dequeue();

                //for piece movement i am just looking for, assuming I have _gameboard
                if (currentNode.depth == dist || _gameBoard.gameBoard[currentNode.data.Item1, currentNode.data.Item2].Contains('7'))
                {
                    possibleDestinations.Add((x, y));
                    continue;
                }



                // Goes to the adjacent cells
                for (int i = 0; i < 4; i++)
                {

                    int adjX = x + dirVectRow[i];
                    int adjY = y + dirVectCol[i];

                    if (isValid(adjX, adjY, visited))
                    {
                        if (!_gameBoard.impassableList.Contains(_gameBoard.gameBoard[adjX, adjY]))
                        {
                            Node adjNode = new Node((adjX, adjY));
                            adjNode.next = currentNode;
                            adjNode.depth = currentNode.depth + 1;

                            queue.Enqueue(adjNode);
                        }
                        visited.Add((adjX, adjY));

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
            List<(int, int)> visited = new List<(int, int)>();

            //linked thing for later
            List<(int, int)> pathList = new List<(int, int)>();
            Node currentNode;

            // Mark the starting cell as visited
            // and push it into the queue
            Node n = new Node((xPos, yPos));
            queue.Enqueue(n);
            visited.Add(n.data);



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
                        if (!_gameBoard.impassableList.Contains(_gameBoard.gameBoard[adjX, adjY]))
                        {
                            Node adjNode = new Node((adjX, adjY));
                            adjNode.next = currentNode;
                            adjNode.depth = currentNode.depth + 1;

                            queue.Enqueue(adjNode);
                        }
                        visited.Add((adjX, adjY));

                    }
                }
            }
            return pathList;
        }

        //returns true if objective found in zone with radius of dist
        public bool isObjInZoneBFS(int row, int col, string objective, int dist)
        {
            // Stores indices of the matrix cells
            Queue<Node> queue = new Queue<Node>();
            List<(int, int)> visited = new List<(int, int)>();

            Node currentNode;

            // Mark the starting cell as visited
            // and push it into the queue
            Node n = new Node((row, col));
            queue.Enqueue(n);
            visited.Add(n.data);



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
                        if (!_gameBoard.impassableList.Contains(_gameBoard.gameBoard[adjX, adjY]))
                        {
                            Node adjNode = new Node((adjX, adjY));
                            adjNode.next = currentNode;
                            adjNode.depth = currentNode.depth + 1;

                            queue.Enqueue(adjNode);
                        }
                        visited.Add((adjX, adjY));

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
            List<(int, int)> visited = new List<(int, int)>();
            int[] pieceList = { 0, 0 };

            Node currentNode;

            // Mark the starting cell as visited
            // and push it into the queue
            Node n = new Node(((int)_gameBoard.minotuarPos[0], (int)_gameBoard.minotuarPos[1]));
            queue.Enqueue(n);
            visited.Add(n.data);



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
                else if (currentNode.depth == 8)
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
                        if (!_gameBoard.impassableList.Contains(_gameBoard.gameBoard[adjX, adjY]))
                        {
                            Node adjNode = new Node((adjX, adjY));
                            adjNode.next = currentNode;
                            adjNode.depth = currentNode.depth + 1;

                            queue.Enqueue(adjNode);
                        }
                        visited.Add((adjX, adjY));

                    }
                }
            }
            return pieceList;
        }

        //determines is an opponent's player pieces can make it to an end zone
        public bool isOpponentTrapped(GameBoard gamestate, bool isBlueTurn)
        {
            List<(int, int)> clearedPathsList = new List<(int, int)>();
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
                List<(int, int)> visited = new List<(int, int)>();
                Node n = new Node((piece[0], piece[1]));
                queue.Enqueue(n);
                visited.Add(n.data);



                // Iterate while the queue
                // is not empty
                while (queue.Count != 0)
                {
                    (int, int) cell = queue.FirstOrDefault().data;
                    int x = cell.Item1;
                    int y = cell.Item2;


                    currentNode = queue.Dequeue();

                    //if current node == objective or part of a cleared path, add it to clearedPieceList
                    if (clearedPathsList.Contains((x, y)) || _gameBoard.gameBoard[x, y] == objective)
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
                            if (!_gameBoard.impassableList.Contains(_gameBoard.gameBoard[adjX, adjY]))
                            {
                                Node adjNode = new Node((adjX, adjY));
                                adjNode.next = currentNode;
                                adjNode.depth = currentNode.depth + 1;

                                queue.Enqueue(adjNode);
                            }
                            visited.Add((adjX, adjY));
                            clearedPathsList.Add((adjX, adjY));
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



        //visualizes the paths for testing purposes
        public List<int> pTester(int row, int col, string objective)
        {
            List<int> pathLengthList = new List<int>();
            List<(int, int)> traveledList = new List<(int, int)>();

            //includes destination at this point
            bfsRespose fastestPath = BFS(row, col, objective);
            if (fastestPath.dataList.Count != 0)
            {
                pathLengthList.Add(fastestPath.dataList.Count);
            }
            else
            {
                pathLengthList.Add(55);
            }

            //for testing

            traveledList.AddRange(fastestPath.dataList);

            List<(int, int)> secondFastestPath = findPlayerRoute(row, col, objective, traveledList);
            if (secondFastestPath.Count != 0)
            {
                pathLengthList.Add(secondFastestPath.Count);
            }
            else
            {
                pathLengthList.Add(60);
            }

            traveledList.AddRange(secondFastestPath);

            List<(int, int)> thirdFastestPath = findPlayerRoute(row, col, objective, traveledList);
            if (thirdFastestPath.Count != 0)
            {
                pathLengthList.Add(thirdFastestPath.Count);
            }
            else
            {
                pathLengthList.Add(50);
            }


            //tester
            foreach ((int, int) item in fastestPath.dataList)
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
