using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Text;

public class testDllWritePath : MonoBehaviour {

    [DllImport("makeDll")]
    private static extern int AddMatFromFile(int w, int h, int channels, StringBuilder data1,StringBuilder data2, double[] dataOut);


    private string temp1 = "";
    private string temp2 ="";
    private double[] dataOut = new double[8] { 1,1,1,1,1,1,1,1};

    void Start () {

        Debug.Log(Application.persistentDataPath);
        temp1 = Application.persistentDataPath + "/data1.txt\0";
        temp2 = Application.persistentDataPath + "/data2.txt\0";

        
        StringBuilder data1 = new StringBuilder(temp1.Length);
        data1.Insert(0,temp1,temp1.Length);
        Debug.Log(data1.Length); 
        StringBuilder data2 = new StringBuilder(temp2.Length);
        data2.Insert(0,temp2,temp2.Length);

        //StreamReader read = new StreamReader(data1);
        //string t= read.ReadToEnd();
        //Debug.Log(t);
        int n =  AddMatFromFile(2,2,2,data1,data2,dataOut);
        foreach(double b in dataOut)
        {
            Debug.Log(b);
        }
    }

}
