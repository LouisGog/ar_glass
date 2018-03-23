using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System;

public class TimerManager  {


	private static TimerManager instance;
	public static TimerManager Instance
	{
		get
		{
			// 当多线程时，可以用lock锁  lock(intstance)
			if (instance == null) {
				instance = new TimerManager ();
			}
			return instance;
		}

	}

	/// <summary>
	/// 实现定时器的主要功能就是这个Timer类
	/// </summary>
	private Timer timer;

    /// <summary>
    /// 这个字典存储： 任务id 和 任务模型 的映射     TODO:  不能引入System.Collections.Concurrent，很奇怪？？？？
    /// </summary>
    private Dictionary<int, TimerModel> idModelDict = new Dictionary<int, TimerModel>();


    /// <summary>
    /// 要移除的任务id列表
    /// </summary>
    private List<int> removeList = new List<int>();

    /// <summary>
    /// 用来表示ID
    /// </summary>
    private int id;

	public TimerManager()
	{
        // timer类的构造可以使用时间 ，单位是毫秒
		timer = new Timer ();
        timer.Elapsed += Timer_Elapsed;
	}

    /// <summary>
    /// 到达时间间隔时触发
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {

       // TimerModel model = null;
       //lock(removeList)
       // {

       // }
        foreach(var id in removeList)
        {
            idModelDict.Remove(id);
        }
        removeList.Clear();

        foreach (var model in idModelDict.Values)
        {

            if(model.Time <= DateTime.Now.Ticks)
            model.Run();
        }
    }

    /// <summary>
    /// 添加定时任务   指定触发的时间  2017年11月28号
    /// </summary>
    public void AddTimeEvent(DateTime dateTime,TimeDelegate timeDelegate)
    {
        long delayTime = dateTime.Ticks - DateTime.Now.Ticks;
        if (delayTime <= 0)
            return;
        AddTimeEvent(delayTime,timeDelegate);
    }

    /// <summary> 
    ///  添加定时任务  指定延时的时间 40s
    /// </summary>
    /// <param name="delayTime">毫秒</param>
    /// <param name="timeDelegate"></param>
    public void AddTimeEvent(long delayTime, TimeDelegate timeDelegate)
    {
        // 可以转换时间单位
        TimerModel model = new TimerModel(id++,DateTime.Now.Ticks+delayTime,timeDelegate);
        idModelDict.Add(model.Id,model);
    }
}
