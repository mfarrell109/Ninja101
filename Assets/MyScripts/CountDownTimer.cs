using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CountDownTimer : MonoBehaviour {

	private Text textClock;
	private float countDownTimerDuration;
	private float countDownTimerStartTime;

	// Use this for initialization
	void Start () {
		textClock = GetComponent<Text>();
		CountDownTimerReset (30);

	}
	
	// Update is called once per frame
	void Update () {
		//default = timer finished
		string timerMessage = "Countdown has finished";
		int timeLeft = (int)CountDownTimerSecondsRemaining();

        if (timeLeft > 0)
            timerMessage = "Countdown seconds remaining = " + LeadingZero(timeLeft);
        textClock.text = timerMessage;
	}

    private void CountDownTimerReset(float delaySeconds)
    {
        countDownTimerDuration = delaySeconds;
        countDownTimerStartTime = Time.time;
    }

    private float CountDownTimerSecondsRemaining()
    {
        float elapsedSeconds = Time.time - countDownTimerStartTime;
        float timeLeft = countDownTimerDuration - elapsedSeconds;
        return timeLeft;
    }

    private string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }
}
