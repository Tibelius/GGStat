using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static GGStat.Game;

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
        //private readonly int[] ownerPositionOffsets = { 0x01C24F90, 0xCC, 0x40, 0x68, 0x42C, 0x6BC };
        //private readonly int[] ownerCharacterOffsets = { 0x1AC87B4 };
        private readonly int[] player1RoundWinsOffsets = { 0x009B3C44, 0x0 };
        private readonly int[] player2RoundWinsOffsets = { 0x009B3C44, 0x4 };
        private readonly int[] player1HP = { 0x01B18C78, 0x9CC };
        private readonly int[] player2HP = { 0x01B18C7C, 0x9CC };

        private readonly int[] lobbyP1CabinetOffsets = { 0x0199BB18, 0x4, 0xC, 0x8, 0x10, 0x718 };
        private readonly int[] lobbyP1PositionOffsets = { 0x0199BB18, 0x4, 0xC, 0x8, 0x10, 0x71C };
        private readonly int[] lobbyP1NameOffsets = { 0x0199BB18, 0x4, 0xC, 0x8, 0x10, 0x4C0 };
        private readonly int[] lobbyP2CabinetOffsets = { 0x0199BB18, 0x4, 0xC, 0xC, 0x10, 0x718 };
        private readonly int[] lobbyP2PositionOffsets = { 0x0199BB18, 0x4, 0xC, 0xC, 0x10, 0x71C };
        private readonly int[] lobbyP2NameOffsets = { 0x0199BB18, 0x4, 0xC, 0xC, 0x10, 0x4C0 };

        private readonly Dictionary<string, int[]> dataList;
        private readonly Dictionary<string, int[]> nameList;

        public Memory() {
            processHandle = OpenProcess(PROCESS_WM_READ, false, Program.process.Id);
            dataList = new Dictionary<string, int[]> {
                {"gameState", gameStateOffsets},
                {"timer", timerOffsets},
                {"lobbyP1Position", lobbyP1PositionOffsets},
                {"lobbyP1Cabinet", lobbyP1CabinetOffsets},
                {"lobbyP2Position", lobbyP2PositionOffsets},
                {"lobbyP2Cabinet", lobbyP2CabinetOffsets},
                //{"ownerCharacter", ownerCharacterOffsets},
                {"player1Rounds", player1RoundWinsOffsets},
                {"player2Rounds", player2RoundWinsOffsets},
                {"player1HP", player1HP},
                {"player2HP", player2HP},
            };

            nameList = new Dictionary<string, int[]> {
                {"lobbyP1Name", lobbyP1NameOffsets},
                {"lobbyP2Name", lobbyP2NameOffsets},
            };
        }

        public long getClientSteamId() {
            try {
                return BitConverter.ToInt64(ScanAddress(new long[] { 0x1AD82E4L }, 8), 0);
            } catch (Exception) {
                return -1;
            }
        }

        public List<Player> getPlayerData() {
            if (!Program.isConnected()) return new List<Player>();
            long[] offs = { 0x1C25AB4L, 0x44CL };
            List<Player> playerList = new List<Player>();
            for (int i = 0; i < 8; i++)
            {
                byte[] byteArr = ScanAddress(offs, 0x48);
                if (byteArr == null) return new List<Player>();
                long steamId = BitConverter.ToInt64(byteArr, 0);
                int characterId = byteArr[0x36];
                int matchesWon = byteArr[0xA];
                int matchesSum = byteArr[8];
                int loadingPct = byteArr[0x44];
                int cabinetId = byteArr[0x38];
                int seatingId = byteArr[0x39];
                byteArr = byteArr.Skip(11).Take(36).ToArray();
                string displayName = Encoding.UTF8.GetString(byteArr, 0, byteArr.Length).Trim('\u0000');
                Player player = new Player(
                    steamId,
                    displayName,
                    characterId,
                    matchesWon,
                    matchesSum,
                    loadingPct,
                    cabinetId,
                    seatingId
                );
                offs[1] += 0x48L;
                playerList.Add(player);
            }
            return playerList;
        }

        public MatchState getMatchState() {
            long[] sortedStructOffs = { 0x9CCL, 0x2888L, 0xA0F4L, 0x22960, 0x2AC64, 0x7AF4, 0x7AF8 };
            long[] p1offs = { 0x1B18C78L, 0L };
            long[] p2offs = { 0x1B18C78L, 0L };
            p2offs[0] += 4L;
            long[] p1roundoffset = { 0x1A3BA38L };
            long[] p2roundoffset = { 0x1A3BA3CL };
            long[] timeroffs = { 0x177A8ACL, 0x450L, 0x4CL, 0x708L };
            long[] gameStateOffsets = { 0x009C0DECL, 0L };
            try {
                p1offs[1] = sortedStructOffs[0];
                p2offs[1] = sortedStructOffs[0];
                int[] health = new int[] { BitConverter.ToInt32(ScanAddress(p1offs, 4), 0), BitConverter.ToInt32(ScanAddress(p2offs, 4), 0) };
                p1offs[1] = sortedStructOffs[1];
                p2offs[1] = sortedStructOffs[1];
                bool[] canBurst = new bool[] { BitConverter.ToInt32(ScanAddress(p1offs, 4), 0) == 1, BitConverter.ToInt32(ScanAddress(p2offs, 4), 0) == 1 };
                p1offs[1] = sortedStructOffs[3];
                p2offs[1] = sortedStructOffs[3];
                int[] risc = new int[] { BitConverter.ToInt32(ScanAddress(p1offs, 4), 0), BitConverter.ToInt32(ScanAddress(p2offs, 4), 0) };
                p1offs[1] = sortedStructOffs[4];
                p2offs[1] = sortedStructOffs[4];
                int[] tension = new int[] { BitConverter.ToInt32(ScanAddress(p1offs, 4), 0), BitConverter.ToInt32(ScanAddress(p2offs, 4), 0) };
                p1offs[1] = sortedStructOffs[5];
                p2offs[1] = sortedStructOffs[5];
                int[] stun = new int[] { BitConverter.ToInt32(ScanAddress(p1offs, 4), 0), BitConverter.ToInt32(ScanAddress(p2offs, 4), 0) };

                int timer = BitConverter.ToInt32(ScanAddress(timeroffs, 4), 0);
                int state = BitConverter.ToInt32(ScanAddress(gameStateOffsets, 4), 0);
                int[] rounds = new int[] { BitConverter.ToInt32(ScanAddress(p1roundoffset, 4), 0), BitConverter.ToInt32(ScanAddress(p2roundoffset, 4), 0) };
                return new MatchState(timer, health, rounds, tension, canBurst, risc, stun, state);
            }  catch (Exception e) {
                Console.WriteLine(e.Message);
                return new MatchState();
            }
        }


        public void ScanMemory(ref Dictionary<string, int> valueList, ref Dictionary<string, string> playerNameList) {
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

            int nameLength = 40;
            buffer = new byte[nameLength];
            foreach (KeyValuePair<string, int[]> item in nameList)
            {
                int value = BASEADDRESS;
                long address;
                foreach (int offset in item.Value)
                {
                    address = value + offset;
                    ReadProcessMemory((int)processHandle, address, buffer, buffer.Length, ref bytesRead);
                    value = BitConverter.ToInt32(buffer, 0);
                }
                //Console.WriteLine(BitConverter.ToString(buffer));
                string name = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                name = string.Join<char>("", name.Where((ch, index) => (index % 2) == 0));
                //Console.WriteLine(name);
                playerNameList[item.Key] = name;
            }

        }

        public byte[] ScanAddress(long[] offsets, int numBytes)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[numBytes];
            long value = BASEADDRESS;
            foreach (long offset in offsets)
            {
                long address = value + offset;
                ReadProcessMemory((int)processHandle, address, buffer, buffer.Length, ref bytesRead);
                value = BitConverter.ToInt32(buffer, 0);
            }
            return buffer;
        }
    }
}
