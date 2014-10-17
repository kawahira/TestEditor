using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WindowsFormsApplication
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RandXorShift.Instance.Seed(1);
            GenerateMap.Config config = new GenerateMap.Config();

            config.minRoomSize      = 8;
            config.marginRoomSize   = 3;
            config.height           = 400 / 4;
            config.width            = 400 / 4;
            config.addRoadMax       = 100;

            Form1 f = new Form1();
            f.map = new GenerateMap.Generator(config);

            Application.Run(f);

        }
    }
}
