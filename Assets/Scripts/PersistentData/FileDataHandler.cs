using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

//code by Trevor Mock, How to make a Save & Load System in Unity - https://youtu.be/aUi9aijvpgs
public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";


    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }


    public EGXData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        EGXData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                using(FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<EGXData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured while loading data to file: " + fullPath + "\n" + e);
            }
        }

        return loadedData;
    }

    public void Save(EGXData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using(FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using(StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError("Error occured while saving data to file: " + fullPath + "\n" + e);
        }
    }
}
