using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BattlesListModel : BaseModel, IBattlesListModel
{
    public List<BattleData> BattleDataList { get; set; }
}
