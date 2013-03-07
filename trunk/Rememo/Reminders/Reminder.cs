using System;
using System.Runtime.Serialization;

/// <summary>
/// This class represents a reminder.
/// </summary>
[DataContract]
public class Reminder
{
    private string description;
    private DateTime time;
    private bool delivered;
    private bool done;
    private ReminderPriority priority;
    private DiaryEntry entry;
    private bool abs;
    private bool not;
    private bool tag;
    private bool mus;

    public Reminder()
        : this("", new DateTime(), false, false, ReminderPriority.Low, null, false, false, false, false)
    {
    }

    public Reminder(string description, DateTime time, bool delivered, bool done, ReminderPriority priority, DiaryEntry entry, bool abs, bool mus, bool tag, bool not)
    {
        Description = description;
        Time = time;
        Delivered = delivered;
        Done = done;
        this.priority = priority;
        this.entry = entry;
        Abs = abs;
        Not = not;
        Tag = tag;
        Mus = mus;

        
    }

    [DataMember]
    public string Description
    {
        get { return description; }
        set { description = value; }
    }

    [DataMember]
    public DateTime Time
    {
        get { return time; }
        set { time = value; }
    }

    [DataMember]
    public bool Delivered
    {
        get { return delivered; }
        set { delivered = value; }
    }

    [DataMember]
    public bool Done
    {
        get { return done; }
        set { done = value; }
    }

    [DataMember]
    public ReminderPriority Priority
    {
        get { return priority; }
        set { priority = value; }
    }

    [IgnoreDataMember]
    public DiaryEntry Entry
    {
        get { return entry; }
        set { entry = value; }
    }

    [DataMember]
    public bool Abs
    {
        get { return abs; }
        set { abs = value; }
    }

    [DataMember]
    public bool Not
    {
        get { return not; }
        set { not = value; }
    }
    
    [DataMember]
    public bool Tag
    {
        get { return tag; }
        set { tag = value; }
    }

    [DataMember]
    public bool Mus
    {
        get { return mus; }
        set { mus = value; }
    }

    /// <summary>
    /// Delay this reminder by the given amount.
    /// </summary>
    public void Delay(TimeSpan offset)
    {
        time = DateTime.Now.Add(offset);
        delivered = false;
        done = false;

        Console.WriteLine("Delayed reminder by " + offset.TotalSeconds + " seconds");
    }

    public override string ToString()
    {
        return String.Format("{0} [{1}] ({2} {3}) {4}", description, done ? "X" : " ", time.ToShortDateString(), time.ToLongTimeString(), priority);
    }
}