using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [Tooltip("Match timer in minutes")]
    [SerializeField] private int duration = 5;
    [SerializeField] private Text timerText;

    private float _startTime;
    private int _secondsLeft;
    private bool _toCount = false;

    public void customStart()
    {
        _startTime = Time.time;
        _secondsLeft = duration * 60 + 1;
        _toCount = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (_toCount)
        {
            float difference = Time.time - _startTime;
            float _secondsLeft = this._secondsLeft - difference;

            int minutes = (int) _secondsLeft / 60;
            int seconds = (int) _secondsLeft % 60;

            string secondsString = seconds.ToString();
            if (seconds < 10)
            {
                secondsString = "0" + seconds;
            }
            
            if ((int) _secondsLeft == 1 || (int) _secondsLeft == 3 || (int)_secondsLeft == 5 || (int)_secondsLeft == 10 || (int)_secondsLeft == 30)
                timerText.color = Color.red;
            else
            {
                timerText.color = Color.white;
            }

            timerText.text = minutes + ":" + secondsString;
            
            if ((int)_secondsLeft < 1)
            {
                _toCount = false;
                gameObject.GetComponent<EnvironmentManager>().endGame();
            }
        }
    }

    public void setLastStand()
    {
        timerText.text = "Last Stand";
        timerText.color = Color.red;
    }
}
