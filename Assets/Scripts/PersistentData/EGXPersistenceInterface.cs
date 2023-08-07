using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//code by Trevor Mock, How to make a Save & Load System in Unity - https://youtu.be/aUi9aijvpgs
public interface EGXPersistenceInterface
{
    void LoadData(EGXData data);        //read EGXData.cs scores
    void SaveData(EGXData data);    //write EGXData.cs scores
}
