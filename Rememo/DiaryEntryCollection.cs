using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

[DataContract]
public class DiaryEntryCollection : IEnumerable<DiaryEntry>
{
    public const String DIARY_COLLECTION_FILE = "diaryEntryCollection.xml";

    private List<DiaryEntry> collection;
    //private List<DiaryNote> notes;
    private DateTime startDate;

    public DiaryEntryCollection()
    {
        collection = new List<DiaryEntry>();
        //notes = new List<DiaryNote>();
    }

    public DiaryEntryCollection(DateTime startDate)
    {
        this.startDate = startDate;
        collection = new List<DiaryEntry>();
        //notes = new List<DiaryNote>();
    }

    #region IEnumerable
    public IEnumerator<DiaryEntry> GetEnumerator()
    {
        return (IEnumerator<DiaryEntry>)collection.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
    #endregion

    #region Fields
    [DataMember]
    public DateTime StartDate
    {
        get { return startDate; }
        set { startDate = value; }
    }

    [DataMember]
    public List<DiaryEntry> Collection
    {
        get { return collection; }
        set { collection = value; }
    }

    //[DataMember]
    /*public List<DiaryNote> Notes
    {
        get { return notes; }
        set { notes = value; }
    }*/
    #endregion

    #region Diary notes
    /// <summary>
    /// Adds a new note to the collection, updating an existing one
    /// if a note already exists for that week and row.
    /// </summary>
   /* public void AddNote(DiaryNote note)
    {
        DiaryNote existing = GetNoteForWeek(note.Week, note.Row);

        if (existing == null)
            notes.Add(note);
        else
        {
            existing.Text = note.Text;
        }

        Save();
    }

    /// <summary>
    /// Returns any existing note for the given week and row.
    /// </summary>
    public DiaryNote GetNoteForWeek(int week, int row)
    {
        foreach (DiaryNote note in notes)
            if (week == note.Week && row == note.Row)
                return note;

        return null;
    }*/

    /// <summary>
    /// Returns any existing notes for the given week.
    /// </summary>
    /*public IEnumerable<DiaryNote> GetNotesForWeek(int week)
    {
        return from note in notes
               where note.Week == week
               select note;
    }*/
    #endregion

    #region Diary entries
    /// <summary>
    /// Adds a new entry to the collection, updating any existing
    /// diary entry for the selected date and time.
    /// </summary>
    public void AddEntry(DiaryEntry entry)
    {
        DiaryEntry existing = GetEntry(entry.Date, entry.Time);

        if (existing == null)
        {
            collection.Add(entry);
        }
        else
        {
            existing.Text = entry.Text;
            existing.Underline = entry.Underline;
            existing.Asterisk = entry.Asterisk;
            existing.Circle = entry.Circle;
            //existing.Reminder = entry.Reminder;
        }

        Save();
    }

    /// <summary>
    /// Gets all diary entries from the given date, if any exist.
    /// </summary>
    public IEnumerable<DiaryEntry> GetEntriesOnDate(DateTime dt)
    {
        return from entry in collection
               where entry.Date.Day == dt.Day && entry.Date.Month == dt.Month && entry.Date.Year == dt.Year
               select entry;
    }

    /// <summary>
    /// Searches for a diary entry at the given date and time.
    /// </summary>
    public DiaryEntry GetEntry(DateTime date, TimeOfDay time)
    {
        IEnumerable<DiaryEntry> entries = GetEntriesOnDate(date);

        foreach (DiaryEntry entry in entries)
        {
            if (entry.Time == time)
                return entry;
        }

        return null;
    }

    /// <summary>
    /// Returns true if an entry already exists at the given date and time.
    /// </summary>
    public bool EntryExists(DateTime date, TimeOfDay time)
    {
        return GetEntry(date, time) != null;
    }
    #endregion

    #region Saving and loading diary entries
    public void Save()
    {
        DiaryEntrySerializer.SerializeDiaryEntryCollection(this, DIARY_COLLECTION_FILE);
    }

    public static DiaryEntryCollection Load()
    {
        return DiaryEntrySerializer.DeserializeDiaryEntryCollection(DIARY_COLLECTION_FILE);
    }
    #endregion
}
