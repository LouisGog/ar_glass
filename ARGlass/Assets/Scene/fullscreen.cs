using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fullscreen : MonoBehaviour {



	void Awake () {

        Resolution[] resolutions = Screen.resolutions;

        Screen.SetResolution(resolutions[resolutions.Length -1].width,resolutions[resolutions.Length-1].height,true);

        Screen.fullScreen = true;
	}
	

	void Update () {
		
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
}
