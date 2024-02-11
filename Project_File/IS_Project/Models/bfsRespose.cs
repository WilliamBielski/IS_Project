using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Project.Models
{
    public class bfsRespose
    {
        public List<(int, int)> dataList;
        public List<Node> nodeList;
        public bfsRespose()
        {
            dataList = new List<(int, int)>();
            nodeList = new List<Node>();
        }
    }
}
