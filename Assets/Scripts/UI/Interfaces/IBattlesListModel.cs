using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public interface IBattlesListModel
{
    List<BattleData> BattleDataList { get; set; }
}
