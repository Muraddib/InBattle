using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public UserInfo info;
}

[Serializable]
public class UserInfo
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
    public int created_at;
    public User user;
    public List<Item> items;

    public int str;
    public int dex;
    public int rancor;
    public int rage;
    public int luck;
    public int @const;
    public int dex_mirror;
    public int rancor_mirror;
    public int rage_mirror;
    public int luck_mirror;
    public int dex_aura;
    public int rancor_aura;
    public int rage_aura;
    public int luck_aura;
    public int free_points;
    public int exp;
    public int exp_to_level_up;
    public int exp_to_interim_up;
    public int money;
    public int potions;
    public int elixirs;
    public int permits;
    public int keys;
    public int oblivions;
    public bool? in_bid;
    public bool? in_battle;
    public bool in_view;
    public int? battle_id;
}
