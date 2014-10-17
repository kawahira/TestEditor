using System;
using System.Collections.Generic;

namespace GenerateMap
{
    public class Lists
    {
        public List<Territory> territory = new List<Territory>();
        public List<Road> road = new List<Road>();
        public void BuildWall(ref Mapchip mapchip, int icon)
        {
            foreach (Territory r in territory)
            {
                mapchip.Line(r.room.lx, r.room.ly, r.room.hx, r.room.ly, icon);
                mapchip.Line(r.room.lx, r.room.hy, r.room.hx, r.room.hy, icon);
                mapchip.Line(r.room.lx, r.room.ly, r.room.lx, r.room.hy, icon);
                mapchip.Line(r.room.hx, r.room.ly, r.room.hx, r.room.hy, icon);
            }
        }
        public void BuildFloor(ref Mapchip mapchip, int icon)
        {
            foreach (Territory r in territory)
            {
                mapchip.Fill(r.room.lx + 1, r.room.ly + 1, r.room.hx - 1, r.room.hy - 1, icon);
            }
        }
        public void BuildRoad(ref Mapchip mapchip, int icon)
        {
            foreach (Road r in road)
            {
                r.Build(ref mapchip, icon);
            }
        }
    };

    public class Generator
    {
        public Lists lists = new Lists();
        public Mapchip mapchip;
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
        public ushort minTerritorySize
        {
            get
            {
                return (ushort)(config.minRoomSize + (config.marginRoomSize * 2));
            }
        }

