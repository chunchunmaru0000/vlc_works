using System;
using System.IO.Ports;
using System.Threading;

namespace vlc_works015
{
    /*
     * Decompiled from https://thyb.oss-cn-beijing.aliyuncs.com/DCFG.exe
     * Via https://github.com/dnSpyEx/dnSpy x86
     * 
     * This class was in 配置软件.BaseClass namespace
     * 
     * Was modified to have a constructor with portName parameter
     */

    // Token: 0x02000027 RID: 39
    public class MODBUS
    {
        // Token: 0x060002BF RID: 703 RVA: 0x000803B0 File Offset: 0x0007E5B0
        public MODBUS(string portName)
        {
            UartPort = new SerialPort(portName);
            this.UartPort.BaudRate = 9600;
            this.UartPort.DataBits = 8;
            this.UartPort.Parity = Parity.None;
            this.UartPort.StopBits = StopBits.One;
            this.UartPort.DataReceived += this.UartPortDataRecv;
        }

        // Token: 0x060002C0 RID: 704 RVA: 0x00080440 File Offset: 0x0007E640
        private void UartPortDataRecv(object sender, SerialDataReceivedEventArgs e)
        {
            bool flag = e.EventType == SerialData.Eof;
            if (!flag)
            {
                this.RecvEnd = true;
            }
        }

        // Token: 0x060002C1 RID: 705 RVA: 0x00080468 File Offset: 0x0007E668
        public bool Open()
        {
            try
            {
                bool isOpen = this.UartPort.IsOpen;
                if (isOpen)
                {
                    this.UartPort.Close();
                }
                this.UartPort.ReadTimeout = 100;
                this.UartPort.DtrEnable = true;
                this.UartPort.RtsEnable = true;
                this.UartPort.Open();
            }
            catch
            {
                return false;
            }
            return true;
        }

        // Token: 0x060002C2 RID: 706 RVA: 0x000804E8 File Offset: 0x0007E6E8
        public void Close()
        {
            bool isOpen = this.UartPort.IsOpen;
            if (isOpen)
            {
                this.UartPort.Close();
            }
        }

        // Token: 0x060002C3 RID: 707 RVA: 0x00080513 File Offset: 0x0007E713
        public void SetComPara(int baud, Parity par, StopBits bit)
        {
            this.UartPort.BaudRate = baud;
            this.UartPort.Parity = par;
            this.UartPort.StopBits = bit;
        }

        // Token: 0x060002C4 RID: 708 RVA: 0x00080540 File Offset: 0x0007E740
        private ushort CRC16(byte[] dataIn, int length)
        {
            int num = 65535;
            int num2;
            for (int i = 0; i < length; i = num2 + 1)
            {
                num ^= (int)dataIn[i];
                for (int j = 0; j < 8; j = num2 + 1)
                {
                    num = (((num & 1) != 0) ? ((num >> 1) ^ 40961) : (num >> 1));
                    num2 = j;
                }
                num2 = i;
            }
            return (ushort)num;
        }

        // Token: 0x060002C5 RID: 709 RVA: 0x000805A4 File Offset: 0x0007E7A4
        private byte[] UartSendRecv(byte[] SendBuf, byte SendNum)
        {
            this.RecvEnd = false;
            try
            {
                bool flag = this.UartPort.BytesToRead > 0;
                if (flag)
                {
                    byte[] array = new byte[this.UartPort.BytesToRead];
                    this.UartPort.Read(array, 0, this.UartPort.BytesToRead);
                }
                ushort num = this.CRC16(SendBuf, (int)(SendNum - 2));
                SendBuf[(int)(SendNum - 1)] = (byte)(num >> 8);
                SendBuf[(int)(SendNum - 2)] = (byte)(num & 255);
                this.UartPort.Write(SendBuf, 0, (int)SendNum);
                Thread.Sleep((int)(SendNum * 11) / this.UartPort.BaudRate + 2);
                int num2 = 0;
                while (!this.RecvEnd && num2 < 20)
                {
                    Thread.Sleep(20);
                    int num3 = num2;
                    num2 = num3 + 1;
                }
                num2 = 0;
                bool flag2 = this.UartPort.BytesToRead == 0;
                if (flag2)
                {
                    return null;
                }
                if (this.RecvEnd || num2 >= 20)
                {
                }
                byte[] array2 = new byte[this.UartPort.BytesToRead];
                this.UartPort.Read(array2, 0, this.UartPort.BytesToRead);
                bool flag3 = array2.Length > 2;
                if (flag3)
                {
                    num = this.CRC16(array2, array2.Length - 2);
                    bool flag4 = array2[array2.Length - 1] == (byte)(num >> 8) && array2[array2.Length - 2] == (byte)(num & 255);
                    if (flag4)
                    {
                        return array2;
                    }
                }
            }
            catch
            {
            }
            return null;
        }

