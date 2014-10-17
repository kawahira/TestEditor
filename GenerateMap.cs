using System;
using System.Collections.Generic;

namespace Generate
{
    public class Map
    {
        public class Replace
        {
            public int org;
            public int rep;
        }
        public List<Replace[,]> replaceList = new List<Replace[,]>();
        void ReplaceDataDebug()
        {
            {   // 「をカーブに変形
                int icon = 3;
                Replace[,] topleft = new Replace[5, 5];
                for (int i = 0; i < topleft.GetLength(0); i++)
                {
                    for (int j = 0; j < topleft.GetLength(1); j++)
                    {
                        topleft[i, j] = new Replace();
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    topleft[i + 1, 1].org = icon;
                    topleft[1, i + 1].org = icon;
                }
                topleft[1, 3].rep = icon;
                topleft[1, 4].rep = icon;
                topleft[2, 2].rep = icon;
                topleft[2, 3].rep = icon;
                topleft[3, 1].rep = icon;
                topleft[3, 2].rep = icon;
                topleft[4, 1].rep = icon;
                replaceList.Add(topleft);
            }
        }
        bool ChackSame(Replace[,] r, int x, int y)
        {
            for (int i = 0; i < r.GetLength(0); i++)
            {
                for (int j = 0; j < r.GetLength(1); j++)
                {
                    if (entity[x + i, y + j] != r[i, j].org) return false;
                }
            }
            return true;
        }
        void Copy(Replace[,] r, int x, int y)
        {
            for (int i = 0; i < r.GetLength(0); i++)
            {
                for (int j = 0; j < r.GetLength(1); j++)
                {
                    entity[x + i, y + j] = r[i, j].rep;
                }
            }
        }
        void ReplaceA()
        {
            ReplaceDataDebug();
            foreach (Replace[,] r in replaceList)
            {
                int w = config.width - r.GetLength(0);
                int h = config.height - r.GetLength(1);
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        if (ChackSame(r, i, j))
                        {
                            Copy(r, i, j);
                        }
                    }
                }
            }
        }
        public enum Direction
        {
            Veritical = 0,
            Horizonal = 1
        }
        public struct Config
        {
            public ushort height;
            public ushort width;
            public byte minRoomSize;
            public byte marginRoomSize;
            public byte addRoadRandomRate;
            public byte iconBlank;
            public byte iconRoom;
            public byte iconRoad;
            public byte iconRoomAndRoad;
            public byte iconRoomWall;
            public byte iconRoadWall;
        }
        public Config config;
        public int[,] entity;
        public class Lists
        {
            public List<Territory> territory = new List<Territory>();
            public List<Road> road = new List<Road>();
        };
        public Lists lists = new Lists();
        public ushort minTerritorySize
        {
            get
            {
                return (ushort)(config.minRoomSize + (config.marginRoomSize * 2));
            }
        }

