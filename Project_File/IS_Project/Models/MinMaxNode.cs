using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Project.Models
{
    public class MinMaxNode
    {
        public List<MinMaxNode> children;
        public GameBoard minmaxGameboard;
        public List<((int, int), (int, int))> currentMove;
        public List<int> bluePathVals;
        public List<int> redPathVals;

        public MinMaxNode(GameBoard gameState)
        {
            children = new List<MinMaxNode>();
            minmaxGameboard = (GameBoard)gameState.deepCopy();
            currentMove = new List<((int, int), (int, int))>();
            bluePathVals = new List<int>() { 0, 0, 0};
            redPathVals = new List<int>() { 0, 0, 0};
        }

        public MinMaxNode(GameBoard gameState, List<int> bluePaths, List<int> redPaths)
        {
            children = new List<MinMaxNode>();
            minmaxGameboard = (GameBoard)gameState.deepCopy();
            currentMove = new List<((int, int), (int, int))>();
            bluePathVals = new List<int>() { bluePaths[0], bluePaths[1], bluePaths[2] };
            redPathVals = new List<int>() { redPaths[0], redPaths[1], redPaths[2] };
        }
    }
}
