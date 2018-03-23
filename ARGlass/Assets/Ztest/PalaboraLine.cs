using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PalaboraLine : MonoBehaviour
{
    /*
    抛物线公式：y=a*x*x+b*x+c,由于z轴方向为正方向，所以：y=a*z*z+b*z+c,我们把抛出点设为原点（0，0），所以抛物线为y=a*z*z+b*z。
    */
    public float a;//控制抛物线的开口和大小
    private float k;//k为抛物线上抛出点的切线 y=kx+d 的斜率，我们把抛出点设为原点，所以：y = kx，k = 2ax +b
    private float b;// 我们把抛出点设为坐标原点，所以由k = 2ax + b,b = k;
    public LineRenderer line; //抛物线的LineRenderer组件
    public int density;//抛物线的精度
    public float space = 5;//每个节点间的间隔 
                           // y = a*space*space+b*space 

    public GameObject sphere;
    List<GameObject> points = new List<GameObject>();
    Vector3 prevPoint;
    void Start()
    {
       
        for (int i = 0; i < density; i++)
        {
            points.Add(Instantiate(sphere));
        }
        line.SetVertexCount(density + 1);
        prevPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 f = transform.forward;
        f.y = 0;
        k = transform.forward.y / f.magnitude;//算出切线斜率（注意物体的旋转角度，当forward为垂直方向时，也就是f.magnitude的值为0的时候，应该为一条直线，这里没有考虑这个情况）
        b = k;

        bool cast = false;
        for (int i = 0; i < density; i++)
        {
            Vector3 p = GetPosition(i * space);
            points[i].transform.position = p;
            cast = Cast(p);
            if (cast)
            {
                break;
            }
        }
        if (!cast)
        {

        }
    }
    /// <summary>
    /// 根据z确定点坐标
    /// </summary>
    /// <param name="z"></param>
    /// <returns></returns>
    Vector3 GetPosition(float z)
    {
        float y = a * z * z + b * z;
        Vector3 f = transform.forward;
        f.y = 0;
        f = f.normalized;

        Vector3 pos = transform.position + f * z;//水平方向坐标
        pos.y = transform.position.y + y;//加上垂直方向坐标
        return pos;
    }

    /// <summary>
    /// 进行抛物线检测
    /// </summary>
    /// <param name="currentPoint"></param>
    /// <returns></returns>
    bool Cast(Vector3 currentPoint)
    {
        Vector3 d = currentPoint - prevPoint;
        RaycastHit hit;
        if (Physics.Raycast(prevPoint, d.normalized, out hit, d.magnitude))
        {
            SetLine(hit.point);
            prevPoint = transform.position;
            return true;
        }
        else
        {
            prevPoint = currentPoint;
            return false;
        }
    }

    /// <summary>
    /// 设置抛物线上每个点
    /// </summary>
    /// <param name="endPos"></param>
    void SetLine(Vector3 endPos)
    {
        Vector3 s = transform.position;
        endPos.y = 0;
        s.y = 0;

        float j = Vector3.Distance(s, endPos) / density;

        for (int i = 0; i < density; i++)
        {
            line.SetPosition(i, GetPosition(i * j));
        }
        line.SetPosition(density, endPos);
        sphere.transform.position = endPos;
    }
}