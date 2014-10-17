using System;
using System.Collections.Generic;

namespace GenerateMap
{
    public class Generator
    {
        public List<Territory> territory = new List<Territory>();
        public List<Road> road = new List<Road>();
        public Mapchip mapchip;
        private Config config;
        public ushort minTerritorySize
        {
            get
            {
                return (ushort)(config.minRoomSize + (config.marginRoomSize * 2));
            }
        }
        public Config GetConfig() { return config;  }
        public Generator(Config cfg)
        {
            config = cfg;
            mapchip = new Mapchip(config.width, config.height);
            (new Territory(ref territory, 0, 0, config.width - 1, config.height - 1)).split(ref territory,ref road, minTerritorySize);
            //            TerritoryToMap(1);
            BuildRoom(config.minRoomSize, config.marginRoomSize);
            AddRoad(config.addRoadMax);

            DrawWall(4);
            DrawRoad(3);
            DrawFloor(2);
            (new Replace()).Build(config.width, config.height, ref mapchip);
            mapchip.AroundReplace(3, 5);
            mapchip.AroundReplace(5, 6);
            mapchip.AroundReplace(4, 7);
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
            if (createMax == 0) return;
            Territory[,] refmap = new Territory[config.width, config.height];
            List<tempResult> result = new List<tempResult>();
            foreach (Territory r in territory)
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
                            foreach (Road r in road)
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
            int max = Math.Min(createMax, result.Count);
            for (int i = 0; i < max; i++)
            {
                int idx = RandXorShift.Instance.Stage.Next(0, result.Count);
                new Road(ref road, result[idx].t0, result[idx].t1, result[idx].direction == 0 ? Road.Direction.Veritical : Road.Direction.Horizonal);
                result.RemoveAt(idx);
            }
        }
        public void DrawWall(int icon)
        {
            foreach (Territory r in territory)
            {
                mapchip.Line(r.room.lx, r.room.ly, r.room.hx, r.room.ly, icon);
                mapchip.Line(r.room.lx, r.room.hy, r.room.hx, r.room.hy, icon);
                mapchip.Line(r.room.lx, r.room.ly, r.room.lx, r.room.hy, icon);
                mapchip.Line(r.room.hx, r.room.ly, r.room.hx, r.room.hy, icon);
            }
        }
        public void DrawFloor(int icon)
        {
            foreach (Territory r in territory)
            {
                mapchip.Fill(r.room.lx + 1, r.room.ly + 1, r.room.hx - 1, r.room.hy - 1, icon);
            }
        }
        public void DrawRoad(int icon)
        {
            foreach (Road r in road)
            {
                r.Build(ref mapchip, icon);
            }
        }
        public void BuildRoom(byte minRoomSize, byte marginRoomSize)
        {
            foreach (Territory r in territory)
            {
                r.room = new Room(r.lx, r.ly, r.hx, r.hy, minRoomSize, marginRoomSize);
            }
        }
        /*        
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
         */
    }
}
