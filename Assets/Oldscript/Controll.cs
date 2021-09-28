using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controll : MonoBehaviour
{
    public GameObject[] plane=new GameObject[8];

    MoveTo M;
    MoveTo m;

    private float timeLine = 0f;

    private Vector3[] MovePos = new Vector3[4]{
        new Vector3(-4,1.5f,4),
        new Vector3(-4,1.5f,-4),
        new Vector3(4,1.5f,-4),
        new Vector3(4,1.5f,4)
    };//绕圈移动点

    private float[] MovePath3 = new float[4] { 0.5f, 0.25f, -0.25f, -0.5f };

    private int Index;

    void Start()
    {
        Index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Plan1();
        //Plan2();
        
        pauseAll();
        Plan3();




    }

    private void Plan1()
    {
        if (Index > 6) return;
        M = (MoveTo)plane[Index].GetComponent("MoveTo");
        m = (MoveTo)plane[Index + 1].GetComponent("MoveTo");
        if (Index == 0) Path1(M,1);
        if (fromToDistance(M.getStartPos(), m.getStartPos())>1.5000f)
        {
            if (fromToDistance(plane[Index].transform.position, plane[Index + 1].transform.position) < 3.5001f)
            {
                Path1(m,1);
                Index++;
            }
            return;
        }
        if (fromToDistance(plane[Index].transform.position, plane[Index + 1].transform.position) > 1.5001f)
        {
            Path1(m,1);
            Index++;
        }
    }

    //绕圈飞行路径
    private void Path1(MoveTo plane,int n)
    {
        //起飞
        Vector3 nextPos = plane.getStartPos();
        nextPos.y += 1.5f;
        plane.addPos(nextPos);
        nextPos.z = 4f;
        plane.addPos(nextPos);
        //绕圈圈数
        for (int j = 0; j < n-1; j++)
        {
            for (int i = 0; i < MovePos.Length; i++)
            {
                plane.addPos(MovePos[i]);
            }
        }
        //复原
        plane.addPos(MovePos[0]);
        plane.addPos(MovePos[1]);
        plane.addPos(MovePos[2]);
        nextPos = plane.getStartPos();
        nextPos.y += 1.5f;
        nextPos.x = 4;
        plane.addPos(nextPos);
        nextPos = plane.getStartPos();
        nextPos.y += 1.5f;
        plane.addPos(nextPos);
        plane.addPos(plane.getStartPos());
    }

    private void Plan2()
    {
        if (Index != 8)
        {
            for (int i = 0; i < plane.Length; i++)
            {
                M = (MoveTo)plane[i].GetComponent("MoveTo");
                Path2(M, 4);
                Index++;
            }
        }
    }

    //上下飞行路径
    private void Path2(MoveTo plane, int n)
    {
        //起飞
        Vector3 nextPos = plane.getStartPos();
        nextPos.y += 1.6f;
        plane.addPos(nextPos);
        nextPos.x -= 3.5f;
        nextPos.z -= 2.5f;
        plane.addPos(nextPos);
        if (nextPos.z < 0) nextPos.z -= 0.5f;
        else nextPos.z += 0.5f;
        plane.addPos(nextPos);
        //上下飞行
        if (nextPos.z > 0) nextPos.y += 0.5f;
        else nextPos.y -= 0.5f;
        plane.addPos(nextPos);
        if (nextPos.z > 0) nextPos.y -= 1.0f;
        else nextPos.y += 1.0f;
        plane.addPos(nextPos);
        for (int i = 0; i < (n - 1)*2; i++)
        {
            if(nextPos.z > 0)
            {
                if (i % 2 == 0) nextPos.y += 1.0f;
                else nextPos.y -= 1.0f;
            }
            else
            {
                if (i % 2 == 0) nextPos.y -= 1.0f;
                else nextPos.y += 1.0f;
            }
            plane.addPos(nextPos);
        }
        if (nextPos.z > 0) nextPos.y += 0.5f;
        else nextPos.y -= 0.5f;
        plane.addPos(nextPos);
        //恢复
        if (nextPos.z < 0) nextPos.z += 0.5f;
        else nextPos.z -= 0.5f;
        plane.addPos(nextPos);
        nextPos = plane.getStartPos();
        nextPos.y += 1.6f;
        plane.addPos(nextPos);
        plane.addPos(plane.getStartPos());
    }

    private float line1(float x)
    {
        return x *(1/3.0f)*Mathf.Sin(Mathf.PI* timeLine);
    }

    private void pauseAll()
    {
        if (Index != 8)
        {
            for (int i = 0; i < plane.Length; i++)
            {
                M = (MoveTo)plane[i].GetComponent("MoveTo");
                M.changePauseFalg();
                Index++;
            }
        }
    }
    private int flag = 0;
    //跷跷板上下飞行路径
    private void Plan3()
    {
        if (flag == 0)
        {
            int num = 0;
            for (int i = 0; i < plane.Length; i++)
            {
                M = (MoveTo)plane[i].GetComponent("MoveTo");
                Vector3 nextMovePos = M.getStartPos();
                nextMovePos.y += 1.5f;
                M.transform.position = Vector3.MoveTowards(M.transform.position, nextMovePos, Time.deltaTime);
                if (Vector3.Distance(nextMovePos, M.transform.position) < 0.00001f)
                {
                    num++;
                }
            }
            if (num == 8) flag++;
        }

        if (flag == 1)
        {
            int num = 0;
            for (int i = 0; i < plane.Length; i++)
            {
                M = (MoveTo)plane[i].GetComponent("MoveTo");
                Vector3 nextMovePos = M.getStartPos();
                nextMovePos.y += 1.5f;
                nextMovePos.x -= 3.5f;
                nextMovePos.z -= 2.5f;
                M.transform.position = Vector3.MoveTowards(M.transform.position, nextMovePos, Time.deltaTime);
                if (Vector3.Distance(nextMovePos, M.transform.position) < 0.00001f)
                {
                    num++;
                }
            }
            if (num == 8) flag++;
        }
        if (flag == 2)
        {
            timeLine += Time.deltaTime;
            for (int i = 0; i < plane.Length; i++)
            {
                M = (MoveTo)plane[i].GetComponent("MoveTo");
                Vector3 nextMovePos = M.getStartPos();
                nextMovePos.y += 1.5f;
                nextMovePos.x -= 3.5f;
                nextMovePos.z -= 2.5f;
                nextMovePos.y += line1(nextMovePos.z);
                M.transform.position = nextMovePos;
            }
        }
    }

    private void Plan4()
    {
        if (Index != 8)
        {
            for (int i = 0; i < plane.Length; i++)
            {
                M = (MoveTo)plane[i].GetComponent("MoveTo");
                Path4(M, 8);
                Index++;
            }
        }
    }

    //上下飞行路径
    private void Path4(MoveTo plane, int n)
    {
        //起飞
        Vector3 nextPos = plane.getStartPos();
        nextPos.y += 1.6f;
        plane.addPos(nextPos);
        nextPos.x -= 3.5f;
        nextPos.z -= 2.5f;
        plane.addPos(nextPos);
        if (nextPos.x < 0) nextPos.x -= 0.5f;
        else nextPos.x += 0.5f;
        plane.addPos(nextPos);
        //上下飞行
        if (Index % 2 == 0) nextPos.y += 0.25f;
        else nextPos.y -= 0.25f;
        plane.addPos(nextPos);
        for (int i = 0; i < (n - 1) * 2; i++)
        {
            if ((Index + i) % 2 == 0) nextPos.y -= 0.5f;
            else nextPos.y += 0.5f;
            plane.addPos(nextPos);
        }
        if ((Index + n - 1) % 2 == 0) nextPos.y += 0.25f;
        else nextPos.y -= 0.25f;
        plane.addPos(nextPos);
        //恢复
        if (nextPos.x < 0) nextPos.x += 0.5f;
        else nextPos.x -= 0.5f;
        plane.addPos(nextPos);
        nextPos = plane.getStartPos();
        nextPos.y += 1.6f;
        plane.addPos(nextPos);
        plane.addPos(plane.getStartPos());
    }

    //测距
    private float fromToDistance(Vector3 a,Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
}
