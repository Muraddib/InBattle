using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using SimpleJSON;
using System;

[Serializable]
public class Battle
{
    public int id;
    public int seed;
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
    public Array variations;
    public Array essences;
}
