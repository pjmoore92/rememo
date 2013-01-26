using System.Collections.Generic;

﻿/// <summary>
/// This interface allows a class to register itself to
/// receive notification that a reminder is due to be
/// delivered.
/// </summary>
public interface ReminderObserver
{
    /// <summary>
    /// Callback function for the observable object to
    /// notify that a reminder is due for delivery.
    /// </summary>
    /// 
    /// <param name="reminders">Reminders to be delivered.</param>
    void DeliverReminders(List<Reminder> reminders);
}
