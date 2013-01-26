using System;
using System.IO;
using System.Runtime.Serialization;
using Rememo;

public class DiaryEntrySerializer
{
    public static DiaryEntryCollection DeserializeDiaryEntryCollection(string filename)
    {
        if (!FileManager.Exists(filename))
        {
            // No collection exists - initialise a new one now
            return new DiaryEntryCollection(DateTime.Now);
        }

        DataContractSerializer x = new DataContractSerializer(typeof(DiaryEntryCollection));

        FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        DiaryEntryCollection collection = x.ReadObject(fs) as DiaryEntryCollection;
        fs.Close();

        return collection;
    }

    public static void SerializeDiaryEntryCollection(DiaryEntryCollection collection, string filename)
    {
        DataContractSerializer x = new DataContractSerializer(typeof(DiaryEntryCollection));

        FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
        x.WriteObject(fs, collection);

        fs.Close();
    }
}