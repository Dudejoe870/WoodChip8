using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WoodChip8
{
    class Interpreter
    {
        private byte[]   V;
        private byte     I;
        private byte     DT;
        private byte     ST;
        private short    PC;
        private short    SP;
        private short[]  Stack;
        private byte[]   RAM;
        private bool[,] Display;
        private int Disp_Width;
        private int Disp_Height;

        public Interpreter()
        {
            V = new byte[0xF];
            I = 0;
            DT = 0;
            ST = 0;
            PC = 0x200;
            SP = 0;
            Stack = new short[16];
            RAM = new byte[4096];
            Disp_Width = 64;
            Disp_Height = 32;
            Display = new bool[Disp_Width, Disp_Height];
        }

        public void LoadProgram(string path)
        {
            byte[] Rom = File.ReadAllBytes(path);
            for(int i = 0; i < Rom.Length; ++i) RAM[0x200 + i] = Rom[i];
        }

        public void StartInterpreter(double UpdateFreq = 2.3181818181818256 /* Very roughly how long the COSMAC Vip would take in one cycle of Chip-8 */)
        {
            System.Timers.Timer CPU = new System.Timers.Timer()
            {
                Interval = UpdateFreq
            };
            CPU.Elapsed += new System.Timers.ElapsedEventHandler(UpdateCPU);
            CPU.Enabled = true;

            System.Timers.Timer CPUTimers = new System.Timers.Timer()
            {
                Interval = 16.666666666667 // 60 Hz
            };
            CPU.Elapsed += new System.Timers.ElapsedEventHandler(UpdateTimers);
            CPU.Enabled = true;
        }

        public void Step(short Opcode)
        {
            short nnn = (short)((Opcode & 0x0FFF));
            byte  nn  = (byte) ((Opcode & 0x00FF));
            byte  n   = (byte) ((Opcode & 0x000F));
            byte  x   = (byte) ((Opcode & 0x0F00) >> 8);
            byte  y   = (byte) ((Opcode & 0x00F0) >> 4);

            switch (Opcode & 0xF000)
            {
                case 0x0000:
                    switch (nn)
                    {
                        case 0x00E0: // CLS
                            for (int col = 0; col < Disp_Height; ++col)
                                for (int row = 0; row < Disp_Width; ++row)
                                    Display[row, col] = false;
                            break;
                        case 0x00EE: // RET
                            PC = Stack[SP];
                            --SP;
                            break;
                    }
                    break;
                case 0x1000: // JP 0x(nnn)
                    PC = nnn;
                    break;
                case 0x2000: // CALL 0x(nnn)
                    ++SP;
                    Stack[SP] = PC;
                    PC = nnn;
                    break;
                case 0x3000: // SE V(x), 0x(nn)
                    if (V[x] == nn) PC += 2;
                    break;
                case 0x4000: // SNE V(x), 0x(nn)
                    if (V[x] != nn) PC += 2;
                    break;
            }
        }

        private void UpdateCPU(object sender, System.Timers.ElapsedEventArgs e)
        {
            short Opcode = BitConverter.ToInt16(new byte[2] { RAM[PC+1], RAM[PC]}, 0);
            Step(Opcode);
            PC += 2;
        }

        private void UpdateTimers(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (DT > 0) --DT;
            if (ST > 0) --ST;
            if (ST != 0)
            {
                // TODO: Add Sound Output
            }
        }
    }
}
