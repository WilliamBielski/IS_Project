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

        public MinMaxNode(GameBoard gameState)
        {
            children = new List<MinMaxNode>();
            minmaxGameboard = (GameBoard)gameState.deepCopy();
            currentMove = new List<((int, int), (int, int))>();
        }
    }
}
