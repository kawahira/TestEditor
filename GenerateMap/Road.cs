using System;
using System.Collections.Generic;

namespace GenerateMap
{
    public class Road
    {
        public enum Direction
        {
            Veritical = 0,
            Horizonal = 1
        }
        public Direction direction;
        public Territory t0;
        public Territory t1;
        public Road(ref Lists lists, Territory r0, Territory r1, Direction dir)
        {
            t0 = r0;
            t1 = r1;
            lists.road.Add(this);
            direction = dir;
        }
    };
}