        public Generator()
        {
            config.minRoomSize = 8;
            config.marginRoomSize = 1;
            config.height = 400 / 4;
            config.width = 400 / 4;
            config.addRoadRandomRate = 0;
            entity = new int[config.width, config.height];
            mapchip = new Mapchip(config.width, config.height);
            (new Territory(ref lists, 0, 0, config.width - 1, config.height - 1)).split(ref lists, minTerritorySize);
            foreach (Territory r in lists.territory)
            {   // 区画が全てfixしてから全区画に対して部屋生成
                r.room = new Room(r.lx, r.ly, r.hx, r.hy, config.minRoomSize, config.marginRoomSize);
            }
//            TerritoryToMap(1);
            AddRoad(config.addRoadRandomRate);
            RoomToWall(4);
            lists.BuildWall(ref mapchip, 4);
//          RoadToMap(3);
//          lists.BuildRoad(ref mapchip, 3);
            RoomToFloor(2);
            lists.BuildFloor(ref mapchip, 2);
//                (new Replace()).Build(config.width, config.height, ref mapchip);
            /*
                        RoadToWall(3, 5);
                        RoadToWall(5, 6);
                        RoadToWall(4, 7);
                        mapchip.AroundReplace(3, 5);
                        mapchip.AroundReplace(5, 6);
                        mapchip.AroundReplace(4, 7);
            */
            for (int i = 1; i < entity.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < entity.GetLength(1) - 1; j++)
                {
                    if (entity[i, j] != mapchip.entity[i, j])
                    {
                        break;
                    }
                }
            }
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
        struct tempResult
        {
            public Territory t0;
            public Territory t1;
            public int direction;
            public tempResult(Territory te1, Territory te2, int dir)
            {
                t0 = te1;
                t1 = te2;
                direction = dir;
            }
        }
        private void AddRoad(ushort createMax)
        {
            if ( createMax == 0 ) return;
            Territory[,] refmap = new Territory[config.width, config.height];
            List<tempResult> result = new List<tempResult>();
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
            int[,] tbl = new int[,] { { 0, -1 }, { 0, -1 }, { +1, -1 }, { -1, 0 }, { +1, 0 }, { -1, +1 }, { 0, +1 }, { +1, +1 } };
            for (int i = 0; i < config.width - 2; i++)
            {
                for (int j = 0; j < config.height - 2; j++)
                {
                    // 下方向に対する検索
                    Territory r0 = refmap[i, j];        // 基準
                    Territory r1 = refmap[i, j + 1];    // 比較（下側)
                    Territory r2 = refmap[i + 1, j];    // 比較 (右側)
                    for (int k = 0; k < 2; k++)
                    {
                        if (r0 != r1)
                        {
                            bool active = true;
                            // すでに存在している道に同じ経路があれば登録をしない
                            foreach (Road r in lists.road)
                            {
                                if ((r.t0 == r0 || r.t0 == r1) && (r.t1 == r0 || r.t1 == r1))
                                {
                                    active = false;
                                    break;
                                }
                            }
                            // 新規として一度登録したResultに同じ場所への道があれば、登録をしない
                            if (active)
                            {
                                foreach (tempResult t in result)
                                {
                                    if ((t.t0 == r0 || t.t0 == r1) && (t.t1 == r0 || t.t1 == r1))
                                    {
                                        active = false;
                                        break;
                                    }
                                }
                            }
                            if (active)
                            {
                                result.Add(new tempResult(r0, r1, k));
                            }
                        }
                        r1 = r2;    // 横方向に対する検索に対象を切り替え
                    }
                }
            }
            {
                int max = Math.Min(createMax,result.Count);
                for (int i = 0; i < max; i ++ )
                {
                    int idx = RandXorShift.Instance.Stage.Next(0, result.Count);
                    new Road(ref lists, result[idx].t0, result[idx].t1, result[idx].direction == 0 ? Road.Direction.Veritical : Road.Direction.Horizonal);
                    result.RemoveAt(idx);
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
            // H:横
            // V:縦
            if ((x0 <= x1) && (y0 >= y1))   // [↗]左下から右上に
            {
                mapchip.Line(min_x, max_y, max_x, max_y, icon);      //   |
                mapchip.Line(max_x, min_y, max_x, max_y, icon);     // ---+

                FillEntityHorizon(min_x, max_y, max_x, icon);       //    |
                FillEntityVeritical(max_x, min_y, max_y, icon);     // ---+
                return;
            };
            if ((x0 > x1) && (y0 > y1))     // [↖]右下から左上に
            {
                mapchip.Line(min_x, min_y, max_x, min_y, icon);     // ---+
                mapchip.Line(max_x, min_y, max_x, max_y, icon);     //    |

                FillEntityHorizon(  min_x, min_y, max_x, icon);       // ---+
                FillEntityVeritical(max_x, min_y, max_y, icon);     //    |
                return;
            };
            if ((x0 > x1) && (y0 <= y1))    // [↙]右上から左下に
            {
                mapchip.Line(min_x, min_y, max_x, min_y, icon);     // +---
                mapchip.Line(min_x, min_y, min_x, max_y, icon);     // |

                FillEntityHorizon(min_x, min_y, max_x, icon);       // +---
                FillEntityVeritical(min_x, min_y, max_y, icon);     // |
                return;
            };
            if ((x0 <= x1) && (y0 < y1))   // [↘]左上から右下に
            {
                mapchip.Line(min_x, max_y, max_x, max_y, icon);     // |
                mapchip.Line(min_x, min_y, min_x, max_y, icon);     // +---

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
                    case Road.Direction.Horizonal:
                        c0x = c.t0.hx;
                        c0y = RandXorShift.Instance.Stage.Next(c.t0.room.ly + 2, c.t0.room.hy - 2);
                        c1x = c.t1.lx;
                        c1y = RandXorShift.Instance.Stage.Next(c.t1.room.ly + 2, c.t1.room.hy - 2);
                        LineToMap(c0x, c0y, c1x, c1y, icon);
                        LineToMap(c.t0.room.hx, c0y, c0x, c0y, icon);
                        LineToMap(c.t1.room.lx, c1y, c1x, c1y, icon);
                        break;
                    case Road.Direction.Veritical:
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
        void TerritoryToMap(int icon)
        {
            int i, j;
            foreach (Territory r in lists.territory)
            {
                for (i = r.lx, j = r.ly; i <= r.hx; i++) entity[i, j] = icon;
                for (i = r.lx, j = r.hy; i <= r.hx; i++) entity[i, j] = icon;
                for (i = r.lx, j = r.ly; j <= r.hy; j++) entity[i, j] = icon;
                for (i = r.hx, j = r.ly; j <= r.hy; j++) entity[i, j] = icon;
            }
        }
    }
}
