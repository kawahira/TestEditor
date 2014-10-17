using System;

namespace GenerateMap
{
    public struct Config
    {
        public ushort height;
        public ushort width;
        public byte minRoomSize;
        public byte marginRoomSize;
        public byte addRoadMax;
        public byte iconBlank;
        public byte iconRoom;
        public byte iconRoad;
        public byte iconRoomAndRoad;
        public byte iconRoomWall;
        public byte iconRoadWall;
    }
}
