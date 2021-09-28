using UWBMod;
using System;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.UI;

public class AutoMove : MonoBehaviour {

	public Text X;
	public Text Y;

    SerialPort sp;
	//以下定义4个公有变量，用于参数传递
	public static string strProtName = "COM1";
	public static string strBaudRate = "921600";
	public static string strDataBits = "8";
	public static string strStopBits = "1";

	private bool readFlag = false;
	void Start()
	{
        sp = new SerialPort();//实例化串口通讯类
        sp.BaudRate = int.Parse(strBaudRate);//波特率
		sp.DataBits = int.Parse(strDataBits);//数据位
		sp.StopBits = (StopBits)int.Parse(strStopBits);//停止位
		sp.ReadTimeout = 500;//读取数据的超时时间，引发ReadExisting异常
		for(int i = 1; i < 10; i++)
        {
			strProtName = "COM" + i;
			sp.PortName = strProtName;//串口号
			try
			{
				sp.Open();
				print(strProtName + "已开启");
				break;
			}
			catch (Exception ex)
			{
				print("错误：" + ex.Message);
			}
		}
        if (sp.IsOpen)
        {
			readFlag = true; 
        }
        else
        {
			print("错误：没有1-9的串口");
		}

	}
    float totalTime = 0;

    private void FixedUpdate()
    {
        if (readFlag)
        {
            totalTime += Time.deltaTime;
            if (totalTime >= 0.04)
            {
                totalTime = 0;
                ReadData(); 
            }
        }
	}

	private void ReadData()
	{
        //读取缓冲区的数据
        byte[] byteData = new byte[4096];
        sp.Read(byteData, 0, 4096);
        //数据开始标记
        int start = -1;
        //数据大小统计
		int size = 0;
        //临时存放数组
		byte[] tempData;
        //接口数据格式
		PortData data;
        //坐标统计
		double[] ans_x = new double[10];
        double[] ans_y = new double[10];
        //一次缓冲数据的有效数据个数
		int num = 0;
        //解析数据
		for (int i = 0; i < byteData.Length; i++)
		{
			if (byteData[i] == 0x55)
			{
				if (start != -1)
				{
					if (size < 48) continue;
					tempData = new byte[size];
					Array.Copy(byteData, start, tempData, 0, size);
					data = new PortData(tempData);
                    //每个无人机坐标的平均值
                    //print(data.System_time);
                    //print(data.Local_time);
                    if (data.Flag)
                    {
						for (int j = 0; j < data.Point.Length; j++)
						{
                            ans_x[j] += data.Point[j].x/1000.0;
							ans_y[j] += data.Point[j].y/1000.0;
                            num++;
						}
					}
					start = i;
					size = 0;
				}
				else
				{
					start = i;
					size = 0;
				}
			}
			size++;
		}
        if (num == 0) return;
        //显示值
        print(num);
        print(ans_x[0] / num);
        print(ans_y[0] / num);
        X.text = "X:" + ans_x[0] / num;
        Y.text = "Y:" + ans_y[0] / num;
        //位置更新
        transform.position =new Vector3((float)ans_x[0] / num, 0.15f, (float)ans_y[0] / num);
    }

	private void OnGUI()
	{
		if (GUILayout.Button("重新启动"))
		{
			//ReadData();
		}
	}
}
