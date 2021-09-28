using System;
using System.Collections.Generic;
using System.Text;

namespace UAVMod
{
    class PID
    {
        private double Last_e;
        private double E;
        public double Target { get; set; }
        public double Current { get; set; }
        public double Pid_out { get; set; }
        public double Kp { get; set; }
        public double Ki { get; set; }
        public double Kd { get; set; }
        public double Max_i { get; set; }
        public double Max_Band { get; set; }
        public double Max_out { get; set; }
        public double P { get; private set; }
        public double I { get; private set; }
        public double D { get; private set; }

        public PID() { Init(); }
        /// <summary>
        /// 构造pid并初始化
        /// </summary>
        /// <param name="kp">比例系数</param>
        /// <param name="ki">积分系数</param>
        /// <param name="kd">微分系数</param>
        /// <param name="max_i">积分最大值</param>
        /// <param name="max_Band">积分分离值</param>
        /// <param name="max_out">最大输出值</param>
        public PID(double kp, double ki, double kd, double max_i, double max_Band, double max_out)
        {
            Init();
            Kp = kp;
            Ki = ki;
            Kd = kd;
            Max_i = max_i;
            Max_Band = max_Band;
            Max_out = max_out;
        }
        public void Init()
        {
            Target = 0.0f;//目标
            Current = 0.0f;//实际
            Pid_out = 0.0f;//输出

            E = 0.0f;//误差
            Last_e = 0.0f;//上一次误差

            P = 0.0f;//比例
            I = 0.0f;//积分
            D = 0.0f;//微分
            Kp = 0.0f;
            Ki = 0.0f;
            Kd = 0.0f;

            Max_i = 0f;//积分限幅值
            Max_Band = 0f;//积分分离值
            Max_out = 0f;//PID限幅值
        }
        /// <summary>
        /// 计算pid当前的输出状态，需要设置 实际值 Current 和 目标值 Target，结果保存在 Pid_out
        /// </summary>
        public void pid_calc()
        {
            E = Target - Current;
            P = Kp * E;
            //积分分离
            if (E < Max_Band)
            {                
                I += Ki * E;
                //积分限幅
                if (I > Max_i) I = Max_i;
                if (I < -Max_i) I = -Max_i;
            }
            else I = 0;
            D = Kd * (E - Last_e);
            Pid_out = P + I + D;
            if (Pid_out > Max_out) Pid_out = Max_out;
            if (Pid_out < -Max_out) Pid_out = -Max_out;
            Last_e = E;
        }
    }
}
