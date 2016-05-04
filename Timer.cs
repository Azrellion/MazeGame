using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public Text timerText;

    int count = 0;

	// Use this for initialization
	void Start () {

        StartCoroutine(OneSecondTimer());
	}

    IEnumerator OneSecondTimer()
{
    while(1==1)
    {
        yield return new WaitForSeconds(1.0f);
        count++;
        timerText.text = "Time: " + count.ToString(); 
    }

}
	
	// Update is called once per frame
	void Update () {
	
	}
}
