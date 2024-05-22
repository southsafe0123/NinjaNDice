using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Minigame : MonoBehaviour
{
    public new string name;
    public float time;
    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> bonusItems = new List<GameObject>();
    public bool hasBeenPlayedLastTurn;
}
