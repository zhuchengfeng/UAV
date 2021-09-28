using System;
using System.Text;

namespace UWBMod
{
    //点
    struct Point
    {
        public byte role;//角色代号（03-控制台，02-标签，01-基站）
        public byte id;// 标签ID（根据自己的实际配置决定，ID 自己定，没有什么特别的要求）
        public ushort x;
        public byte x_sign;
        public ushort y;
        public byte y_sign;
        public ushort z;
        public byte z_sign;
    }

    class PortData
    {
        //private uint reserved;//预留参数 第一个有4b+9b后面的9b
        public byte Frame_Header { get; set; }//数据开头 默认55
        public byte Function_Mark { get; set; }//功能标记 默认03
        public ushort Frame_Length { get; set; }//数据长度
        public byte Role { get; set; }//角色代号（03-控制台，02-标签，01-基站）
        public byte Id { get; set; }//控制台 ID（根据自己的实际配置决定，ID 自己定，没有什么特别的要求）
        public uint System_time { get; set; }//系统时间
        public uint Local_time { get; set; }//本地时间
        public byte Sum_Check { get; set; }//校验码
        public bool Flag { get; set; }//该数据是否合法
        internal Point[] Point { get; set; }

        public PortData(byte[] data)
        {
            int x = (data.Length - 28) / 20;
            Frame_Header = data[0];
            Function_Mark = data[1];
            Frame_Length = (ushort)((data[3] << 8) | data[2]);
            Role = data[4];
            Id = data[5];
            System_time = (uint)(data[9] << 24 | data[8] << 16 | data[7] << 8 | data[6]);
            Local_time = (uint)(data[13] << 16 | data[12] << 16 | data[11] << 8 | data[10]);
            int num = 18;
            Point = new Point[x];
            for (int i = 0; i < x; i++)
            {
               
                num += 9;
                Point[i].role = data[num++];
                Point[i].id = data[num++];
                Point[i].x = (ushort)(data[num + 1] << 8 | data[num]);
                num += 2;
                Point[i].x_sign = data[num++];
                Point[i].y = (ushort)(data[num + 1] << 8 | data[num]);
                num += 2;
                Point[i].y_sign = data[num++];
                Point[i].z = (ushort)(data[num + 1] << 8 | data[num]);
                num += 2;
                Point[i].z_sign = data[num++];
            }
            num += 9;
            Sum_Check = data[num];
            byte sum = 0;
            for (int i = 0; i < data.Length - 1; i++)
            {
                sum += data[i];
            }
            Flag = sum == Sum_Check;
        }



        //按指定编码将16进制字符串转成字符
        public string HexStringToString(string hs, Encoding encoding)
        {
            string[] chars = hs.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] b = new byte[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                b[i] = Convert.ToByte(chars[i], 16);
            }
            return encoding.GetString(b);
        }

        // 字符串转16进制字节数组
        public static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        // 字节数组转16进制字符串
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
    }
}
