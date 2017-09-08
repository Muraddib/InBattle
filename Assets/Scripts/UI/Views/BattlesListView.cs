using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlesListView<M,C> : BaseView<M,C> where M: BattlesListModel where C: BattlesListController<M>, new() 
{

}
