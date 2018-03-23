using System;
using System.Threading;
using System.Collections.Generic;

public class ConcurrentInteger
{
    int value;
    Mutex tex = new Mutex();

    public ConcurrentInteger() { }
    public ConcurrentInteger(int value)
    {
        lock (this)
        {
            this.value = value;
        }
    }

    /// <summary>
    /// 自增并返回值
    /// </summary>
    /// <returns></returns>
    public int GetAndAdd()
    {
        lock (this)
        {
            tex.WaitOne();
            value++;
            tex.ReleaseMutex();
            return value;
        }
    }

    /// <summary>
    /// 自减并返回值
    /// </summary>
    /// <returns></returns>
    public int GetAndReduce()
    {
        lock (this)
        {
            tex.WaitOne();
            value--;
            tex.ReleaseMutex();
            return value;
        }
    }

    /// <summary>
    /// 重制value为0
    /// </summary>
    public void Reset()
    {
        lock (this)
        {
            tex.WaitOne();
            value = 0;
            tex.ReleaseMutex();
        }
    }

    public int Get()
    {
        return this.value;
    }
}

