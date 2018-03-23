using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour {

    public static UIController instance = null;

    public GameObject shoppingPanelObj;         // 购物车面板obj
    public GameObject helpPanelObj;             // 帮助面板obj
    public GameObject infoPanelObj;             // 物品详情面板obj
    public GameObject comparePanelObj;          // 比一比面板obj
    public GameObject paySuccessPanelObj;       // 支付成功面板obj

    [HideInInspector]
    public ShoppingPanel shoppingPanel;         // 购物车面板上的脚本

    private Transform functionPanel;

    private Button helpBt;
    private Button clearBt;
    private Button infoBt;
    private Button shopCarBt;
    private Button fenpingBt;
    private Text shoppingNum;
    private GameObject loginImg;         //登录界面的图片
    private GameObject tipObj;          // 提示信息

    void Awake()
    {
        instance = this;
    }

	void Start () {

        functionPanel = transform.Find("FunctionPanel");

        shoppingNum = functionPanel.Find("shoppingNum").GetComponentInChildren<Text>();

        helpBt = functionPanel.Find("help").GetComponent<Button>();
        clearBt = functionPanel.Find("clear").GetComponent<Button>();
        infoBt = functionPanel.Find("info").GetComponent<Button>(); 
        shopCarBt = functionPanel.Find("shopcar").GetComponent<Button>();

        shoppingPanel = shoppingPanelObj.GetComponent<ShoppingPanel>();

        helpBt.onClick.AddListener(ShowHelpPanel);
        clearBt.onClick.AddListener(ClearCurrentData);
        infoBt.onClick.AddListener(ShowInfoPanel);
        shopCarBt.onClick.AddListener(ShowShoppingPanel);
        //    fenpingBt.onClick.AddListener(FenPing);
        tipObj = functionPanel.Find("tip").gameObject;
        loginImg = transform.Find("LoginImage").gameObject;
        Invoke("SetLoginImgFalse",1.2f);
    }

    // 将登录图片隐藏
    void SetLoginImgFalse()
    {
        loginImg.SetActive(false);
    }

    private void Update()
    {
      // 在这里判断是否显示购物车里的物品数量
        if (AppConst.commdityIdList.Count == 0)
        {
            shoppingNum.transform.parent.gameObject.SetActive(false);
            return;
        }
        else
        {
            shoppingNum.transform.parent.gameObject.SetActive(true);
            ShowShoppingNum();
        }
      
    }

    //显示购物车数量
    private void ShowShoppingNum()
    {
        shoppingNum.text = AppConst.commdityIdList.Count.ToString();
    }


    // 显示购物车面板
    public void ShowShoppingPanel()
    {
        if(shoppingPanelObj.activeSelf)
        {
            return;
        }
        shoppingPanelObj.SetActive(true);
        GameController.instance.HideCurrentGlass();
      
    }


    //显示帮助面板
    private void ShowHelpPanel()
    {
        if(helpPanelObj.activeSelf)
        {
            return;
        }
        helpPanelObj.SetActive(true);
    }


    // 显示商品详情面板
    private void ShowInfoPanel()
    {
        if(infoPanelObj.activeSelf)
        {
            return;
        }
        infoPanelObj.SetActive(true);
    }



    // 清空当前数据
    private void ClearCurrentData()
    {
        // 清空数据前判断当前商品链表里是否存在商品
        if (AppConst.commdityIdList.Count == 0)
        {
            return;
        }
        for (int i = 0; i < AppConst.commdityIdList.Count; i++)
        {
            DelGlassCart(AppConst.commdityIdList[i]);
        }

        AppConst.commdityIdList.Clear();
        tipObj.SetActive(true);
        Invoke("DisableTipObj",1.5f);
        Debug.Log("清空数据");
    }

    // 隐藏清空数据obj
    private void DisableTipObj()
    {
        tipObj.SetActive(false);
    }


    /// <summary>
    /// 删除选中的眼镜商品
    /// </summary>
    /// <param name="id"></param>
    private void DelGlassCart(int id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id.ToString());
        byte[] data = form.data;
        NetManager.Instance.OnReceiveTextEvent(AppConst.delCartUrl, data, (string result) =>
        {
            string code = LitJson.JsonMapper.ToObject(result)["code"].ToString();
            if (code == "200")
            {
                Debug.Log("删除物品成功");
            }
            else
            {
                Debug.Log("商品不存在");
            }
        });
    }

}
