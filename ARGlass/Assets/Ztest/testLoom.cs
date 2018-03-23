using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using System;
using System.Threading;

public class testLoom : MonoBehaviour {


    public Text text;
    Thread thread;

    void Start () {

        //Loom.RunAsync(() =>
        //{
        //   thread = new Thread(RefreshText);
        //    thread.Start();

        //});
          InvokeRepeating("Excute",0,1);
        //Invoke("Excute",0);
	}
	
	void Update () {
		
        if(Input.GetMouseButtonDown(0))
        {
            thread.Abort();
        }
	}


    private void RefreshText()
    {
        // 用Loom的方法在Unity主线程中调用Text组件
        Loom.QueueOnMainThread(() =>
        {
            text.text = "Hello Loom!";
            Debug.Log(1111);
        });
    }

    int i = 0;
    private void Excute()
    {
      
        string mytext= "";
        Debug.Log(i);
        Loom.RunAsync(()=> {
            i++;
            Debug.Log("yibu : "+i);
            mytext = "我是第" + i + " 次执行";
        });
        Loom.QueueOnMainThread(()=> {
            text.text = mytext;
            Debug.Log("yifan");
        });
        
    }
}