        // Token: 0x060002C6 RID: 710 RVA: 0x00080748 File Offset: 0x0007E948
        public ushort[] Search(int addr)
        {
            bool flag = this.UartPort.BytesToRead > 0;
            if (flag)
            {
                byte[] array = new byte[this.UartPort.BytesToRead];
                this.UartPort.Read(array, 0, this.UartPort.BytesToRead);
            }
            byte[] array2 = new byte[] { 36, 48, 48, 53, 65, 65, 53, 10 };
            array2[1] = (byte)(addr / 16 % 16 + 48);
            array2[2] = (byte)(addr % 16 + 48);
            this.UartPort.ReceivedBytesThreshold = 6;
            this.UartPort.Write(array2, 0, 8);
            int num = 0;
            this.RecvEnd = false;
            while (!this.RecvEnd && num < 20)
            {
                Thread.Sleep(20);
                int num2 = num;
                num = num2 + 1;
            }
            bool flag2 = this.UartPort.BytesToRead == 6;
            if (flag2)
            {
                byte[] array3 = new byte[6];
                this.UartPort.Read(array3, 0, 6);
                byte b = Convert.ToByte(array3[1] + array3[2] + array3[3]);
                bool flag3 = array3[0] == 36 && array3[5] == 10 && b == array3[4];
                if (flag3)
                {
                    return new ushort[]
                    {
                        (ushort)array3[1],
                        (ushort)array3[2],
                        (ushort)array3[3]
                    };
                }
            }
            return null;
        }

        // Token: 0x060002C7 RID: 711 RVA: 0x00080898 File Offset: 0x0007EA98
        public byte[] Search()
        {
            bool flag = this.UartPort.BytesToRead > 0;
            if (flag)
            {
                byte[] array = new byte[this.UartPort.BytesToRead];
                this.UartPort.Read(array, 0, this.UartPort.BytesToRead);
            }
            byte[] array2 = new byte[] { 58, 32, 53, 65, 65, 53, 32, 10 };
            this.UartPort.ReceivedBytesThreshold = 6;
            this.UartPort.Write(array2, 0, 8);
            int num = 0;
            this.RecvEnd = false;
            while (!this.RecvEnd && num < 20)
            {
                Thread.Sleep(20);
                int num2 = num;
                num = num2 + 1;
            }
            bool flag2 = this.UartPort.BytesToRead == 6;
            if (flag2)
            {
                byte[] array3 = new byte[6];
                this.UartPort.Read(array3, 0, 6);
                byte b = Convert.ToByte(array3[1] + array3[2] + array3[3]);
                bool flag3 = array3[0] == 36 && array3[5] == 10 && b == array3[4];
                if (flag3)
                {
                    return array3;
                }
            }
            return null;
        }

        // Token: 0x060002C8 RID: 712 RVA: 0x000809B4 File Offset: 0x0007EBB4
        public bool[] ReadCoil(int Addr, int CoilAddr, int CoilNum)
        {
            this.UartPort.ReceivedBytesThreshold = 5 + (CoilNum + 7) / 8;
            byte[] array = new byte[] { 1, 1, 0, 0, 0, 1, 0, 0 };
            array[0] = (byte)Addr;
            array[2] = (byte)(CoilAddr >> 8);
            array[3] = (byte)CoilAddr;
            array[5] = (byte)CoilNum;
            byte[] array2 = this.UartSendRecv(array, 8);
            bool flag = array2 == null;
            bool[] array3;
            if (flag)
            {
                array3 = null;
            }
            else
            {
                bool[] array4 = new bool[CoilNum];
                int num;
                for (int i = 0; i < CoilNum; i = num + 1)
                {
                    array4[i] = ((int)array2[3 + i / 8] & (1 << i % 8)) > 0;
                    num = i;
                }
                array3 = array4;
            }
            return array3;
        }

