using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using SimpleJSON;

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
    public List<string> variations;
    public List<Essence> essences;
    public List<object> teams;
}

[Serializable]
public class Essence
{
    public int id;
    //public int seed;
    public Gex gex;
    public string kind;
    public string size;
    public double exp;
    public string status;
    public string result;
    public List<string> modifiers;
    //public object log_data;
    public object team_index;
    public BattlerInfo info;
    public List<Gex> gexes;
    public int solidity;
    public int injury;
}

[Serializable]
public class BattlerInfo
{
    public int id;
    public string name;
    public string race;
    public string gender;
    public int level;
    public int head_def;
    public int body_def;
    public int right_hand_def;
    public int left_hand_def;
    public int legs_def;
    public string head_status;
    public string body_status;
    public string right_hand_status;
    public string left_hand_status;
    public string legs_status;
    public int rating;
    public int authority;
    public int consequence;
    public int wins;
    public int fails;
    public int draws;
    public int kills;
    public int death;
    public int interventions;
    public int user_id;
    public int hp;
    public int max_hp;
    //public object about;
    public int? created_at;
    public User user;
    public List<Item> items;
}

[Serializable]
public class Gex
{
    public int x;
    public int y;
}

public class Player
{
    public int id;
    public string name;
    public string race;
    public string gender;
    public int level;
    public User user;
}

[Serializable]
public class User
{
    public int id;
    public int? created_at;
    public string account;
}

[Serializable]
public class Item
{
    public int id;
    public string template;
    public string status;
    public string position;
    public int enchases;
}

