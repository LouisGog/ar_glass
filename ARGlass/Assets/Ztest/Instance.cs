using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instance : MonoBehaviour {

    public static Instance instance;

	void Awake () {
        instance = this;
	}
	
	void Update () {
		
	}
}
