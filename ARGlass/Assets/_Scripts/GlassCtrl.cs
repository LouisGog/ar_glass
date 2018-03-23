using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlassCtrl : MonoBehaviour {


    Transform leftVec;
    Transform rightVec;
    Vector3 rotateValue;
    GameObject glass;

	void Start () {

        leftVec = transform.Find("glass/leftVec");
        rightVec = transform.Find("glass/rightVec");
        glass = transform.Find("glass").gameObject;
	}
	
	void Update () {

        rotateValue = transform.rotation.eulerAngles;
        if(rotateValue.y > 180)
        {
            if(rotateValue.y >= 187)
            {
                leftVec.gameObject.SetActive(false);
                rightVec.gameObject.SetActive(true);
            }
            else
            {
                SetState(false);
            }
        }
        else
        {
            if (rotateValue.y <= 176)
            {
                rightVec.gameObject.SetActive(false);
                leftVec.gameObject.SetActive(true);
            }
            else
            {
                SetState(false);
            }
        }

        // 设置当 x轴转的角度 与 y轴转动的角度比较，如果x大于y的， 则判断x轴 ，否则判断y轴
        float temY = Mathf.Abs(180 - rotateValue.y);
        float temX = Mathf.Abs(rotateValue.x);



        //if (temY > CalculateValue(transform.position.x))
        //{
        //    glass.SetActive(false);
        //}
        //else
        //{
        //    glass.SetActive(true);
        //}


        if (GameController.instance.value <= 0)
        {
            timer += Time.deltaTime;
            if (timer > 1.0f)
            {
                transform.position = new Vector3(0, 0, 0.4f);
                transform.rotation = Quaternion.Euler(0, 180, 0);
                transform.localScale = Vector3.one;
            }
        }
        else
        {
            timer = 0f;
        }

        //timer += Time.deltaTime;
        //if (timer >= 1.0f)
        //{
        //    x.text = "X :" + transform.position.x.ToString();
        //    y.text = "Y :" + transform.position.y.ToString();
        //    z.text = "Z :" + transform.position.z.ToString();
        //    timer = 0;
        //   // Debug.Log(y.text);
        //}
    }

    float timer = 0f;
    //public Text x;
    //public Text y;
    //public Text z;


    float criticalValue = 0;

    // 计算眼镜消失的临界角度
    private float CalculateValue(float tempX)
    {

        if (tempX > 0)
        {
            if (tempX < 0.05f)
            {
                criticalValue = 32;
            }
            else if (tempX >= 0.05f && tempX < 0.1f)
            {
                criticalValue = 37;
            }
            else if (tempX >= 0.1f && tempX < 0.2f)
            {
                criticalValue = 47;
            }
            else
            {
                criticalValue = 55;
            }
      
        }
        else
        {
            if (tempX >= -0.02f)
            {
                criticalValue = 35;
            }
            else if (tempX < -0.02f && tempX >= -0.07f)
            {
                criticalValue = 38;
            }
            else if (tempX < -0.07f && tempX >= -0.1f)
            {
                criticalValue = 43;
            }
            else if (tempX < -0.1f && tempX >= -0.2f)
            {
                criticalValue = 47;
            }
            else
            {
                criticalValue = 55;
            }
        }
        // Debug.Log("当前临界点值为：" + criticalValue);
        return criticalValue;
    }

    private void SetState(bool isactive)
    {
        leftVec.gameObject.SetActive(isactive);
        rightVec.gameObject.SetActive(isactive);
    }
}
