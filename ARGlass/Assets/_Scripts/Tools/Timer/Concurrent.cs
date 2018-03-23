using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// 线程安全的int类型
/// </summary>
public class Concurrent  {

    private int value;

    public Concurrent(int value)
    {
        this.value = value;
    }

    /// <summary>
    /// 添加并获取
    /// </summary>
    /// <returns></returns>
    public int Add_Get()
    {
        lock(this)
        {
            value++;
            return value;
        }
    }


    /// <summary>
    /// 减少并获取
    /// </summary>
    /// <returns></returns>
    public int Reduce_Get()
    {
        lock (this)
        {
            value--;
            return value;
        }
    }

    /// <summary>
    /// 获取
    /// </summary>
    /// <returns></returns>
    public int Get()
    {
      return value;
    }

}
