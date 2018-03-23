using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ImageHandler : MonoBehaviour {

 
    public Image glassImg;
    public Text nameText;
    public Text pieceText;
    public int id;                //当前图片对应的眼镜id

    private GameObject selectImgObj;
    private GameObject noselectImgObj;

    private ComparePanel comparePanel;

    void Awake()
    {

    }

	void Start () {

        selectImgObj = transform.Find("select").gameObject;
        noselectImgObj = transform.Find("noselect").gameObject;
        comparePanel = transform.root.Find("ComparePanel").GetComponent<ComparePanel>();
        this.GetComponent<Button>().onClick.AddListener(OnClickImage);
        SetSelectState(false);
    }
	

    void OnEnable()
    {
       
    }

    // 当关闭比较面板的时候，重新将图片设置为未选中状态
    void OnDisable()
    {
        SetSelectState(false);
    }

    // 选中图片和未选中图片状态
    public void SetSelectState(bool select)
    {
        if(select)
        {
            selectImgObj.SetActive(true);
            noselectImgObj.SetActive(false);
            nameText.color = new Color(152.0f / 255.0f, 93.0f / 255.0f, 59.0f / 255.0f);
        }
        else
        {
            selectImgObj.SetActive(false);
            noselectImgObj.SetActive(true);
            nameText.color = Color.black;
        }
    }

	void Update () {
		

	}

    // 点击图片
   public void OnClickImage()
    {
        // 逻辑判断加载模型
        if (comparePanel.selectImgList.Count == 1 && comparePanel.selectImgList[0].id == this.id)
        {
            return;
        }

        // 点击效果显示
        SetSelectState(true);
     
        string name = "unity_" + this.id.ToString();
        Debug.Log("加载name " + name);
        // 当选中的图片大于2时，清除列表里的，然后加载新选中的，并且存入列表中
        if (comparePanel.selectImgList.Count >= 2)
        {
            foreach(ImageHandler temp in comparePanel.selectImgList)
            {
                if(this.id != temp.id )
                {
                    temp.SetSelectState(false);
                }
                
            }
            EventCenter.Broadcast(EGameEvent.Event_DeleteGlass,true);
            comparePanel.selectImgList.Clear();
            comparePanel.selectImgList.Add(this);       
            EventCenter.Broadcast(EGameEvent.Event_CreateGlass, name);
        }
        // 当小于2的时候，判断如果当前选中的图片为0， 则把当前选中的加入到选中列表中，当前眼镜为currentModel；如果是已经存在一个，则说明当前存在的是currentModel，
        //即加载的是anotherModel,并且 把这个图片存到列表中
        else
        {
            if(comparePanel.selectImgList.Count == 0)
            {
                comparePanel.selectImgList.Add(this);
                EventCenter.Broadcast(EGameEvent.Event_CreateGlass, name);
            }
            else if(comparePanel.selectImgList.Count == 1)
            {
                EventCenter.Broadcast(EGameEvent.Event_DeleteGlass, false);
                comparePanel.selectImgList.Add(this);
                GameController.instance.LoadAnotherGlass(name);
            }
        }
    }
}

