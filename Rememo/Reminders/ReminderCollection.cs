using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;

/// <summary>
/// This class represents a collection of reminders which are
/// managed and delivered using an observer pattern.
/// </summary>
[DataContract]
public class ReminderCollection : ReminderObservable, IEnumerable<Reminder>
{
    // Update interface in seconds
    private const int UPDATE_INTERVAL = 20;

    private List<ReminderObserver> observers;
    private Thread reminderClock;

    [DataMember]
    protected List<Reminder> reminders;
     

    public ReminderCollection()
    {
        observers = new List<ReminderObserver>();
        reminders = new List<Reminder>();
    }

    public List<Reminder> returnReminders{
        get { return reminders; }
    }  

    public void TESTDeliverNext()
    {
        List<Reminder> next = new List<Reminder>();

        foreach (Reminder reminder in reminders)
        {
            if (reminder.Delivered == false && reminder.Done == false && (next == null || next.Count == 0 || reminder.Time.Ticks < next[0].Time.Ticks))
                next.Add(reminder);
        }

        if (next != null)
        {
            foreach (ReminderObserver observer in observers)
            {
                observer.DeliverReminders(next);
            }
        }
    }

    #region Enumerable
    public IEnumerator<Reminder> GetEnumerator()
    {
        return (IEnumerator<Reminder>)reminders.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
    #endregion

    #region Clock
    /// <summary>
    /// Start the reminder manager process.
    /// </summary>
    public void Start()
    {
        reminderClock = new Thread(ReminderClock);
        reminderClock.Name = "Reminder clock";
        reminderClock.Start();
    }

    /// <summary>
    /// Halt the reminder manager process.
    /// </summary>
    public void Stop()
    {
        reminderClock.Interrupt();
    }
    #endregion

    #region ReminderObservable
    public bool RegisterObserver(ReminderObserver o)
    {
        lock (observers)
        {
            observers.Add(o);
            return true;
        }
    }

    public void DeregisterObserver(ReminderObserver o)
    {
        lock (observers)
        {
            observers.Remove(o);
        }
    }

    public bool AddReminder(Reminder r)
    {
        lock (reminders)
        {
            if (r != null)
            {
                reminders.Add(r);
                return true;
            }
            else
                return false;
        }
    }

    public bool AddReminders(ReminderCollection reminders)
    {
        lock (this.reminders)
        {
            this.reminders.AddRange(reminders);
            return true;
        }
    }

    public bool RemoveReminder(Reminder r)
    {
        lock (reminders)
        {
            if (r != null)
                return reminders.Remove(r);
        }

        return false;
    }

    public bool HasReminder(Reminder r)
    {
        lock (reminders)
        {
            return reminders.Contains(r);
        }
    }
    #endregion

    #region ReminderClock
    /// <summary>
    /// This function should execute in its own thread, and will
    /// notify observers at an appropriate time when a reminder is
    /// due to be delivered.
    /// </summary>
    private void ReminderClock()
    {
        List<Reminder> toDeliver = new List<Reminder>();

        while (true)
        {
            DateTime now = DateTime.Now;

            #region FindDueReminders
            // Find reminders due for delivery
            lock (reminders)
            {
                foreach (Reminder r in reminders)
                {
                    DateTime due = r.Time;

                    /* Check if reminder is due. */
                    if (!r.Delivered && due.Ticks <= now.Ticks)
                        toDeliver.Add(r);
                }
            }
            #endregion

            if (reminders.Count == 0)
                Console.WriteLine("No reminders left");
            else
            {
                Console.WriteLine("\nShowing reminders...");
                

                foreach (Reminder reminder in reminders)
                {
                    Console.WriteLine(reminder.ToString());
                }

                Console.WriteLine("\n");
            }

            #region Diagnostics
            foreach (Reminder r in toDeliver)
            {
                Console.WriteLine(String.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), r.ToString()));
            }
            #endregion

            #region DeliverReminders
            /* 
             * Remove these reminders from the collection of
             * reminders and add their next (if any) recurring
             * reminder.
             */
            lock (reminders)
            {
                foreach (Reminder r in toDeliver)
                {
                    r.Delivered = true;
                }

                /* Deliver the reminders to all observers. */
                lock (observers)
                {
                    foreach (ReminderObserver o in observers)
                    {
                        o.DeliverReminders(toDeliver);
                    }
                }
            }
            #endregion

            toDeliver.Clear();

            try
            {
                Thread.Sleep(UPDATE_INTERVAL * 1000);
            }
            catch (ThreadInterruptedException e)
            {
                return;
            }
        }
    }
    #endregion
}