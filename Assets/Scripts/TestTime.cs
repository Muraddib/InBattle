using UnityEngine;
using System.Collections;
using System;

public class TestTime : MonoBehaviour {

	// Use this for initialization

    static double ConvertToUnixTimestamp(DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        TimeSpan diff = date - origin;
        return Math.Floor(diff.TotalSeconds);
    }

	void Start () {
	
      Debug.Log(ConvertToUnixTimestamp(DateTime.Now));
	}

}
