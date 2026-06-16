using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// LeveState class define how deficult level is using Enum and this is public so that every class can access it and manupate it
/// </summary>
public enum Level
{
    Level1,
    Level2,
    Level3,
    Level4,
    Level5,
    Level6,
    Level7,
    Level8
}
public class LevelState : MonoBehaviour
{
    public Level level;
    public string SaveKey =>
    level.ToString();
}
