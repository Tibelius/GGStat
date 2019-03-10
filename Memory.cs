using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GGStat
{
    class Memory
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess,
        long lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        private readonly int PROCESS_WM_READ = 0x0010;
        private int BASEADDRESS = Program.process.MainModule.BaseAddress.ToInt32();
        private IntPtr processHandle;

        private readonly int[] gameStateOffsets = { 0x009C0DEC, 0x0 };
        private readonly int[] timerOffsets = { 0x0177A8AC, 0x450, 0x4C, 0x870, 0x4C, 0x1C, 0x4, 0x14, 0x28, 0x708 };
        private readonly int[] player1RoundWinsOffsets = { 0x009B3C44, 0x0 };
        private readonly int[] player2RoundWinsOffsets = { 0x009B3C44, 0x4 };
        private readonly int[] player1HP = { 0x01B18C78, 0x9CC };
        private readonly int[] player2HP = { 0x01B18C7C, 0x9CC };

        private readonly Dictionary<string, int[]> dataList;

        //int[] timerOffsets = { 0x0167552C, 0x50, 0xAD4, 0x708 };
        public Memory() {
            processHandle = OpenProcess(PROCESS_WM_READ, false, Program.process.Id);
            dataList = new Dictionary<string, int[]> {
                {"gameState", gameStateOffsets},
                {"timer", timerOffsets},
                {"player1Rounds", player1RoundWinsOffsets},
                {"player2Rounds", player2RoundWinsOffsets},
                {"player1HP", player1HP},
                {"player2HP", player2HP},
            };
        }

        public void ScanMemory(ref Dictionary<string, int> valueList) {
            int bytesRead = 0;
            byte[] buffer = new byte[4];

            foreach (KeyValuePair<string, int[]> item in dataList) {
                int value = BASEADDRESS;
                foreach (int offset in item.Value) {
                    long address = value + offset;
                    ReadProcessMemory((int)processHandle, address, buffer, buffer.Length, ref bytesRead);
                    value = BitConverter.ToInt32(buffer, 0);
                }
                valueList[item.Key] = value;
            }
        }

        public int ScanAddress(int addr) {
            int bytesRead = 0;
            byte[] buffer = new byte[4];

            ReadProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesRead);
            return BitConverter.ToInt32(buffer, 0);
        }
    }
}
