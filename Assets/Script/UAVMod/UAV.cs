using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UAVMod
{
    class UAV
    {
        public UAV()
        {
            Init();
        }
        /// <summary>
        /// 初始化无人机
        /// </summary>
        /// <param name="x">x轴坐标</param>
        /// <param name="y">y轴坐标</param>
        /// <param name="z">z轴坐标</param>
        /// <param name="phi">角度x轴坐标</param>
        /// <param name="theta">角度y轴坐标</param>
        /// <param name="psi">角度z轴坐标</param>
        public UAV(double x, double y, double z, double phi, double theta, double psi)
        {
            Init();
            X = x;
            Y = y;
            Z = z;
            Phi = phi;
            Theta = theta;
            Psi = psi;
        }
        //输入量 旋翼转速,角速度
        public double[] W { get; set; }
        //6个输出量
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        //角度制
        public double Phi { get; set; }//x轴 横滚角
        public double Theta { get; set; }//y轴 俯仰角
        public double Psi { get; set; }//z轴 偏航角
        //中间产生量
        public double[] Speed { get; set; }//当前六个方向的速度
        public double[] Accelerated { get; set; }//当前六个方向的加速度
        public double[] Force { get; set; }//四个螺旋桨产生的力
        //无人机各方向，姿态的控制量
        //0无人机垂直升降控制量
        //1无人机横滚控制量
        //2无人机俯仰控制量
        //3无人机偏航控制量
        public double[] U { get; set; }
        public PID[] pid { get; set; }
        //常量
        public const double m = 1.692f;//质量 kg
        public const double g = 9.85f;//重力加速度
        public const double l = 0.17f;//旋翼到中心的距离 m
        public const double b = 0.042f;//旋翼升力系数
        public const double d = 1f;//旋翼阻力系数
        public const double Ix = 2.35f;//转动惯量 kg*m^2 千克平方米
        public const double Iy = 2.35f;
        public const double Iz = 5.26f;

        //标志位
        public bool start_flag = false;//启动标志
        public bool update_move_flag = false;//更新加速度、速度、位移的标志
        public bool arrive_flag = false;//到达标志
        public bool over_flag = false;//结束标志

        //电机提供的最大转速
        private const double Min_W = 0.0f;
        private const double Max_W = 15.0f;

        //最大水平飞行速度pid分离值
        private const double Max_Speed = 0.09f;

        private void Init()
        {
            W = new double[4];
            Speed = new double[6];
            Accelerated = new double[6];
            Force = new double[4];
            U = new double[4];
            pid = new PID[6];
            //初始化6个方向的pid
            pid[0] = new PID(0.4f, 0f, 150f, 0f, 0f, 0.07f);
            pid[1] = new PID(0.4f, 0f, 150f, 0f, 0f, 0.07f);
            pid[2] = new PID(1.00f, 0f, 90f, 0f, 0f, 2.0f);
            pid[3] = new PID(0.35f, 0f, 60f, 0f, 0f, 0.4f);
            pid[4] = new PID(0.35f, 0f, 60f, 0f, 0f, 0.4f);
            pid[5] = new PID(0.35f, 0f, 60f, 0f, 0f, 0.4f);
        }
        //升力值
        private void Update_Force()
        {
            //升力 Fi=Ki*wi^2 升力计算公式
            for (int i = 0; i < 4; i++)
            {
                if (W[i] < Min_W) W[i] = Min_W;
                if (W[i] > Max_W) W[i] = Max_W;
                Force[i] = b * W[i] * W[i];
            }
        }
        //U0 为垂直运动的总升力
        //U1 为翻滚运动时产生的合力
        //U2 为俯仰运动时产生的合力
        //U3 为偏航运动时产生的合力
        private void Force_U()
        {
            U[0] = Force[0] + Force[1] + Force[2] + Force[3];
            U[1] = l * (Force[3] - Force[1]);
            U[2] = l * (Force[2] - Force[0]);
            U[3] = d * (Force[0] + Force[2] - Force[1] - Force[3]);
        }
        //角度转弧度
        private double DegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return radians;
        }
        //弧度转角度
        private double RadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return degrees;
        }
        //线位移 -> x,y,z轴的加速度
        private void Update_LineA()
        {
            double phi = DegreesToRadians(Phi);
            double theta = DegreesToRadians(Theta);
            double psi = DegreesToRadians(Psi);
            double d1 = Math.Sin(phi) * Math.Sin(psi) + Math.Cos(psi) * Math.Sin(theta) * Math.Cos(phi);
            double d2 = Math.Sin(psi) * Math.Sin(theta) * Math.Cos(phi) - Math.Cos(psi) * Math.Sin(phi);
            double d3 = Math.Cos(phi) * Math.Cos(theta);
            Accelerated[0] = U[0] * d1 / m;//x
            Accelerated[1] = U[0] * d2 / m;//y
            Accelerated[2] = U[0] * d3 / m - g;//z
        }

        //角速度转换矩阵
        private double[,] Angular_Matrix(double Phi, double Theta, double Psi, double p, double q, double r)
        {
            double[,] Matrix = new double[3, 1]
            {
                { p + p * Math.Sin(Phi) * Math.Tan(Theta) + r * Math.Cos(Phi) * Math.Tan(Theta) },//Phi
                { q * Math.Cos(Phi) + r * Math.Sin(Phi) },//Theta
                { q * Math.Sin(Phi) * (1f/Math.Cos(Theta)) + r * Math.Cos(Phi) * (1f/Math.Cos(Theta)) },//Psi
            };
            return Matrix;
        }

        //角位移 -> x,y,z轴的角加速度
        private void Update_AngleA()
        {
            //模型简化
            //double p = Speed[3];
            //double q = Speed[4];
            //double r = Speed[5];
            //角速度转换
            //double[,] ASpeed = ASpeed_Matrix(Phi, Theta, Psi, p, q, r);//高精度才需要
            //Accelerated[3] = (U[1] + (Iy - Iz)*q*r) / Ix; //Phi
            //Accelerated[4] = (U[2] + (Iz - Ix)*p*r) / Iy; //Theta
            Accelerated[3] = U[1] / Ix; //Phi
            Accelerated[4] = U[2] / Iy; //Theta
            Accelerated[5] = U[3] / Iz; //Psi
        }

        /// <summary>
        /// 无人机启动 由start_flag控制
        /// </summary>
        public void Start()
        {
            if (!start_flag) return;
            over_flag = false;
            if (Accelerated[2] > 0f)
            {
                start_flag = false;
                update_move_flag = true;
            }
            else
            {
                for (int i = 0; i < W.Length; i++)
                {
                    W[i] += 0.05;
                }
            }
        }
        /// <summary>
        /// 无人机停止 由over_flag控制
        /// </summary>
        public void Over()
        {
            if (!over_flag) return;
            start_flag = false;
            if (Math.Abs(Z - 0.09f) <= 0.001f)
            {
                over_flag = false;
                update_move_flag = false;
                for (int i = 0; i < W.Length; i++)
                {
                    W[i] -= 0.05;
                    if (W[i] <= 0) W[i] = 0;
                    else over_flag = true;
                }
            }
            else
            {
                Move_Point(X, Y, 0f);
            }
            if (!over_flag)
            {
                for (int i = 0; i < Speed.Length; i++)
                {
                    Speed[i] = 0;
                }
                for (int i = 0; i < Accelerated.Length; i++)
                {
                    Accelerated[i] = 0;
                }
            }
        }
        public void Stop()
        {

        }
        /// <summary>
        /// 更新无人机变量参数
        /// </summary>
        /// <param name="time">t刷新的时间间隔</param>
        public void Update(double time)
        {
            //更新当前旋翼产生的力
            Update_Force();
            Force_U();
            //更新出6个方向的加速度
            Update_LineA();
            Update_AngleA();
            //检查是否可以起飞
            if (!update_move_flag) return;
            //更新位置
            X += Speed[0] * time + Accelerated[0] * time * time / 2;
            Y += Speed[1] * time + Accelerated[1] * time * time / 2;
            Z += Speed[2] * time + Accelerated[2] * time * time / 2;
            Phi += Speed[3] * time + Accelerated[3] * time * time / 2;
            Theta += Speed[4] * time + Accelerated[4] * time * time / 2;
            Psi += Speed[5] * time + Accelerated[5] * time * time / 2;
            //更新速度
            for (int i = 0; i < Speed.Length; i++)
            {
                Speed[i] += Accelerated[i] * time;
            }
        }
        //通过加速度反算出合力u
        private double[] Acc_U(double x, double y, double z, double psi)
        {
            psi = DegreesToRadians(psi);
            //通过x,y,z,psi反算出u0,phi,theta
            double u0 = m * Math.Sqrt(x * x + y * y + (z + g) * (z + g));
            double phi = Math.Asin((x * Math.Sin(psi) - y * Math.Cos(psi)) * m / u0);
            double theta = Math.Asin(x * m - u0 * Math.Sin(psi) * Math.Sin(phi)) / (u0 * Math.Cos(psi) * Math.Cos(phi));
            phi = RadiansToDegrees(phi);
            theta = RadiansToDegrees(theta);
            psi = RadiansToDegrees(psi);
            //通过目标方位角度pid获取角加速度的力
            pid[3].Target = phi;
            pid[4].Target = theta;
            pid[5].Target = psi;
            pid[3].Current = Phi;
            pid[4].Current = Theta;
            pid[5].Current = Psi;
            for (int i = 3; i < pid.Length; i++)
            {
                pid[i].pid_calc();
            }
            double u1 = pid[3].Pid_out * Ix;
            double u2 = pid[4].Pid_out * Iy;
            double u3 = pid[5].Pid_out * Iz;
            return new double[4] {u0, u1, u2, u3};
        }
        //通过合力u反算出螺旋桨转速
        private void U_Force_W(double[] u)
        {
            double f0 = ((u[0] + u[3] / d) / 2 - u[2] / l) / 2;
            double f2 = u[2] / l + f0;
            double f1 = (u[0] - f0 - f2 - u[1] / l) / 2;
            double f3 = u[1] / l + f1;
            if (f0 < 0) f0 = 0;
            if (f1 < 0) f1 = 0;
            if (f2 < 0) f2 = 0;
            if (f3 < 0) f3 = 0;
            W[0] = Math.Sqrt(f0 / b);
            W[1] = Math.Sqrt(f1 / b);
            W[2] = Math.Sqrt(f2 / b);
            W[3] = Math.Sqrt(f3 / b);
        }
        /// <summary>
        /// 按照坐标点控制无人机飞行
        /// </summary>
        /// <param name="x">x轴坐标（范围尽量控制在0，10.2）</param>
        /// <param name="y">y轴坐标（范围尽量控制在0，9.6）</param>
        /// <param name="z">z轴坐标（范围尽量控制在0，3）</param>
        /// <param name="psi">偏航角 默认值0</param>
        public void Move_Point(double x, double y, double z, double psi = 0)
        {
            if (!update_move_flag) return;
            psi %= 45;
            psi = -Math.Abs(psi); 
            pid[0].Target = x;
            pid[1].Target = y;
            pid[2].Target = z;
            pid[0].Current = X;
            pid[1].Current = Y;
            pid[2].Current = Z;
            for (int i = 0; i < 3; i++)
            {
                pid[i].pid_calc();
            }
            //速度限速，超过时移除加速度
            if (Speed[0] > Max_Speed && pid[0].Pid_out > 0) pid[0].Pid_out = 0;
            if (Speed[0] < -Max_Speed && pid[0].Pid_out < 0) pid[0].Pid_out = 0;
            if (Speed[1] > Max_Speed && pid[1].Pid_out > 0) pid[1].Pid_out = 0;
            if (Speed[1] < -Max_Speed && pid[1].Pid_out < 0) pid[1].Pid_out = 0;

            double[] u = Acc_U(pid[0].Pid_out, pid[1].Pid_out, pid[2].Pid_out, psi);
            U_Force_W(u);
        }
        /// <summary>
        /// 判断无人机是否到达目标点，位置精度控制在2cm左右，角度控制在0.1度左右
        /// </summary>
        /// <param name="x">x轴坐标</param>
        /// <param name="y">y轴坐标</param>
        /// <param name="z">z轴坐标</param>
        /// <param name="phi">横滚角 默认值0</param>
        /// <param name="theta">俯仰角 默认值0</param>
        /// <param name="psi">偏航角 默认值0</param>
        /// <returns></returns>
        public bool Is_Arrive_Point(double x, double y, double z, double phi = 0f, double theta = 0f, double psi = 0f)
        {
            if (Math.Abs(X - x) >= 0.02f) return false;
            if (Math.Abs(Y - y) >= 0.02f) return false;
            if (Math.Abs(Z - z) >= 0.02f) return false;
            if (Math.Abs(Phi - phi) >= 0.1f) return false;
            if (Math.Abs(Theta - theta) >= 0.1f) return false;
            if (Math.Abs(Psi - psi) >= 0.1f) return false;
            for (int i = 0; i < Speed.Length; i++)
            {
                if (Math.Abs(Speed[i]) > 0.01f) return false;
            }
            for (int i = 0; i < Accelerated.Length; i++)
            {
                if (Math.Abs(Accelerated[i]) > 0.01f) return false;
            }
            return true;
        }
    }
}
