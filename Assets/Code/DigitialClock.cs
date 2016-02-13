using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class DigitialClock : MonoBehaviour {

	private Text textClock;

	string LeadingZero(int n) {
		return n.ToString ().PadLeft (2, '0');
	}
	// Use this for initialization
	void Start () {
		textClock = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		DateTime time = DateTime.Now;

		String hour = LeadingZero (time.Hour);
		String minute = LeadingZero (time.Minute);
		String second = LeadingZero (time.Second);
		textClock.text = hour + ":" + minute + ":" + second;
	
	}


}
