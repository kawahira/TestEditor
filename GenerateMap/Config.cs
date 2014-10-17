// Racanhack 法により自動生成
// original source http://racanhack.sourceforge.jp/rhdoc/index.html 

using System;
using System.Collections.Generic;

namespace GenerateMap
{
    public class Config
    {
        public struct Icon
        {
            ushort room;
            ushort floor;
            ushort road;
        }
        public int randomSeed;
        public ushort height;
        public ushort width;
        public byte minRoomSize;
        public byte marginRoomSize;
        public byte addRoadMax;

        public byte iconBlank;
        public byte iconRoom;
        public byte iconRoomFloor;
        public byte iconRoomWall;
        public byte iconRoomAndRoad;
        
        public byte iconRoad;
        public byte iconRoadWall;
        public List<Replace> replaceList = new List<Replace>();
    }
}
