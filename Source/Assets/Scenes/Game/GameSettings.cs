using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Race_GameMode 
{
    Racing
}

public class GameSettings 
{
    public Race_GameMode Mode;
    public int TimeLimit = 0;
    public int LapLimit = 3;

    public GameSettings(Race_GameMode mode) 
    {
        Mode = mode;
    }    
}
