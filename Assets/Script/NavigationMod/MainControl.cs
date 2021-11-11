using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//无人机编队任务控制器
public class MainControl : MonoBehaviour {

    private Transform[] drone;//位置组件
    private Navigation[] nav;//导航组件
    private bool allFinishFlag = true;//总任务是否完成
    //private int index;//整体任务点序号
    //private bool Flag;//启动完成

    void Start() {
        drone = new Transform[8];
        nav = new Navigation[8];
        //index = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            drone[i] = transform.GetChild(i);
            nav[i] = drone[i].GetComponent<Navigation>();
        }
    }

    void Update()
    {
        startByIndex();
        check();
        if (Input.GetKeyUp(KeyCode.M))
        {
            plan2();
        }
    }

    private void check()
    {
        bool f = true;
        for (int i = 0; i < nav.Length; i++)
        {
            //暂停的不需要进行判断
            if (!nav[i].getPauseFalg())
            {
                //if (nav[i].getFinishFalg())
                    //nav[i].nextPos();
                if (!nav[i].getFinishFalg())
                {
                    f = false;
                }
            }
        }
        if (f)
            for (int i = 0; i < nav.Length; i++)
            {
                //暂停的不需要进行判断
                if (!nav[i].getPauseFalg())
                {
                    if (nav[i].getFinishFalg())
                        nav[i].nextPos();
                }
            }
    }
    
    //按照前一个无人机的任务点进行起飞
    private void startByIndex()
    {
        //int[] def = new int[8] { 0, 2, 2, 2, 5, 2, 2, 2 };
        int[] def = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        for (int i = 0; i < nav.Length - 1; i++)
        {
            if (nav[i].index == def[i + 1])
            {
                nav[i + 1].setPauseFalg(false);
            }
        }
        nav[0].setPauseFalg(false);
    }

    void plan1()
    {
        for (int i = 0; i < nav.Length; i++)
        {
            if (!nav[i].getFinishFalg()) return;
        }
        for (int i = 0; i < nav.Length; i++)
        {
            nav[i].plan1();
        }
    }

    Vector3[] roates = new Vector3[8] {
        new Vector3(6f, 1.5f, 4f),
        new Vector3(5f, 1.5f, 4f),
        new Vector3(4f, 1.5f, 5f),
        new Vector3(4f, 1.5f, 6f),
        new Vector3(5f, 1.5f, 7f),
        new Vector3(6f, 1.5f, 7f),
        new Vector3(7f, 1.5f, 6f),
        new Vector3(7f, 1.5f, 5f),
    };

    //分配器
    public int distributor(int a)
    {
        bool[] f = new bool[8] {false, false, false, false, false, false, false, false};
        int[] ans = new int[8];
        for (int i = 0; i < nav.Length; i++)
        {
            int t = 0;
            for (int j = 0; j < nav.Length; j++)
            {
                if (!f[j])
                {
                    t = j;
                    break;
                }
            }
            //找出到这个目标点最小的无人机
            for (int j = t + 1; j < nav.Length; j++)
            {
                if (f[j]) continue;
                Vector3 last_t = nav[j].getPoint(-1);
                Vector3 last_min = nav[t].getPoint(-1);
                if (Vector3.Distance(last_t, roates[i]) < Vector3.Distance(last_min, roates[i]))
                {
                    t = j;
                }
            }
            f[t] = true;
            ans[t] = i;
        }
        return ans[a];
    }

    void plan2()
    {
        for (int i = 0; i < nav.Length; i++)
        {
            if (!nav[i].getFinishFalg()) return;
        }
        for (int i = 0; i < nav.Length; i++)
        {
            nav[i].plan2(i);
        }
    }
}
