using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;



public class JSONTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        string path = @"Assets\Data\localestext.txt";
        ///string path = @"Assets\Data\shop.txt";


        StreamReader src = new StreamReader(path);

        string json = src.ReadToEnd();

        src.Close();

        var nparsed = JSON.Parse(json);

        var name = nparsed["tutorial"]["phases"]["click_on_the_orc"];

        Debug.Log(name);

	}
	
	
}
