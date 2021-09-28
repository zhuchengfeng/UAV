using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UAVMod;

public class Control : MonoBehaviour {
    //四个螺旋桨
    private Transform[] obj = new Transform[4];
    //无人机数学模型
    private UAV uav;
    //目标点
    private Vector3 Target_point;
    //导航
    private Navigation nav;

    void Start () {
        //获取螺旋桨对象
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            obj[i] = transform.GetChild(0).GetChild(i);
        }
        //旋转角初始化
        transform.localEulerAngles = new Vector3(0, 0, 0);
        //无人机参数初始化
        uav = new UAV(transform.position.x, transform.position.z, transform.position.y,
            transform.rotation.x, transform.rotation.z, transform.rotation.y
            );
        GetComponent<Rigidbody>().mass = (float)UAV.m;
        Target_point = transform.position;
        //导航
        nav = GetComponent<Navigation>();
    }
    void Update() { }

    //固定刷新60hz
	void FixedUpdate () {
        if (Input.GetKey(KeyCode.S))
        {
            uav.start_flag = true;
        }
        if (Input.GetKey(KeyCode.O))
        {
            if (uav.Is_Arrive_Point(Target_point.x, Target_point.z, Target_point.y)) {
                uav.over_flag = true;
            }
        }
        UAV_fly();
    }

    private void UAV_fly()
    {
        //更新螺旋桨旋转
        for (int i = 0; i < uav.W.Length; i++)
        {
            obj[i].transform.Rotate(0, (float)uav.W[i] * 360 / 60f, 0); //10*60
        }
        //调用无人机数学模型进行模拟飞行
        uav.Start();
        if(!uav.over_flag)
            uav.Move_Point(Target_point.x, Target_point.z, Target_point.y);
        uav.Over();
        uav.Update(1 / 60f);
        //增加收敛速度
        if (!uav.over_flag)
            uav.Move_Point(Target_point.x, Target_point.z, Target_point.y);
        uav.Update(1 / 60f);
        if (!uav.over_flag)
            uav.Move_Point(Target_point.x, Target_point.z, Target_point.y);
        uav.Update(1 / 60f);
        //更新目标点
        Target_point = nav.nextMovePos;
        //是否到达，到达后复位
        if (uav.Is_Arrive_Point(nav.startPos.x, nav.startPos.z, 1.5f))
        {
            uav.over_flag = true;
            uav.arrive_flag = true;
        } 
        else
            uav.arrive_flag = false;
        //更新位置
        transform.position = new Vector3((float)uav.X, (float)uav.Z, (float)uav.Y);
        transform.localEulerAngles = new Vector3((float)uav.Phi, (float)uav.Psi, (float)uav.Theta);
    }
}
