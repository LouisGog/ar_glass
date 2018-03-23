using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZenFulcrum.EmbeddedBrowser;

[RequireComponent(typeof(Browser))]
public class CommodityPanel : MonoBehaviour {

    private Browser browser;


	void Start () {

        browser = GetComponent<Browser>();

        //browser.Url = AppConst.commdityUrl;
        Invoke("OpenUrl",1.0f);
	}
	
    void OpenUrl()
    {

        string url = AppConst.commdityUrl + "?device_id=" + AppConst.dataIndex_Url;

        browser.LoadURL(url, true);

        // 加入购物车方法
        browser.RegisterFunction("transferUnity", args => {
            int n = args[0];
            // 加入购物车后， 获取加入的物品id，存到商品列表里
            if(AppConst.commdityIdList.Contains(n))
            {
                return;
            }
            AppConst.commdityIdList.Add(n);
            Debug.Log("加入购物车的商品id ： " + n);
        });

        // 点击后加载眼镜，并且可以获取当前商品的id，查看商品详情
        browser.RegisterFunction("showUnityModel", args => {
            string name = args[0];
            int id = args[1];
            Debug.Log(name + "," + id);
            GameController.instance.needLoadModelId = id;

            if (name == GameController.instance.currentModel.name)
            {
                return;
            }
            // GameController.instance.LoadGlassByName(name);
            EventCenter.Broadcast(EGameEvent.Event_CreateGlass,name);
        });
    }
}
