﻿using System.IO;
using System.Runtime.Serialization;

/// <summary>
/// This class allows Reminder objects to be serialized and
/// deserialized from XML representations.
/// </summary>
public class ReminderSerializer
{
    /// <summary>
    /// Construct a set of Reminders from an XML file. It is assumed
    /// that the filename path is correctly constructed.
    /// </summary>
    /// 
    /// <param name="filename">Path to the XML file.</param>
    /// 
    /// <returns>Reminders constructed from serialized version.</returns>
    public static ReminderCollection DeserializeReminders(string filename)
    {
        DataContractSerializer x = new DataContractSerializer(typeof(ReminderCollection));

        FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        ReminderCollection reminders = x.ReadObject(fs) as ReminderCollection;
        fs.Close();

        return reminders;
    }

    /// <summary>
    /// Serializes a set of Reminders to an XML file.
    /// </summary>
    /// 
    /// <param name="filename">Desired filename of XML file. Will overwrite if exists.</param>
    public static void SerializeReminders(ReminderCollection reminders, string filename)
    {
        DataContractSerializer x = new DataContractSerializer(typeof(ReminderCollection));

        FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
        x.WriteObject(fs, reminders);

        fs.Close();
    }
}
