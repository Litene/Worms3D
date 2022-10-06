using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static GameObject GetCorrectPrefab(PlayerColor color) { 
        switch (color) {
            case PlayerColor.Blue:
                return Resources.Load("BluePlayer") as GameObject;
            case PlayerColor.Purple: 
                return Resources.Load("PurplePlayer") as GameObject;
            case PlayerColor.Red:
                return Resources.Load("RedPlayer") as GameObject;
            default:
                return Resources.Load("YellowPlayer") as GameObject;
        }
    }

    public static Transform GetCorrectSpawnParent(PlayerColor color) {
        switch (color) {
            case PlayerColor.Blue:
                 return GameObject.Find("BluePlayerWorms").transform;
            case PlayerColor.Purple: 
                 return GameObject.Find("PurplePlayerWorms").transform;
            case PlayerColor.Red:
                 return GameObject.Find("RedPlayerWorms").transform;
            default:
                 return GameObject.Find("YellowPlayerWorms").transform;
        }
    }

    public static List<Worm> GetCorrectsWorms(PlayerColor color) {
        List<Worm> wormObjects = new List<Worm>();
        foreach (Transform children in Utility.GetCorrectSpawnParent(color)) {
            wormObjects.Add(children.GetComponent<Worm>());
        }
        return wormObjects;
    }
}