        // Token: 0x060002C9 RID: 713 RVA: 0x00080A58 File Offset: 0x0007EC58
        public bool WriteCoil(int Addr, int CoilAddr, bool CoilVal)
        {
            this.UartPort.ReceivedBytesThreshold = 5;
            byte[] array = new byte[] { 1, 15, 0, 0, 0, 1, 1, 0, 0, 0 };
            array[0] = (byte)Addr;
            array[2] = (byte)(CoilAddr >> 8);
            array[3] = (byte)CoilAddr;
            if (CoilVal)
            {
                array[7] = 1;
            }
            byte[] array2 = this.UartSendRecv(array, 10);
            bool flag = array2 == null;
            return !flag;
        }

        // Token: 0x060002CA RID: 714 RVA: 0x00080AC4 File Offset: 0x0007ECC4
        public bool WriteCoil(int Addr, int CoilAddr, bool[] CoilBuf)
        {
            bool flag = CoilBuf.Length > 32;
            bool flag2;
            if (flag)
            {
                flag2 = false;
            }
            else
            {
                this.UartPort.ReceivedBytesThreshold = 5;
                byte[] array = new byte[(CoilBuf.Length + 7) / 8 + 9];
                array[0] = (byte)Addr;
                array[1] = 15;
                array[2] = (byte)(CoilAddr >> 8);
                array[3] = (byte)CoilAddr;
                array[4] = (byte)(CoilBuf.Length >> 8);
                array[5] = (byte)(CoilBuf.Length & 255);
                array[6] = (byte)((CoilBuf.Length + 7) / 8);
                int num;
                for (int i = 0; i < CoilBuf.Length; i = num + 1)
                {
                    ref byte ptr = ref array[7 + i / 8];
                    ptr &= (byte)(~(byte)(1 << i % 8));
                    bool flag3 = CoilBuf[i];
                    if (flag3)
                    {
                        ptr = ref array[7 + i / 8];
                        ptr |= (byte)(1 << i % 8);
                    }
                    num = i;
                }
                bool flag4 = this.UartSendRecv(array, (byte)array.Length) != null;
                flag2 = flag4;
            }
            return flag2;
        }

        // Token: 0x060002CB RID: 715 RVA: 0x00080BBC File Offset: 0x0007EDBC
        public bool WriteReg(int Addr, int RegAddr, ushort Reg)
        {
            this.UartPort.ReceivedBytesThreshold = 8;
            byte[] array = new byte[]
            {
                1, 16, 0, 0, 0, 1, 2, 0, 0, 0,
                0
            };
            array[0] = (byte)Addr;
            array[2] = (byte)(RegAddr >> 8);
            array[3] = (byte)RegAddr;
            array[7] = (byte)(Reg >> 8);
            array[8] = (byte)(Reg & 255);
            return this.UartSendRecv(array, 11) != null;
        }

        // Token: 0x060002CC RID: 716 RVA: 0x00080C28 File Offset: 0x0007EE28
        public bool WriteReg(int Addr, int RegAddr, ushort[] RegBuf)
        {
            bool flag = RegBuf.Length > 64;
            bool flag2;
            if (flag)
            {
                flag2 = false;
            }
            else
            {
                this.UartPort.ReceivedBytesThreshold = 8;
                byte[] array = new byte[RegBuf.Length * 2 + 9];
                array[0] = (byte)Addr;
                array[1] = 16;
                array[2] = (byte)(RegAddr >> 8);
                array[3] = (byte)RegAddr;
                array[4] = (byte)(RegBuf.Length >> 8);
                array[5] = (byte)(RegBuf.Length & 255);
                array[6] = (byte)(RegBuf.Length * 2);
                int num;
                for (int i = 0; i < RegBuf.Length; i = num + 1)
                {
                    array[7 + i * 2] = (byte)(RegBuf[i] >> 8);
                    array[8 + i * 2] = (byte)(RegBuf[i] & 255);
                    num = i;
                }
                bool flag3 = this.UartSendRecv(array, (byte)array.Length) != null;
                flag2 = flag3;
            }
            return flag2;
        }

