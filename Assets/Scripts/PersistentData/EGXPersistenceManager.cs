using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

//code by Trevor Mock, How to make a Save & Load System in Unity - https://youtu.be/aUi9aijvpgs
public class EGXPersistenceManager : MonoBehaviour
{
    //script references
    private EGXData data;
    private FileDataHandler dataHandler;

    //objects using a script that references EGX Persistence Interface
    private List<EGXPersistenceInterface> dataPersistenceObjects;
    public static EGXPersistenceManager instance { get; private set; }

    //what to call the data file
    [SerializeField] private string fileName;

    private void Awake()
    {
        //if there's a duplicate instance of the persistance manager, destroy it
        if(instance != null)
        {
            if (PlayerData.GetDevMode()) { Debug.Log("duplicate PersistenceManager found"); }
            instance = null;
        }

        //set instance to this
        instance = this;
    }

    private void Start()
    {
        //Debug.Log("game started");
        if (PlayerData.GetDevMode()) { Debug.Log("Data path: " + Application.persistentDataPath); }
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAlldataPersistenceObjects();
        LoadGame();
    }

    private List<EGXPersistenceInterface> FindAlldataPersistenceObjects()
    {
        //find all objects referencing EGXPersistenceInterface
        IEnumerable<EGXPersistenceInterface> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<EGXPersistenceInterface>();
        return new List<EGXPersistenceInterface>(dataPersistenceObjects);
    }


    public void NewGame() //rename ResetScore or ResetData
    {
        if (PlayerData.GetDevMode()) { Debug.Log("new game"); }
        this.data = new EGXData();
    }


    public void LoadGame() //rename LoadScore or LoadData
    {
        if (PlayerData.GetDevMode()) { Debug.Log("EGX PersitenceManager script load data"); }
        if (PlayerData.GetDevMode()) { Debug.Log("loading game state"); }
        this.data = dataHandler.Load();

        if(this.data == null)
        {
            if (PlayerData.GetDevMode()) { Debug.Log("no egx data found"); }
            NewGame();
        }

        //update each script referencing EGXPersistenceInterface with relevant game data
        foreach (EGXPersistenceInterface dataPersistenceOBJ in dataPersistenceObjects){ dataPersistenceOBJ.LoadData(data); }
        if (PlayerData.GetDevMode()) { Debug.Log("game state loaded"); }

        if (PlayerData.GetDevMode())
        {
            for (int i = 0; i < data.playerScores.Length; i++)
            {
                Debug.Log("Loaded pos" + i + " - Score: " + data.playerScores[i] + ", TSLK: " + data.playerTSLKs[i] + ", spriteID: " + data.playerSpriteIDs[i]);
            }

            for (int i = 0; i < data.storedPositions; i++)
            {
                Debug.Log("Loaded pos" + i + " - Score: " + data.storedScores[i] + ", TSLK: " + data.storedTSLKs[i]);
            }
        }
    }


    public void SaveGame() //rename SaveScore or SaveData
    {
        if (PlayerData.GetDevMode()) { Debug.Log("EGX PersitenceManager script save data"); }
        if (PlayerData.GetDevMode()) { Debug.Log("saving game state"); }
        //update EGXData with each script referencing EGXPersistenceInterface relevant game data
        foreach (EGXPersistenceInterface dataPersistenceOBJ in dataPersistenceObjects) { dataPersistenceOBJ.SaveData(data); }


        if (PlayerData.GetDevMode())
        {
            for (int i = 0; i < data.playerScores.Length; i++)
            {
                Debug.Log("Saved pos" + i + " - Score: " + data.playerScores[i] + ", TSLK: " + data.playerTSLKs[i] + ", spriteID: " + data.playerSpriteIDs[i]);
            }

            for (int i = 0; i < data.storedPositions; i++)
            {
                Debug.Log("Saved pos" + i + " - Score: " + data.storedScores[i] + ", TSLK: " + data.storedTSLKs[i]);
            }
        }

        dataHandler.Save(data);
    }


    private void OnApplicationQuit()
    {
        if (PlayerData.GetDevMode()) { Debug.Log("game closing"); }
        SaveGame();
    }

    private void OnDestroy()
    {
        if (PlayerData.GetDevMode()) { Debug.Log("EGX persistence manager destroyed"); }
        SaveGame();
    }
}
