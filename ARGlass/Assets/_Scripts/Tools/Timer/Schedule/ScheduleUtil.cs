using System;
using System.Timers;
using System.Collections.Generic;

/// <summary>
/// 计时任务工具类
/// 例： ScheduleUtil.Instance.Schedule(一个委托, 时间间隔, 是否循环);
/// </summary>
public class ScheduleUtil
{
    private static ScheduleUtil _scheduleUtil;
    public static ScheduleUtil Instance
    {
        get
        {
            if (_scheduleUtil == null)
            {
                _scheduleUtil = new ScheduleUtil();
            }
            return _scheduleUtil;
        }
    }

    private ScheduleUtil()
    {
        timer = new Timer(100);
        timer.Elapsed += CallBack;
        timer.Start();
    }

    private Timer timer; //定时器
    private ConcurrentInteger index = new ConcurrentInteger(); //自增器

    /// <summary>
    /// 等待执行的任务表
    /// </summary>
    private Dictionary<int, TimeTaskModel> mission = new Dictionary<int, TimeTaskModel>();
    /// <summary>
    /// 等待移除的任务列表
    /// </summary>
    private List<int> removeList = new List<int>();

    void CallBack(object sender, ElapsedEventArgs e)
    {
        lock (mission)
        {
            lock (removeList)
            {
                foreach (int item in removeList)
                {
                    mission.Remove(item);
                }
                removeList.Clear();
                foreach (TimeTaskModel item in mission.Values)
                {
                    if (item.time <= DateTime.Now.Ticks)
                    {
                        item.Run();
                        if (!item.isLoop)
                            removeList.Add(item.id);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 任务调用 毫秒
    /// </summary>
    /// <param name="task">任务委托</param>
    /// <param name="delay">时间间隔</param>
    /// <param name="isLoop">是否循环</param>
    /// <returns>任务ID</returns>
    public int Schedule(TimeDelegate task, long delay, bool isLoop = false)
    {
        //毫秒 转 100倍纳秒
        return ScheduleMms(task, delay / 1000 * 10000000, isLoop);
    }

    public int Schedule(TimeDelegate task, DateTime time, bool isLoop = false)
    {
        long t = time.Ticks - DateTime.Now.Ticks;
        t = Math.Abs(t);
        return ScheduleMms(task, t, isLoop);
    }

    public int TimeSchedule(TimeDelegate task, long time, bool isLoop = false)
    {
        long t = time - DateTime.Now.Ticks;
        t = Math.Abs(t);
        return ScheduleMms(task, t, isLoop);
    }

    /// <summary>
    /// 100倍纳秒级时间轴
    /// </summary>
    private int ScheduleMms(TimeDelegate task, long delay, bool isLoop)
    {
        lock (mission)
        {
            int id = index.GetAndAdd();
            TimeTaskModel model = new TimeTaskModel(id, task, delay, isLoop);
            mission.Add(id, model);
            return id;
        }
    }

    public void RemoveMission(int id)
    {
        lock (removeList)
        {
            removeList.Add(id);
        }
    }
}

public delegate void TimeDelegate();
