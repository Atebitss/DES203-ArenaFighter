using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EGXPersistenceInterface
{
    void LoadData(EGXData data);        //read EGXData.cs scores
    void SaveData(EGXData data);    //write EGXData.cs scores
}
