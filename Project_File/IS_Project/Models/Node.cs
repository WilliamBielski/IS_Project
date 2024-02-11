using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Project.Models
{
    public class Node
    {
        public (int, int) data;
        public Node next;
        public int depth;
        //public int complementDir;
        //public (int, int) connectionLoc;
        public Node() { }
        public Node((int, int) inputData)
        {
            data = inputData;
            next = null;
            depth = 0;
            //complementDir = 0;
            //connectionLoc = (0,0);
        }
    }
}
