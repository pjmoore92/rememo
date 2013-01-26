/// <summary>
/// This interface is implemented by classes which provide some
/// reminder management service, and allows the delivery of
/// reminders by an observer callback pattern.
/// </summary>
public interface ReminderObservable
{
    /// <summary>
    /// Register an observer with the reminder service. The
    /// observer will be notified whenever a new reminder is
    /// due for delivery.
    /// </summary>
    /// 
    /// <param name="o">Observer to be registered.</param>
    /// 
    /// <returns>True if registered successfully.</returns>
    bool RegisterObserver(ReminderObserver o);

    /// <summary>
    /// Deregister an observer from the reminder service.
    /// This observer will no longer receive reminder updates.
    /// </summary>
    /// 
    /// <param name="o">Observer to be deregistered.</param>
    void DeregisterObserver(ReminderObserver o);

    /// <summary>
    /// Asks the reminder service to add the given reminder.
    /// Depending on policy, the service may ignore any
    /// external requests.
    /// </summary>
    /// 
    /// <param name="r">Reminder to be added.</param>
    /// 
    /// <returns>True if added successfully.</returns>
    bool AddReminder(Reminder r);

    /// <summary>
    /// Asks the reminder service to add the given reminders.
    /// Depending on policy, the service may ignore any
    /// external requests.
    /// </summary>
    /// 
    /// <param name="reminders">Reminders to be added.</param>
    /// 
    /// <returns>True if added successfully.</returns>
    bool AddReminders(ReminderCollection reminders);

    /// <summary>
    /// Asks the reminder service to remove the given
    /// reminder from the service. Depending on policy,
    /// the service may ignore any external requests.
    /// </summary>
    /// 
    /// <param name="r">Reminder to be removed.</param>
    /// 
    /// <returns>True if removed successfully.</returns>
    bool RemoveReminder(Reminder r);

    /// <summary>
    /// Queries the service to see if a reminder is being
    /// managed by the system.
    /// </summary>
    /// 
    /// <param name="r">Reminder to check for.</param>
    /// 
    /// <returns>True if the reminder exists.</returns>
    bool HasReminder(Reminder r);
}