        // Token: 0x060002CD RID: 717 RVA: 0x00080CF0 File Offset: 0x0007EEF0
        public ushort[] ReadReg(int Addr, int RegAddr, int RegNum)
        {
            this.UartPort.ReceivedBytesThreshold = 5 + RegNum * 2;
            byte[] array = new byte[] { 1, 3, 0, 0, 0, 1, 0, 0 };
            array[0] = (byte)Addr;
            array[2] = (byte)(RegAddr >> 8);
            array[3] = (byte)RegAddr;
            array[5] = (byte)RegNum;
            byte[] array2 = this.UartSendRecv(array, 8);
            bool flag = array2 == null;
            ushort[] array3;
            if (flag)
            {
                array3 = null;
            }
            else
            {
                ushort[] array4 = new ushort[RegNum];
                int num;
                for (int i = 0; i < RegNum; i = num + 1)
                {
                    array4[i] = (ushort)array2[3 + i * 2];
                    ref ushort ptr = ref array4[i];
                    ptr = (ushort)(ptr << 8);
                    ptr = ref array4[i];
                    ptr |= (ushort)array2[4 + i * 2];
                    num = i;
                }
                array3 = array4;
            }
            return array3;
        }

        // Token: 0x060002CE RID: 718 RVA: 0x00080DB4 File Offset: 0x0007EFB4
        public float[] ReadFloat(int Addr, int FloatAddr, int FloatNum)
        {
            this.UartPort.ReceivedBytesThreshold = 5 + FloatNum * 4;
            byte[] array = new byte[] { 1, 3, 0, 0, 0, 1, 0, 0 };
            array[0] = (byte)Addr;
            array[2] = (byte)(FloatAddr >> 8);
            array[3] = (byte)FloatAddr;
            array[5] = (byte)(FloatNum * 2);
            byte[] array2 = this.UartSendRecv(array, 8);
            bool flag = array2 == null;
            float[] array3;
            if (flag)
            {
                array3 = null;
            }
            else
            {
                float[] array4 = new float[FloatNum];
                int num;
                for (int i = 0; i < FloatNum; i = num + 1)
                {
                    array4[i] = BitConverter.ToSingle(new byte[]
                    {
                        array2[6 + i * 4],
                        array2[5 + i * 4],
                        array2[4 + i * 4],
                        array2[3 + i * 4]
                    }, 0);
                    num = i;
                }
                array3 = array4;
            }
            return array3;
        }

        // Token: 0x060002CF RID: 719 RVA: 0x00080E84 File Offset: 0x0007F084
        public bool WriteFloat(int Addr, int FloatAddr, float[] FloatBuf)
        {
            bool flag = FloatBuf.Length > 16;
            bool flag2;
            if (flag)
            {
                flag2 = false;
            }
            else
            {
                this.UartPort.ReceivedBytesThreshold = 8;
                byte[] array = new byte[FloatBuf.Length * 4 + 9];
                array[0] = (byte)Addr;
                array[1] = 16;
                array[2] = (byte)(FloatAddr >> 8);
                array[3] = (byte)FloatAddr;
                array[4] = (byte)(FloatBuf.Length * 2 >> 8);
                array[5] = (byte)((FloatBuf.Length * 2) & 255);
                array[6] = (byte)(FloatBuf.Length * 4);
                int num;
                for (int i = 0; i < FloatBuf.Length; i = num + 1)
                {
                    byte[] bytes = BitConverter.GetBytes(FloatBuf[i]);
                    array[7 + i * 4] = bytes[3];
                    array[8 + i * 4] = bytes[2];
                    array[9 + i * 4] = bytes[1];
                    array[10 + i * 4] = bytes[0];
                    num = i;
                }
                bool flag3 = this.UartSendRecv(array, (byte)array.Length) != null;
                flag2 = flag3;
            }
            return flag2;
        }

        // Token: 0x04000806 RID: 2054
        public SerialPort UartPort;

        // Token: 0x04000807 RID: 2055
        public int TimeInterval = 1000;

        // Token: 0x04000808 RID: 2056
        public int TimeOutVal = 200;

        // Token: 0x04000809 RID: 2057
        private bool RecvEnd = false;
    }
}

