using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZenFulcrum.EmbeddedBrowser;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {

    private Browser browser;
    private int currentId;              //当前眼镜id
   // private string oldUrl;

    private Button closeBt; 
	void Awake () {

        browser = transform.GetComponentInChildren<Browser>();
       
    }

    private void Start()
    {
        closeBt = transform.Find("InfoBrowser/closeBt").GetComponent<Button>();
        closeBt.onClick.AddListener(delegate () {
        //    GameController.instance.ShowCurrentModel(true);
            this.gameObject.SetActive(false);
        });
    }

    // 每次打开info面板的时候，初始化url地址
    private void OnEnable() 
    {

        currentId = GameController.instance.needLoadModelId;

        string url = AppConst.infoUrl + currentId;

            Debug.Log("当前物品信息地址：" + url);
            browser.LoadURL(url, true);
            Debug.Log(browser.Url);
            browser.RegisterFunction("transferUnity", args =>
            {
                int n = args[0];
                // 加入购物车后， 获取加入的物品id，存到商品列表里
                if(AppConst.commdityIdList.Contains(n))
                {
                    return;
                }
                AppConst.commdityIdList.Add(n);
                Debug.Log("加入购物车的商品id ： " + n);
            });

        browser.RegisterFunction("order", args => {
            int id = args[0];
            Debug.Log("立即购买");
            if (!AppConst.commdityIdList.Contains(id))
            {
                AppConst.commdityIdList.Add(id);
            }
            
            UIController.instance.ShowShoppingPanel();
            this.gameObject.SetActive(false);
        });
       
    }


    void Update () {
		
	}
}
