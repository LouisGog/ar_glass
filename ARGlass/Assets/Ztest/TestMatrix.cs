using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMatrix : MonoBehaviour {


    Matrix4x4 mat;
    Matrix4x4 tranposeMat;
	void Start () {

        mat = new Matrix4x4();
        mat.m00 = 1;
        mat.m01 = 0;
        mat.m02 = 0;
        mat.m03 = 1;
        mat.m10 = 0;
        mat.m11 = 0;
        mat.m12 = 1;
        mat.m13 = 1;
        mat.m20 = 0;
        mat.m21 = -1;
        mat.m22 = 0;
        mat.m23 = 2;
        mat.m30 = 0;
        mat.m31 = 0;
        mat.m32 = 0;
        mat.m33 = 1;
        //tranposeMat = mat.transpose;
	}


    Quaternion getRotationFromMatrix(Matrix4x4 m)
    {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }

    Vector3 getPositionFromMatrix(Matrix4x4 m)
    {
        return m.GetColumn(3);
    }

    void Update () {

      
            transform.rotation = getRotationFromMatrix(mat);
            transform.position = getPositionFromMatrix(mat);
            Debug.Log(111);
       
      

	}
}
