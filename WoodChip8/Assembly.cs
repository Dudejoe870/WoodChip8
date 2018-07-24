using System;
using System.Collections.Generic;
using System.Text;

namespace WoodChip8.Assembly
{
    public static class Disassembler
    {
        public static string GetASM(short Opcode)
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
                        case 0x00E0: return "CLS";
                        case 0x00EE: return "RET";
                    }
                    break;
                case 0x1000: return String.Format("JP 0x{0:X}", nnn);
                case 0x2000: return String.Format("CALL 0x{0:X}", nnn);
                case 0x3000: return String.Format("SE V{0:X}, 0x{1:X}", x, nn);
                case 0x4000: return String.Format("SNE V{0:X}, 0x{1:X}", x, nn);
                case 0x5000: return String.Format("SE V{0:X}, V{1:X}", x, y);
                case 0x6000: return String.Format("LD V{0:X}, 0x{1:X}", x, nn);
                case 0x7000: return String.Format("ADD V{0:X}, 0x{1:X}", x, nn);
                case 0x8000:
                    switch (n)
                    {
                        case 0x0000: return String.Format("LD V{0:X}, V{1:X}", x, y);
                        case 0x0001: return String.Format("OR V{0:X}, V{1:X}", x, y);
                        case 0x0002: return String.Format("AND V{0:X}, V{1:X}", x, y);
                        case 0x0003: return String.Format("XOR V{0:X}, V{1:X}", x, y);
                        case 0x0004: return String.Format("ADD V{0:X}, V{1:X}", x, y);
                        case 0x0005: return String.Format("SUB V{0:X}, V{1:X}", x, y);
                        case 0x0006: return String.Format("SHR V{0:X}, V{1:X}", x, y);
                        case 0x0007: return String.Format("SUBN V{0:X}, V{1:X}", x, y);
                        case 0x000E: return String.Format("SHL V{0:X}, V{1:X}", x, y);
                    }
                    break;
                case 0x9000: return String.Format("SNE V{0:X}, V{1:X}", x, y);
                case 0xA000: return String.Format("LD I, 0x{0:X}", nnn);
                case 0xB000: return String.Format("JP V0, 0x{0:X}", nnn);
                case 0xC000: return String.Format("RND V{0:X}, 0x{1:X}", x, nn);
                case 0xD000: return String.Format("RND V{0:X}, V{1:X}, 0x{2:X}", x, y, n);
                case 0xE000:
                    switch (nn)
                    {
                        case 0x009E: return String.Format("SKP V{0:X}", x);
                        case 0x00A1: return String.Format("SKNP V{0:X}", x);
                    }
                    break;
                case 0xF000:
                    switch (nn) 
                    {
                        case 0x0007: return String.Format("LD V{0:X}, DT", x);
                        case 0x000A: return String.Format("LD V{0:X}, K", x);
                        case 0x0015: return String.Format("LD DT, V{0:X}", x);
                        case 0x0018: return String.Format("LD ST, V{0:X}", x);
                        case 0x001E: return String.Format("ADD I, V{0:X}", x);
                        case 0x0029: return String.Format("LD F, V{0:X}", x);
                        case 0x0033: return String.Format("LD B, V{0:X}", x);
                        case 0x0055: return String.Format("LD [I], V{0:X}", x);
                        case 0x0065: return String.Format("LD V{0:X}, [I]", x);
                    }
                    break;

            }

            throw new NotImplementedException(String.Format("Unimplemented / Invalid Opcode (0x{0:X}) to Dissassemble", Opcode));
        }
    }

    public static class Assembler
    {
        private static short StringExprToShort(string expr)
        {
            if (expr.StartsWith("0x"))
            {
                return short.Parse(expr.Substring(2), System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                return short.Parse(expr);
            }
        }

        private static byte StringExprToByte(string expr)
        {
            if (expr.StartsWith("0x"))
            {
                return byte.Parse(expr.Substring(2), System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                return byte.Parse(expr);
            }
        }

        private static byte GetRegisterFromStringExpr(string expr)
        {
            if (expr.ToUpper()[0] == 'V' && expr.Length == 2)
            {
                return Byte.Parse(""+expr[1], System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                throw new ArgumentException(String.Format("Register ({0}) is not a valid Register", expr));
            }
        }

        // WIP
        public static short GetOpcode(string ASM)
        {
            string[] SplitInst = ASM.Split(" ");
            string Inst = SplitInst[0];
            string ArgStr = "";
            string[] Args = null;

            int i = 0;
            foreach (string s in SplitInst) 
            {
                if (i > 0)
                {
                    ArgStr += s;
                }

                ++i;
            }

            if (ArgStr.Contains(',')) Args = ArgStr.Split(',');
            else if (SplitInst.Length == 2)
            {
                Args = new string[1];
                Args[0] = SplitInst[1];
            }

            try
            {
                switch (Inst)
                {
                    case "CLS":  return 0x00E0;
                    case "RET":  return 0x00EE;
                    case "JP":   return (short)(0x1000 | StringExprToShort(Args[0]));
                    case "CALL": return (short)(0x2000 | StringExprToShort(Args[0]));
                    case "SE":   return (short)((0x3000 | (GetRegisterFromStringExpr(Args[0]) << 12)) | StringExprToByte(Args[1]));
                    case "SNE":  return (short)((0x4000 | (GetRegisterFromStringExpr(Args[0]) << 12)) | StringExprToByte(Args[1]));
                }
            }
            catch
            {
                throw new ArgumentException("Not enough Parameters.");
            }

            throw new NotImplementedException(String.Format("Unimplemented / Invalid Assembly ({0}) to Assemble", ASM));
        }
    }
}