        public Map()
        {
            config.minRoomSize = 8;
            config.marginRoomSize = 2;
            config.height = 400 / 4;
            config.width = 400 / 4;
            config.addRoadRandomRate = 100;
            entity = new int[config.width, config.height];
            (new Territory(ref lists, 0, 0, config.width - 1, config.height - 1)).split(ref lists, minTerritorySize);
            foreach (Territory r in lists.territory)
            {   // 区画が全てfixしてから全区画に対して部屋生成
                r.room = new Room(r.lx, r.ly, r.hx, r.hy, config.minRoomSize, config.marginRoomSize);
            }
            //            TerritoryToMap();
            AddRoad(config.addRoadRandomRate);
            RoomToWall(4);
            RoadToMap(3);
            RoomToFloor(2);
            ReplaceA();
            RoadToWall(3, 5);
            RoadToWall(5, 6);
            RoadToWall(4, 7);
        }
        void FillCheck(int x, int y, int icon)
        {
            if (entity[x, y] == 0) entity[x, y] = icon;
        }
        void RoadToWall(int roadicon, int wallicon)
        {
            int[,] tbl = new int[,] { { -1, -1 }, { 0, -1 }, { +1, -1 }, { -1, 0 }, { +1, 0 }, { -1, +1 }, { 0, +1 }, { +1, +1 } };
            for (int i = 1; i < entity.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < entity.GetLength(1) - 1; j++)
                {
                    if (entity[i, j] == roadicon)
                    {
                        for (int k = 0; k < tbl.GetLength(0); k++)
                        {
                            FillCheck(i + tbl[k, 0], j + tbl[k, 1], wallicon);
                        }
                    }
                }
            }
        }
        private void AddRoad(int randomMax)
        {
            Territory[,] refmap = new Territory[config.width, config.height];
            foreach (Territory r in lists.territory)
            {
                for (int i = r.lx; i < r.hx; i++)
                {
                    for (int j = r.ly; j < r.hy; j++)
                    {
                        refmap[i, j] = r;
                    }
                }
            }
            for (int i = 0; i < config.width - 2; i++)
            {
                for (int j = 0; j < config.height - 2; j++)
                {
                    if (refmap[i, j] != refmap[i, j + 1] && RandXorShift.Instance.Stage.Next(0, randomMax) == 0)
                    {
                        Territory r0 = refmap[i, j];
                        Territory r1 = refmap[i, j + 1];
                        bool active = true;
                        foreach (Road r in lists.road)
                        {
                            if (r.t0 == r0 || r.t0 == r1)
                            {
                                if (r.t1 == r0 || r.t1 == r1)
                                {
                                    active = false;
                                    break;
                                }
                            }
                        }
                        if (active)
                        {
                            new Road(ref lists, Direction.Veritical, r0, r1);
                        }
                    }
                    if (refmap[i, j] != refmap[i + 1, j] && RandXorShift.Instance.Stage.Next(0, randomMax) == 0)
                    {
                        Territory r0 = refmap[i, j];
                        Territory r1 = refmap[i + 1, j];
                        bool active = true;
                        foreach (Road r in lists.road)
                        {
                            if (r.t0 == r0 || r.t0 == r1 && r.t1 == r0 || r.t1 == r1)
                            {
                                active = false;
                                break;
                            }
                        }
                        if (active)
                        {
                            new Road(ref lists, Direction.Horizonal, r0, r1);
                        }
                    }
                }
            }
        }
        private void RoomToWall(int icon)
        {
            foreach (Territory r in lists.territory)
            {
                FillEntityHorizon(r.room.lx, r.room.ly, r.room.hx, icon);
                FillEntityHorizon(r.room.lx, r.room.hy, r.room.hx, icon);
                FillEntityVeritical(r.room.lx, r.room.ly, r.room.hy, icon);
                FillEntityVeritical(r.room.hx, r.room.ly, r.room.hy, icon);
            }
        }
        private void RoomToFloor(int icon)
        {
            foreach (Territory r in lists.territory)
            {
                for (int i = r.room.lx + 1; i < r.room.hx; i++)
                {
                    FillEntityVeritical(i, r.room.ly + 1, r.room.hy - 1, icon);
                }
            }
        }
        private void FillEntityVeritical(int x, int y, int h, int icon, bool check = false)
        {
            if (check && CheckBlankVeritical(x, y, h) == false) return;
            for (int i = y; i <= h; i++)
            {
                entity[x, i] = icon;
            }
        }
        private void FillEntityHorizon(int x, int y, int w, int icon, bool check = false)
        {
            if (check && CheckBlankHorizon(x, y, w) == false) return;
            for (int i = x; i <= w; i++)
            {
                entity[i, y] = icon;
            }
        }
        private bool CheckBlankVeritical(int x, int y, int h)
        {
            for (int i = y; i <= h; i++)
            {
                if (entity[x, i] != 0) return false;
            }
            return true;
        }
        private bool CheckBlankHorizon(int x, int y, int w)
        {
            for (int i = x; i <= w; i++)
            {
                if (entity[i, y] != 0) return false;
            }
            return true;
        }

        private void LineToMap(int x0, int y0, int x1, int y1, int icon)
        {
            int min_x, max_x, min_y, max_y;
            min_x = Math.Min(x0, x1);
            max_x = Math.Max(x0, x1);
            min_y = Math.Min(y0, y1);
            max_y = Math.Max(y0, y1);
            if ((x0 <= x1) && (y0 >= y1))   // [↗]左下から右上に
            {
                FillEntityHorizon(min_x, max_y, max_x, icon);       //    |
                FillEntityVeritical(max_x, min_y, max_y, icon);     // ---+
                return;
            };
            if ((x0 > x1) && (y0 > y1))     // [↖]右下から左上に
            {
                FillEntityHorizon(min_x, min_y, max_x, icon);       // ---+
                FillEntityVeritical(max_x, min_y, max_y, icon);     //    |
                return;
            };
            if ((x0 > x1) && (y0 <= y1))    // [↙]右上から左下に
            {
                FillEntityHorizon(min_x, min_y, max_x, icon);       // +---
                FillEntityVeritical(min_x, min_y, max_y, icon);     // |
                return;
            };
            if ((x0 <= x1) && (y0 < y1))   // [↘]左上から右下に
            {
                FillEntityHorizon(min_x, max_y, max_x, icon);       // |
                FillEntityVeritical(min_x, min_y, max_y, icon);     // +---
                return;
            };
        }
        private void RoadToMap(int icon)
        {
            int c0x, c0y, c1x, c1y;
            foreach (Road c in lists.road)
            {
                switch (c.direction)
                {
                    case Direction.Horizonal:
                        c0x = c.t0.hx;
                        c0y = RandXorShift.Instance.Stage.Next(c.t0.room.ly + 2, c.t0.room.hy - 2);
                        c1x = c.t1.lx;
                        c1y = RandXorShift.Instance.Stage.Next(c.t1.room.ly + 2, c.t1.room.hy - 2);
                        LineToMap(c0x, c0y, c1x, c1y, icon);
                        LineToMap(c.t0.room.hx, c0y, c0x, c0y, icon);
                        LineToMap(c.t1.room.lx, c1y, c1x, c1y, icon);
                        break;
                    case Direction.Veritical:
                        c0x = RandXorShift.Instance.Stage.Next(c.t0.room.lx + 2, c.t0.room.hx - 2); // 接続する部屋.1までのX座標(random)
                        c0y = c.t0.hy;                                                             // 接続領域.1のY座標を固定化
                        c1x = RandXorShift.Instance.Stage.Next(c.t1.room.lx + 2, c.t1.room.hx - 2); // 接続する部屋.2までのX座標(random)
                        c1y = c.t1.ly;                                                             // 接続領域.2のY座標を固定化
                        LineToMap(c0x, c0y, c1x, c1y, icon);                  // 境界同士の接続
                        LineToMap(c0x, c.t0.room.hy, c0x, c0y, icon);      // 境界から領域内の部屋.0への接続
                        LineToMap(c1x, c.t1.room.ly, c1x, c1y, icon);      // 境界から領域内の部屋.1への接続
                        break;
                }
            }
        }
        void TerritoryToMap()
        {
            int i, j;
            foreach (Territory r in lists.territory)
            {
                for (i = r.lx, j = r.ly; i <= r.hx; i++) entity[i, j] = 1;
                for (i = r.lx, j = r.hy; i <= r.hx; i++) entity[i, j] = 1;
                for (i = r.lx, j = r.ly; j <= r.hy; j++) entity[i, j] = 1;
                for (i = r.hx, j = r.ly; j <= r.hy; j++) entity[i, j] = 1;
            }
        }
        public class Room
        {
            public int lx, ly, hx, hy;
            public Room(int sx, int sy, int ex, int ey, byte minRoomSize, byte marginRoomSize)
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
        };
        public class Territory
        {
            private bool done_split_v;
            private bool done_split_h;
            public int lx, ly, hx, hy;
            public Room room;
            public Territory(ref Lists lists, int x1, int y1, int x2, int y2)
            {
                lx = x1;
                ly = y1;
                hx = x2;
                hy = y2;
                lists.territory.Add(this);
            }
            public void split(ref Lists lists, ushort minTerritorySize)
            {
                // Clip Check.
                if ((this.hy - this.ly) <= (minTerritorySize * 2)) this.done_split_v = true;
                if ((this.hx - this.lx) <= (minTerritorySize * 2)) this.done_split_h = true;
                if ((this.done_split_v) && (this.done_split_h)) return;

                Territory child = new Territory(ref lists, this.lx, this.ly, this.hx, this.hy);

                // どちらかしか同時に行わない
                if (this.done_split_v == false)
                {
                    int split_coord_y;
                    split_coord_y = RandXorShift.Instance.Stage.Next(this.ly + minTerritorySize, this.hy - minTerritorySize);
                    this.hy = split_coord_y;
                    child.ly = split_coord_y;
                    if (!((this.hy - this.ly) >= (minTerritorySize * 3)
                    && (child.hy - child.ly) >= (minTerritorySize * 3)))
                    {
                        this.done_split_v = true;
                        child.done_split_v = true;
                    }
                    new Road(ref lists, Direction.Veritical, this, child);
                }
                else
                {
                    int split_coord_x;
                    split_coord_x = RandXorShift.Instance.Stage.Next(this.lx + minTerritorySize, this.hx - minTerritorySize);
                    this.hx = split_coord_x;
                    child.lx = split_coord_x;
                    if (!((this.hx - this.lx) >= (minTerritorySize * 3)
                    && (child.hx - child.lx) >= (minTerritorySize * 3)))
                    {
                        this.done_split_h = true;
                        child.done_split_h = true;
                    }
                    new Road(ref lists, Direction.Horizonal, this, child);
                }
                this.split(ref lists, minTerritorySize);
                child.split(ref lists, minTerritorySize);
            }
        };
        public class Road
        {
            public Direction direction;
            public Territory t0;
            public Territory t1;
            public Road(ref Lists lists, Direction vh, Territory r0, Territory r1)
            {
                direction = vh;
                t0 = r0;
                t1 = r1;
                lists.road.Add(this);
            }
        };
    }
}
