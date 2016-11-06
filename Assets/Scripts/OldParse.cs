using UnityEngine;
using System.Collections;

public class OldParse : MonoBehaviour {


    //private static void ParseData(string data)
    //{
    //    var message = Json.Deserialize(data) as Dictionary<string, object>;

    //    foreach (KeyValuePair<string, object> pair in message)
    //    {

    //        Debug.Log("{ " + "key:" + pair.Key + " value:" + pair.Value + " }");

    //        //Debug.Log(type.Key.ToString() + " " + type.Value.ToString());


    //       // Debug.Log(type.Key + " " + type.Value);



    //        //string value = (string)pair.Value;


    //        //string cKey = pair.Key;


    //        //Debug.Log(cKey);



    //        switch (pair.Key)
    //        {

    //            case "redirect":


    //                if (pair.Value as string != null)
    //                {
    //                    Debug.Log(pair.Value as string);


    //                }

    //                //if (value == "refresh" || value == "duplicate" || value == "banned" || value == "destroy")
    //                //{
    //                //    //quiet_close = true;
    //                //    //puts("OK");
    //                //}
    //                //activate_title_button(value);
    //                break;
    //            case "info":

    //                if (pair.Value as Dictionary<string, object> != null)
    //                {

    //                    var infoDict = pair.Value as Dictionary<string, object>;

    //                    foreach (KeyValuePair<string, object> infoP in infoDict)
    //                    {
    //                        Debug.Log("info{ " + "key:" + infoP.Key + " value:" + infoP.Value + " }");

    //                        switch (infoP.Key)
    //                        {

    //                            case "user":



    //                                var userDict = infoP.Value as Dictionary<string, object>;


    //                                foreach (var userI in userDict)
    //                                {
    //                                    Debug.Log("user{ " + "key:" + userI.Key + " value:" + userI.Value + " }");

    //                                }

    //                                break;

    //                            case "items":

    //                                if (infoP.Value as List<object> != null)
    //                                {

    //                                    var itemsList = infoP.Value as List<object>;

    //                                    foreach (var item in itemsList)
    //                                    {
    //                                        if (item as Dictionary<string, object> != null)
    //                                        {

    //                                            var itemDict = item as Dictionary<string, object>;

    //                                            foreach (var o in itemDict)
    //                                            {
    //                                                //Debug.Log(o);
    //                                            }


    //                                        }
    //                                    }

    //                                }

    //                                break;

    //                        }


    //                    }


    //                }

    //                //character_refresh(value);
    //                break;
    //            case "characters":
    //                //character_list(value);
    //                break;
    //            case "online":
    //                //refresh_online();
    //                break;
    //            case "level_up":
    //                //level_up(value);
    //                break;
    //            case "present":
    //                //show_present();
    //                break;
    //            case "about":
    //                //about_activate(value);
    //                break;
    //            case "chat":

    //                if (pair.Value as Dictionary<string, object> != null)
    //                {

    //                    var infoDict = pair.Value as Dictionary<string, object>;

    //                    foreach (KeyValuePair<string, object> infoP in infoDict)
    //                    {
    //                        Debug.Log("chat{ " + "key:" + infoP.Key + " value:" + infoP.Value + " }");
    //                    }


    //                }


    //                //Debug.Log(type.Value.GetType().Name);

    //                //if (type.Value as Dictionary<string, object> != null)
    //                //{

    //                //    foreach (var VARIABLE in type.Value)
    //                //    {
    //                //        Debug.Log(VARIABLE);
    //                //    }

    //                //}

    //                //write_to_chat(value);
    //                break;
    //            case "flashback":
    //                //flashback(value);
    //                break;
    //            case "hall":
    //                //refresh_hall();
    //                break;
    //            case "battle":
    //                //battle_refresh(value);
    //                break;
    //            case "battler":
    //                //battler_refresh(value);
    //                break;
    //            case "action":
    //                //action_performed(value);
    //                break;
    //            case "message":
    //                //print(value);
    //                break;
    //            case "error":
    //                //show_error(value);
    //                break;
    //            case "sync":
    //                // reset_timestamp(value);
    //                break;
    //            case "farmers":
    //                break;
    //            case "options":
    //                break;
    //            case "battles":
    //                break;
    //            case "keeper":
    //                break;
    //            case "competent":
    //                // show_log(type, value);
    //                break;
    //        }
    //    }
    //}
}
