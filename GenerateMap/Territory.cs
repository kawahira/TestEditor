using System;
using System.Collections.Generic;

namespace GenerateMap
{
    public class Territory
    {
        private bool done_split_v;
        private bool done_split_h;
        public int lx, ly, hx, hy;
        public Room room;
        public Territory(ref List<Territory> territoryList, int x1, int y1, int x2, int y2)
        {
            lx = x1;
            ly = y1;
            hx = x2;
            hy = y2;
            territoryList.Add(this);
        }
        public void Build(ref List<Territory> territoryList, ref List<Road> roadList, ushort minTerritorySize,byte minRoomSize, byte marginRoomSize)
        {
            Split(ref territoryList, ref roadList, minTerritorySize);
            int count = 0;
            foreach (Territory r in territoryList)
            {
                r.room = new Room(null,null,0.0);
                r.room.Build(r.lx, r.ly, r.hx, r.hy, minRoomSize, marginRoomSize);
                r.room.index = count;
                ++count;
            }
        }
        private void Split(ref List<Territory> territoryList, ref List<Road> roadList, ushort minTerritorySize)
        {
            // Clip Check.
            if ((this.hy - this.ly) <= (minTerritorySize * 2)) this.done_split_v = true;
            if ((this.hx - this.lx) <= (minTerritorySize * 2)) this.done_split_h = true;
            if ((this.done_split_v) && (this.done_split_h)) return;

            Territory child = new Territory(ref territoryList, this.lx, this.ly, this.hx, this.hy);

            Road.Direction direction = Road.Direction.Veritical;
            // どちらかしか同時に行わない
            if (this.done_split_v == false)
            {
                this.hy = child.ly = RandXorShift.Instance.Stage.Next(this.ly + minTerritorySize, this.hy - minTerritorySize);
                if (!((this.hy - this.ly) >= (minTerritorySize * 3) && (child.hy - child.ly) >= (minTerritorySize * 3)))
                {
                    this.done_split_v = child.done_split_v = true;
                }
            }
            else
            {
                this.hx = child.lx = RandXorShift.Instance.Stage.Next(this.lx + minTerritorySize, this.hx - minTerritorySize);
                if (!((this.hx - this.lx) >= (minTerritorySize * 3) && (child.hx - child.lx) >= (minTerritorySize * 3)))
                {
                    this.done_split_h = child.done_split_h = true;
                }
                direction = Road.Direction.Horizonal;
            }
            new Road(ref roadList, this, child, direction);
            this.Split(ref territoryList, ref roadList, minTerritorySize);
            child.Split(ref territoryList, ref roadList, minTerritorySize);
        }
        public void DrawBefore(ref Mapchip mapchip, byte icon)
        {
            mapchip.Line(room.lx, room.ly, room.hx, room.ly, icon);
            mapchip.Line(room.lx, room.hy, room.hx, room.hy, icon);
            mapchip.Line(room.lx, room.ly, room.lx, room.hy, icon);
            mapchip.Line(room.hx, room.ly, room.hx, room.hy, icon);
        }
        public void DrawAfter(ref Mapchip mapchip, byte icon)
        {
            mapchip.Fill(room.lx + 1, room.ly + 1, room.hx - 1, room.hy - 1, icon);
        }
    };
}
