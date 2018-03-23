using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ComparePanel : MonoBehaviour {

    [HideInInspector]
    public List<ImageHandler> selectImgList;            // 用来存放比一比时选中的眼镜列表

    private Button shopcar;
    private Transform glasses;
    private ShoppingPanel shoppingPanelSpt;
    private Text numberText;
  //  private int count ;


	void Awake () {

    //    count = 0;
        selectImgList = new List<ImageHandler>();
        shopcar = transform.Find("shopcar").GetComponent<Button>();
        glasses = transform.Find("glasses");
        numberText = shopcar.transform.Find("number").GetComponent<Text>();

        shoppingPanelSpt = UIController.instance.shoppingPanel;

        shopcar.onClick.AddListener(BackShoppingCar);
	}
	
    void OnEnable()
    {
        SetInit();    
    }

    // 根据shoppingPanel脚本中的glassPanel链表，来初始化
    void SetInit()
    {
        for (int i = 0; i < shoppingPanelSpt.glassPanelList.Count; i++)
        {
            GlassPanel tmpGlass = shoppingPanelSpt.glassPanelList[i];

            //TODO 在这里判断如果商品列表中物品的名字不是glass开头的(这里可以和服务器那边商量，服务器那边定下传过来的眼镜信息的时候，
            //顺便把该眼镜对应的unity里的眼镜名字传过来，存储在glassPanel中，方便之后在这里的判断是眼镜还是其他物品)，则continue（停止本次赋值，执行i+1次）
            
  

            ImageHandler glassImageHlr = glasses.GetChild(i).GetComponent<ImageHandler>();
            glassImageHlr.glassImg.sprite = tmpGlass.touxiang.sprite;
            glassImageHlr.nameText.text = tmpGlass.nameText.text;
            glassImageHlr.pieceText.text = tmpGlass.piece.text;
            glassImageHlr.id = tmpGlass.id;
            glassImageHlr.gameObject.SetActive(true);

        }
        numberText.text = shoppingPanelSpt.glassPanelList.Count.ToString();
    }


    //void OnDisable()
    //{
    //    for (int i = 0; i < count - 1; i++)
    //    {
    //        GameObject glassImageObj = glasses.GetChild(i).gameObject;
    //        glassImageObj.SetActive(false);
    //       // glassImage.imgState = ImageState.NoSelect;

    //    }
    //}

	void Update () {
		
	}


    /// <summary>
    /// 点击购物车按钮返回购物车
    /// </summary>
    void BackShoppingCar()
    {
        GameController.instance.BackToShoppingPanel();
        selectImgList.Clear();
        this.gameObject.SetActive(false);
    }
}
