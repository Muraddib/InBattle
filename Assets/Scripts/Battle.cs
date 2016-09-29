using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using SimpleJSON;
using System;

[Serializable]
public class BattleData
{
    public Battle battle;
}

[Serializable]
public class Battle
{
    public int id;
    //public int seed;
    public string status;
    public int number;
    public byte min_lvl;
    public byte max_lvl;
    public int width;
    public int height;
    public int round;
    public int start_time;
    public int round_time;
    public int updated_at;
    public List<object> variations;
    public List<Essence> essences;
}

[Serializable]
public class Essence
{
    public int id;
    public string kind;
    //public int seed;
    public Gex gex;
}

[Serializable]
public class Gex
{
    public int x;
    public int y;
}


