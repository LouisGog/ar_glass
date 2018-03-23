using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
public class testThread : MonoBehaviour
{



    void StartThread()

    {
        Thread athread = new Thread(new ThreadStart(goThread));
        
        athread.IsBackground = true;//防止后台现成。相反需要后台线程就设为false
      //  athread.Start();
    }
    void Awake()
    {
        StartThread();
    }
    object lockd = new object();
    void goThread()
    {
        int index = 0;
        while (true)
        {
            lock (lockd)//防止其他线程访问当前线程使用的数据
            {
                Debug.Log("in thread" + index);
                index++;
                if (index == 100)
                {
                    Thread.Sleep(1000);//   将当前线程挂起指定的时间 毫秒  时间结束后 继续执行下一步  和yield类似
                }
                else if (index == 200)
                {
                    break;//该函数执行完自动结束该线程
                }
            }
        }
    }
}

