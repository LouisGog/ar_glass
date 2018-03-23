using System;

public class TimeTaskModel
{
    /// <summary>
    /// 任务逻辑
    /// </summary>
    private TimeDelegate execut;
    /// <summary>
    /// 任务执行的时间间隔
    /// </summary>
    public long timeInterval;
    /// <summary>
    /// 下一个任务执行的时间
    /// </summary>
    public long time;
    /// <summary>
    /// 任务ID
    /// </summary>
    public int id;
    /// <summary>
    /// 是否循环
    /// </summary>
    public bool isLoop;

    public TimeTaskModel(int id, TimeDelegate execut, long delay, bool isLoop)
    {
        this.id = id;
        this.execut = execut;
        this.timeInterval = delay;
        this.time = DateTime.Now.Ticks + delay;
        this.isLoop = isLoop;
    }

    public void Run()
    {
        execut();
        this.time = DateTime.Now.Ticks + timeInterval;
    }
}