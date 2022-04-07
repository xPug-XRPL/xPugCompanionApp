using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Persistent_Game_Controller : MonoBehaviour
{
    [Header("Persistent Game Attributes")]
    [SerializeField] private CosmeticInventory _LocalCosmeticInventory;
    [SerializeField] private List<CosmeticIcon> cosmeticIconDatabase;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SaveCosmeticDataToFile();
            Debug.Log("Writing to file!");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            LoadCosmeticDataFromFile();
            Debug.Log("Reading from file!");
        }
    }

    public void AddToCosmeticInventory(CosmeticName name, int quantity, string description, CosmeticType type)
    {
        CosmeticData newData = new CosmeticData();
        newData.name = name;
        newData.quantity = quantity;
        newData.description = description;
        newData.type = type;

        bool noClone = true;

        // Check if item already exists, if so just add to quantity
        foreach (CosmeticData data in _LocalCosmeticInventory.cosmeticInventory)
        {
            if (data.name == name && data.description == description)
            {
                data.quantity += quantity;
                noClone = false;
            }
        }

        if (noClone)
        {
            _LocalCosmeticInventory.cosmeticInventory.Add(newData);
        }
        else
        {
            Debug.Log("Item was already found in CosmeticInventory, adding " + quantity + " to quantity instead!");
        }
    }

    public void ModifyCosmeticInventory(CosmeticName nameToModify, CosmeticName newName, int newQuantity, string newDescription, CosmeticType newType)
    {
        foreach (CosmeticData data in _LocalCosmeticInventory.cosmeticInventory)
        {
            if (data.name == nameToModify)
            {
                data.name = newName;
                data.quantity = newQuantity;
                data.description = newDescription;
                data.type = newType;
            }
            else
            {
                Debug.Log("No such item to modify!");
            }
        }
    }

    public void RemoveFromCosmeticInventory(CosmeticName nameToDelete)
    {
        CosmeticInventory tempModifiedCosmeticInventory = _LocalCosmeticInventory;

        foreach (CosmeticData data in tempModifiedCosmeticInventory.cosmeticInventory)
        {
            if (data.name == nameToDelete)
            {
                tempModifiedCosmeticInventory.cosmeticInventory.Remove(data);

                _LocalCosmeticInventory = tempModifiedCosmeticInventory;
                return;
            }
            else
            {
                Debug.Log("No such item to remove!");
            }
        }
    }

    public void SaveCosmeticDataToFile()
    {
        string tempSaveString = "";

        string formattedData = JsonUtility.ToJson(_LocalCosmeticInventory);
        tempSaveString += formattedData;

        System.IO.File.WriteAllText(Application.persistentDataPath + "/CosmeticData.json", tempSaveString);
    }

    public void LoadCosmeticDataFromFile()
    {
        try
        {
            string jsonFile = File.ReadAllText(Application.persistentDataPath + "/CosmeticData.json");

            CosmeticInventory cosmeticsFromJson = JsonUtility.FromJson<CosmeticInventory>(jsonFile);

            _LocalCosmeticInventory = cosmeticsFromJson;
        }
        catch (Exception e)
        {
            print("No CosmeticData file to load! Did the player save?");
        }
    }

    public CosmeticInventory GetCosmeticInventory()
    {
        Debug.Log("Returning CosmeticInventory!");
        return _LocalCosmeticInventory;
    }

    public Sprite GetCosmeticIcon(CosmeticName referenceName)
    {
        Sprite returnIcon = cosmeticIconDatabase[0].icon;

        for (int iconCount = 0; iconCount < cosmeticIconDatabase.Count; iconCount++)
        {
            CosmeticIcon tempIcon = cosmeticIconDatabase[iconCount];
            if (tempIcon.referenceName == referenceName)
            {
                returnIcon = tempIcon.icon;
            }
        }

        return returnIcon;
    }
}

[Serializable]
public class CosmeticInventory
{
    public List<CosmeticData> cosmeticInventory;
}

[Serializable]
public class CosmeticData
{
    public CosmeticName name;
    public int quantity;
    public string description;
    public CosmeticType type;
}

[Serializable]
public class CosmeticIcon
{
    public CosmeticName referenceName;
    public Sprite icon;
}

public enum CosmeticName { Empty, ArmyHelmet, ThreeDGlasses, Hammer, Welt, Burger, TopHat, NoseRing, ChefHat, Knife, NinjaMask, PhotoHat, HeartGlasses, BucketHat, Bucket}

public enum CosmeticType { Empty, Hat, Face, Body, Leg, Tail }
