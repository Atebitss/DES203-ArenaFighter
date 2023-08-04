using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//code by Trevor Mock, How to make a Save & Load System in Unity - https://youtu.be/aUi9aijvpgs
public class EGXPersistenceManager : MonoBehaviour
{
    private EGXData data;
    private List<EGXPersistenceInterface> dataPersistenceObjects;
    public static EGXPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("duplicate PersistenceManager found");
            instance = null;
        }

        instance = this;
    }

    private void Start()
    {
        Debug.Log("game stated");
        this.dataPersistenceObjects = FindAlldataPersistenceObjects();
        LoadGame();
    }

    private List<EGXPersistenceInterface> FindAlldataPersistenceObjects()
    {
        //find all objects referencing EGXPersistenceInterface
        IEnumerable<EGXPersistenceInterface> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<EGXPersistenceInterface>();
        return new List<EGXPersistenceInterface>(dataPersistenceObjects);
    }


    private void OnApplicationQuit()
    {
        Debug.Log("game closing");
        SaveGame();
    }


    public void NewGame() //rename ResetScore or ResetData
    {
        Debug.Log("new game");
        this.data = new EGXData();
    }


    public void LoadGame() //rename LoadScore or LoadData
    {
        Debug.Log("loading game state");
        if(this.data == null)
        {
            Debug.Log("no egx data found");
            NewGame();
        }

        //update each script referencing EGXPersistenceInterface with relevant game data
        foreach (EGXPersistenceInterface dataPersistenceOBJ in dataPersistenceObjects){ dataPersistenceOBJ.LoadData(data); }


        for(int i = 0; i < data.playerScores.Length; i++)
        {
            Debug.Log("Loaded pos" + i + " - Score: " + data.playerScores[i] + ", TSLK: " + data.playerTSLKs[i] + ", spriteID: " + data.playerSpriteIDs[i]);
        }
    }


    public void SaveGame() //rename SaveScore or SaveData
    {
        Debug.Log("saving game state");
        //update EGXData with each script referencing EGXPersistenceInterface relevant game data
        foreach (EGXPersistenceInterface dataPersistenceOBJ in dataPersistenceObjects) { dataPersistenceOBJ.SaveData(ref data); }


        for (int i = 0; i < data.playerScores.Length; i++)
        {
            Debug.Log("Saved pos" + i + " - Score: " + data.playerScores[i] + ", TSLK: " + data.playerTSLKs[i] + ", spriteID: " + data.playerSpriteIDs[i]);
        }
    }
}
