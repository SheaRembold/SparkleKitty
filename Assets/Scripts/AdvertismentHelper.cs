using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvertismentHelper : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {

        if(Input.GetMouseButtonDown(0))
        {
            ScreenCapture.CaptureScreenshot("C:/Users/POG-LW/Documents/cats.png",2);
        }
	}

  
}
