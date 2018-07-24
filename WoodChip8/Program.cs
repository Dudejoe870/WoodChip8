using System;

namespace WoodChip8
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            new MainWindow().Run(60, 60);
        }
    }
}
