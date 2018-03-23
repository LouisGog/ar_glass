using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testScroview : MonoBehaviour {


    public RectTransform content;
    //public RectTransform parentContent;
    //public RectTransform parentScroll;
    public RectTransform jiesuan;

    RectTransform thisRec;
	void Start () {
        thisRec = this.transform as RectTransform;
        
        Debug.Log(thisRec.rect.height);
        Debug.Log(thisRec.sizeDelta.y);
        //   System.IO.File.Delete(@"C:\Users\xloong\Desktop\bg.png");
      //  this.GetComponent<Button>().onClick.AddListener(()=> { Debug.Log("sddf"); });   
        
    }
	
	
	void Update () {
        
        //thisRec.anchoredPosition = new Vector2(446,0);

        thisRec.sizeDelta = new Vector2(thisRec.sizeDelta.x, content.sizeDelta.y);
        jiesuan.anchoredPosition = new Vector2(jiesuan.anchoredPosition.x, 574 - thisRec.sizeDelta.y);

       // Debug.Log(jiesuan.anchoredPosition.y);
        //parentContent.sizeDelta = new Vector2(parentContent.sizeDelta.x,thisRec.sizeDelta.y + jiesuan.sizeDelta.y);
        //parentScroll.sizeDelta = new Vector2(parentScroll.sizeDelta.x,parentContent.sizeDelta.y);
	}
}
