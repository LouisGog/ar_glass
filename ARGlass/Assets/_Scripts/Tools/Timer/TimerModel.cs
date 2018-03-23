using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public delegate void TimeDelegate();

public class TimerModel  {

	public int Id;

	/// <summary>
	/// 任务执行的事件
	/// </summary>
	public long Time;

	private TimeDelegate timeDelegate;

	public TimerModel(int id, long time,TimeDelegate td)
	{
		this.Id = id;
		this.Time = time;
		this.timeDelegate = td;
	}


	public void Run()
	{
		timeDelegate ();
	}
}
