// Racanhack 法により自動生成
// original source http://racanhack.sourceforge.jp/rhdoc/index.html 

using System;
using System.Collections;
using System.Collections.Generic;

namespace GenerateMap
{
    public class Room : Node.Pathfinding.AStarNode
    {
        public int index;
        public List<Road> roadList = new List<Road>();
        public int lx, ly, hx, hy;
        public Room(Room AParent, Room AGoalNode, double ACost)
            : base(AParent, AGoalNode, ACost)
        {
        }
        public void Build(int sx, int sy, int ex, int ey, byte minRoomSize, byte marginRoomSize)
        {
            int w, h;
            w = RandXorShift.Instance.Stage.Next(minRoomSize, ex - sx - (marginRoomSize * 2) + 1);
            h = RandXorShift.Instance.Stage.Next(minRoomSize, ey - sy - (marginRoomSize * 2) + 1);
            float distX = (float)((float)w / (float)(ex - sx));
            if (distX < 0.4f)
            {
                //                    w *= 3;
            }
            float distY = (float)((float)h / (float)(ey - sy));
            if (distY < 0.4f)
            {
                //                    h *= 3;
            }
            lx = RandXorShift.Instance.Stage.Next(sx + marginRoomSize, ex - marginRoomSize - w + 1);
            ly = RandXorShift.Instance.Stage.Next(sy + marginRoomSize, ey - marginRoomSize - h + 1);
            hx = lx + w;
            hy = ly + h;
        }
        public int GetWidth
        {
            get
            {
                return (hx - lx) / 2;
            }
        }
        public int GetHeight { get { return (hy - ly) / 2; } }
        public void GetCenter(ref int x, ref int y)
        {
            x = lx + GetWidth;
            y = ly + GetHeight;
        }
        #region Private Methods

        /// <summary>
        /// Determines wheather the current node is the same state as the on passed.
        /// </summary>
        /// <param name="ANode">AStarNode to compare the current node to</param>
        /// <returns>Returns true if they are the same state</returns>
        public override bool IsSameState(Node.Pathfinding.AStarNode ANode)
        {
            if (ANode == null)
            {
                return false;
            }
            return this.index == ((Room)ANode).index;
        }

        /// <summary>
        /// Adds a successor to a list if it is not impassible or the parent node
        /// </summary>
        /// <param name="ASuccessors">List of successors</param>
        private void AddSuccessor(ArrayList ASuccessors, Room r, Double AddCost)
        {
            Room newRoom = new Room(this, (Room)GoalNode, Cost + AddCost);
                newRoom.index = r.index;
                newRoom.lx = r.lx;
                newRoom.ly = r.ly;
                newRoom.hx = r.hx;
                newRoom.hy = r.hy;
                foreach (Road road in r.roadList)
                {
                    newRoom.roadList.Add(road);
                }
            if (newRoom.IsSameState(Parent))
            {
                return;
            }
            ASuccessors.Add(newRoom);
        }

        #endregion
        /// <summary>
        /// Gets all successors nodes from the current node and adds them to the successor list
        /// </summary>
        /// <param name="ASuccessors">List in which the successors will be added</param>
        public override void GetSuccessors(ArrayList ASuccessors)
        {
            ASuccessors.Clear();
            foreach (Road r in roadList)
            {
                AddSuccessor(ASuccessors, r.t0.room.index == this.index ? r.t1.room : r.t0.room, r.length);
            }
        }
		/// <summary>
		/// Prints information about the current node
		/// </summary>
		public override void PrintNodeInfo()
		{
            int parentIdx = -1;
            if (Parent != null)
            {
                parentIdx = ((Room)Parent).index;
            }
            int goalIdx = -1;
            if (GoalNode != null)
            {
                goalIdx = ((Room)GoalNode).index;
            }
            Console.WriteLine("parrent:\t{0}\tindex:\t{1}\tCost:\t{2}\tEst:\t{3}\tTotal:\t{4}\tGoal:\t{5}", parentIdx, index, Cost, GoalEstimate, TotalCost, goalIdx);
		}

    }
}
