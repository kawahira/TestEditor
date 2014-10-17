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
        public byte iconRoomFloor;
        public byte iconRoomWall;
        public byte iconRoomAndRoad;
        
        public byte iconRoad;
        public byte iconRoadWall;
    }
}
