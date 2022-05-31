using System;
using System.Collections.Generic;
using System.Text;

    // Пакет известного формата (заголовок + тело): 
    // - заголовок:
    // -------------------------------------------------------------------------
    // 1 байт   |        2 байта        |    2 байта     |    2 байта     | ...
    // -------------------------------------------------------------------------
    // Тип пакета | Кол-во строк в пакете | длина строки 1 | длина строки 2 | ...
    // -------------------------------------------------------------------------
    // - тело:
    // ---------------------
    // Строка_1 | Строка_2...

    // Ключевые методы:
    //  - обращение в массив байт     byte[] ToBytes()
    //  - инциализация массивом байт  void FromBytes(byte[] abIn, ...)
    //                                static Packet ParseBytes(byte[] bytes, ...)
    //  - возврат типа                PacketType Type
    //  - возврат поля/строки         string GetItem(int i)

    enum PacketType { SimpleMessage = 1, ClientList = 2, Login = 3 }

    class Packet {
        private PacketType pt;

        private ushort[] anLen;
        private string[] asData;
        private byte[] abBuffer; // буфер для конвертации пакета в массив байт

        public Packet(PacketType _pt = PacketType.SimpleMessage, int nLen = 2) {
            pt = _pt;
            switch (pt) {
                case PacketType.SimpleMessage: nLen = 2; break;
                case PacketType.Login: nLen = 1; break;
            }
            if (nLen> 0) asData = new string[nLen];
        }

        public PacketType Type {
            get { return pt; }
            set { pt = value; }
        }

        public int ItemCount {
            get { return (asData != null) ? asData.Length : 0; }
            set { 
                if (value <= 0) return;
                if (ItemCount == value) return;
                asData = new string[value];
            }
        }
        
        public int HeaderSize {   // Размер заголовка
            get {
                // тип пакета 1б + кол-во строк 2б
                int ret = 1 + 2;
                // длины строк
                ret += 2 * ItemCount;
                return ret;
            }
        }

        public int Size {         // Размер пакета
            get {
                int ret = HeaderSize;
                if (ret == 0) return 0;
                
                int N = ItemCount;
                // строки (для ASCII)
                for (int i = 0; i < N; i++)
                    ret += asData[i].Length;
                return ret;
            }
        }

        public int LoadHeader(byte[] abBuffer, int nShift = 0) {
            if (abBuffer.Length < nShift + 3) return 0;

            // тип пакета
            pt = (PacketType)abBuffer[nShift + 0];
            // кол-во строк
            ushort N = BitConverter.ToUInt16(abBuffer, nShift + 1); // 2 байта -> ushort
            if (abBuffer.Length < nShift + 3 + 2 * N) return 0;

            asData = new string[N];
            anLen = new ushort[N];
            int ret = HeaderSize;
            for (int i = 0; i < N; i++) { // массив длин
                anLen[i] = BitConverter.ToUInt16(abBuffer, nShift + 3 + 2 * i);
                ret += anLen[i];
            }
            return ret; // размер пакета
        }

        public byte[] ToBytes() {      // пакет -> массив байт
            abBuffer = new byte[Size];

            byte[] ab;
            int iPos = 0; // индекс позиционной записи в abBuffer

            // тип пакета

            abBuffer[iPos] = (byte)pt;
            iPos += 1;

            // кол-во строк
            ushort N = (ushort)ItemCount;
            ab = BitConverter.GetBytes(N); // ushort -> 2 байта
            ab.CopyTo(abBuffer, iPos);
            iPos += ab.Length;

            // длины строк
            for (int i = 0; i < N; i++) {
                ab = BitConverter.GetBytes((ushort)asData[i].Length); // используем массив строк, а не длин, т.к. последний не инициализируется в Packet()
                ab.CopyTo(abBuffer, iPos);
                iPos += ab.Length;
            }
            // строки
            for (int i = 0; i < N; i++) {
                ab = Str2Bytes(asData[i]);
                ab.CopyTo(abBuffer, iPos);
                iPos += ab.Length;
            }
            return abBuffer;
        }

        public void FromBytes(byte[] abIn, int nShift = 0, bool bLoadHeader = false) { // массив байт -> пакет
            if (bLoadHeader && LoadHeader(abIn, nShift) == 0)
                throw new Exception("Insufficient data to load a header");

            int N = ItemCount;
            int iPos = nShift + HeaderSize; // заголовок
            for (int i = 0; i < N; i++) {   // тело
                asData[i] = Bytes2Str(abIn, iPos, anLen[i]);
                iPos += anLen[i];
            }
        }
        // строим новый пакет из массива байт
        public static Packet ParseBytes(byte[] bytes, int nShift = 0) {
            Packet packet = new Packet();
            packet.FromBytes(bytes, nShift, true);
            return packet;
        }
        public string[] Items {
            get { return asData; }
            set { asData = value; }
        }

        public string GetItem(int i) { 
            if (i < 0 || i >= ItemCount) return "";
            return asData[i];
        }

        public void SetItem(int i, string sValue) {
            if (i < 0 || i >= ItemCount) return;
            asData[i] = sValue;
        }

        public void FromItemList(List<string> listItems) {
            if (ItemCount != listItems.Count)
                asData = new string[listItems.Count];
            int N = ItemCount;
            for (int i = 0; i < N; i++)
                asData[i] = listItems[i];
        }

        // строка -> массив байт
        private static byte[] Str2Bytes(string s)                             { return Encoding.GetEncoding(1251).GetBytes(s); }
        // массив байт (позиция, число байт) -> строка
        private static string Bytes2Str(byte[] bytes, int nIndex, int nCount) { return Encoding.GetEncoding(1251).GetString(bytes, nIndex, nCount); }
    }
