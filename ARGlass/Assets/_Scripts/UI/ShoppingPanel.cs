using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System;
using System.Text;

/// <summary>
///  购物车面板
/// </summary>
public class ShoppingPanel : MonoBehaviour {

    public GameObject glassPref;            //显示眼镜数据的预制体
    public Sprite normalSprite;             //没选择状态下的sprite 
    public Sprite selectSprite;             //选择状态下的sprite
    public GameObject downObj;              // 二维码obj
    public GameObject nothing;              //当购物车里没有物品的时候，显示的购物车空空的图片obj

    private Transform panel;                
    private Transform upTran;
    private Transform commoditysTran;
    private Transform contentTran;
    private Transform jiesuanTran;
 //   private Transform downTran;
    private Transform compareTran;
    private Button compareBt;


    private Text allPiece;
    private Text payText;
    private Button closeBt;
    private Button payBt;
    private Button selectAllBt;
    private Image qrCodeImg;

    private List<GlassModel> glassList;             //存储从服务器获取到的商品列表
    [HideInInspector]
    public List<GlassPanel> glassPanelList;         // 存储每个商品的面板信息
    [HideInInspector]
    public bool isSelectAll ;               // 是否全选

    RectTransform comRec;
    RectTransform contentRec;
    RectTransform jiesuanRec;
    [HideInInspector]
    public bool isUpdatePos = false;        // 是否更新二维码以及支付 一栏的位置


    void Awake () {

        glassList = new List<GlassModel>();
        glassPanelList = new List<GlassPanel>();
        goodsInfoList = new List<GoodsInfo>();
      //  goodsIdList = new List<int>();
        isSelectAll = false;

        panel = transform.Find("panel");
        upTran = transform.Find("up");
        commoditysTran = panel.Find("commoditys");
        contentTran = commoditysTran.Find("Viewport/Content");
        jiesuanTran = panel.Find("jiesuan");
      //  downTran = jiesuanTran.Find("down");

        closeBt = upTran.Find("closeBt").GetComponent<Button>();
        payBt = jiesuanTran.Find("payBt").GetComponent<Button>();
        selectAllBt = jiesuanTran.Find("allSelectBt").GetComponent<Button>();

        compareTran = jiesuanTran.Find("compareTran");
        compareBt = compareTran.Find("compareBt").GetComponent<Button>();

        allPiece = jiesuanTran.Find("allPiece").GetComponent<Text>();
        payText = payBt.GetComponentInChildren<Text>();
        qrCodeImg = downObj.transform.Find("qrcode").GetComponent<Image>();

        closeBt.onClick.AddListener(CloseShopCarPanel);
        selectAllBt.onClick.AddListener(SelectAll);
        payBt.onClick.AddListener(ClickPay);

        compareBt.onClick.AddListener(OpenComparePanel);
        //   _myGoods = new GoodsList();

        comRec = commoditysTran as RectTransform;
        contentRec = contentTran as RectTransform;
        jiesuanRec = jiesuanTran as RectTransform;

        AddEvent();

    }


    void AddEvent()
    {
        EventCenter.AddListener(EGameEvent.Event_UpdateScrollPos,UpdateScrollPos);
        EventCenter.AddListener(EGameEvent.Event_CancelSelect,CancelSelectAll);
    }

    void RemoveEvent()
    {
        EventCenter.RemoveListener(EGameEvent.Event_UpdateScrollPos, UpdateScrollPos);
        EventCenter.RemoveListener(EGameEvent.Event_CancelSelect, CancelSelectAll);
    }

    private void OnEnable()
    {
        pollingCount = 0;
        isSelectAll = false;
        isStartPolling = false;
        selectAllBt.GetComponent<Image>().sprite = normalSprite;
        if (AppConst.commdityIdList.Count != 0)
        {
            DeserializationJoson(AppConst.shopCartUrl);
        }
      
    }

    /// <summary>
    /// 当购物车没有物品时，更新二维码以及支付 一栏的位置
    /// </summary>
    void  SetNothingPos()
    {
        allPiece.text = "￥0";
        payText.text = "立即支付";
        nothing.SetActive(true);
        compareTran.gameObject.SetActive(false);
        jiesuanRec.anchoredPosition = new Vector2(0, 159);
        comRec.sizeDelta = new Vector2(comRec.sizeDelta.x, 0);
 
    }

    /// <summary>
    ///  取消全选
    /// </summary>
    private void CancelSelectAll()
    {
        if(isSelectAll)
        {
            isSelectAll = false; 
        }
        selectAllBt.GetComponent<Image>().sprite = normalSprite;
    }

    private bool isSetNoPos = false;        //标志位，确定是否设置了位置
    private float timer = 0;                //
    private int pollingCount = 0;           // 轮询次数

    private void Update()
    {
        if(glassPanelList.Count == 0 )
        {
            if(!isSetNoPos)
            {
                SetNothingPos();
                isSetNoPos = true;
            }       
        }
        else
        {       
            isSetNoPos = false;
            if (isUpdatePos)
            {
                Invoke("UpdateScrollPos", 0.03f);
                isUpdatePos = false;
            }
        }

        
        if(isStartPolling)
        {
            timer += Time.deltaTime;
            if(timer >= 2.0f)
            {
                pollingCount += 1;
                // 轮询分钟
                if(pollingCount > 90)
                {
                    HideDownObj();
                }
                else
                {
                    StartPolling();
                }            
                timer = 0;
            }
        }
    }


    /// <summary>
    /// 解析获取到的购物车json
    /// </summary>
    /// <param name="url"></param>
    private void DeserializationJoson(string url)
    {
        if(url == null)
        {
            Debug.Log("无效地址！！！");
            return;
        }
        NetManager.Instance.OnReceiveTextEvent(url, null,(string result) => {
            Debug.Log(result);
            JsonData json = JsonMapper.ToObject(result);
            string code = json["code"].ToString();
            if(code != "200")
            {
                Debug.Log("读取购物车信息失败！！");
                return;
            }
            JsonData array = json["data"];
            Debug.Log(array.Count);
            for (int i = 0; i < array.Count; i++)
            {
                string id = array[i]["id"].ToString();
                string num = array[i]["num"].ToString();
                string imgPath = array[i]["img"].ToString();
                string name = array[i]["product_name"].ToString();
                string piece = array[i]["product_price"].ToString();


                GlassModel glassModel = new GlassModel(name, id, piece, imgPath, num);
                glassList.Add(glassModel);

            }
            if (glassList.Count == 0)
            {
                return;
            }
            Debug.Log( "GlassListCout " + glassList.Count);
          
            for (int i = 0; i < glassList.Count; i++)
            {
                GameObject glassTran = Instantiate(glassPref);
                glassTran.transform.parent = contentTran;
                GlassPanel glassPanel = glassTran.AddComponent<GlassPanel>();
                glassPanel.nameText.text = glassList[i].name;
                glassPanel.id = Int32.Parse(glassList[i].id);
                glassPanel.piece.text = "￥" + glassList[i].piece;
                glassPanel.number.text = glassList[i].num;
                glassPanel.imageUrl = glassList[i].imageUrl;

                glassPanelList.Add(glassPanel);
                
            }
            nothing.SetActive(false);
            compareTran.gameObject.SetActive(true);
           
            // 调整scrow整体ui的位置
            Invoke("UpdateScrollPos", 0.1f); 
        });

    }

    /// <summary>
    ///  更新二维码以及支付 一栏的位置
    /// </summary>
    private void UpdateScrollPos()
    {
        comRec.sizeDelta = new Vector2(comRec.sizeDelta.x, contentRec.sizeDelta.y);
        float y = comRec.anchoredPosition.y - comRec.sizeDelta.y - jiesuanRec.sizeDelta.y/2;
        jiesuanRec.anchoredPosition = new Vector2(0, y);
 
    }


    /// <summary>
    ///  更新最后全部商品的总价
    /// </summary>
    public void UpdateTotalPrice()
    {
       
        float totalPrice = 0;
        int selectNumber = 0;
        for(int i =0; i< glassPanelList.Count; i++)
        {
            if(glassPanelList[i].isSelect)
            {
                string temp = glassPanelList[i].piece.text.Replace("￥", "");
                float unitPiece = Convert.ToSingle(temp);
                int num = Int32.Parse(glassPanelList[i].number.text);
                float allPiece = unitPiece * num;
                totalPrice = (float)Math.Round(totalPrice + allPiece ,2);
                selectNumber += 1;
            }
            allPiece.text = "￥" + totalPrice.ToString();
            payText.text = "立即支付(" + selectNumber.ToString() + ")";
        }
    }


    /// <summary>
    ///  全部选择
    /// </summary>
    private void SelectAll()
    {
        isSelectAll = !isSelectAll;
        if(isSelectAll)
        {
            selectAllBt.GetComponent<Image>().sprite = selectSprite;
            if (glassPanelList.Count == 0)
            {
                return;
            }
          
            for(int i=0;i < glassPanelList.Count; i++)
            {
                if (glassPanelList[i].isSelect == false)
                {
                    glassPanelList[i].OnSelect();
                }                         
            }
        }
        else
        {
            selectAllBt.GetComponent<Image>().sprite = normalSprite;
            for (int i = 0; i < glassPanelList.Count; i++)
            {
                if (glassPanelList[i].isSelect == true)
                {
                    glassPanelList[i].OnSelect();
                }
            }

        }

    }

    /// <summary>
    ///  判断当前是否有选择商品
    /// </summary>
    /// <returns></returns>
    private bool IsHaveSelected()
    {
        for (int i = 0; i < glassPanelList.Count; i++)
        {
            if(glassPanelList[i].isSelect == true)
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    ///   隐藏二维码部分
    /// </summary>
    public void  HideDownObj()
    {
        if(downObj.activeSelf)
        {
            downObj.SetActive(false);
            pollingCount = 0;
            isStartPolling = false;
        }
    }


    private List<GoodsInfo> goodsInfoList ;         // 存储选择的商品信息的链表

    public bool isStartPolling = false;     // 是否开始轮询
    /// <summary>
    /// 点击支付
    /// </summary>
    private void ClickPay()
    {

        if(glassPanelList.Count == 0)
        {
            return;
        }
       if(!IsHaveSelected())
        {
            return;
        }
        //TODO   在这之前获取选择商品的id 和 数量 ，转成json
     for(int i=0;i < glassPanelList.Count; i++)
        {
           if(glassPanelList[i].isSelect)
            {
                int id = glassPanelList[i].id;
                int num = Int32.Parse(glassPanelList[i].number.text);
                GoodsInfo goods = new GoodsInfo(id, num);
                goodsInfoList.Add(goods);
                // 点击支付，清除选中商品的id
                AppConst.commdityIdList.Remove(id);
            }
        }

        string goodsJson = JsonMapper.ToJson(goodsInfoList);
        Debug.Log(goodsJson);
       // byte[] postGoodsInfo = Encoding.Default.GetBytes(goodsJson);
       // form表单添加header头
        WWWForm form = new WWWForm();
        form.AddField("order",goodsJson);
        byte[] postGoodsInfo = form.data;
            NetManager.Instance.OnReceiveTextEvent(AppConst.payUrl, postGoodsInfo, (string result) =>
            {
                JsonData json = JsonMapper.ToObject(result);
                JsonData data = json["data"];
                int code = Int32.Parse(json["code"].ToString());             
                if(code == 200)
                {
                    string imgUrl = data["url"].ToString();
                    AppConst.order_id = data["order_id"].ToString();
                    Debug.Log(AppConst.order_id);
                    NetManager.Instance.OnReceiveImageEvent(imgUrl, null, (Texture2D t) =>
                    {
                        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
                        qrCodeImg.sprite = temp;
                    });
                }
                else
                {
                    // 这里弹出错误提示，可能有好几种结果：1。 订单商品不够 2.订单商品不存在
                    Debug.Log("商品不存在！！！");
                }
            });
        if (!downObj.activeSelf)
        {
            downObj.SetActive(true);
        }
        isStartPolling = true;
        pollingCount = 0;
        goodsInfoList.Clear();
    }


    /// <summary>
    /// 开始轮询
    /// </summary>
    private void StartPolling()
    {
        string pendingUrl = AppConst.pengdingStateUrl + AppConst.order_id;

        NetManager.Instance.OnReceiveTextEvent(pendingUrl, null, (string result) => 
        {
            Debug.Log(result);
            JsonData json = JsonMapper.ToObject(result);
            JsonData data = json["data"];
            int code = Int32.Parse(json["code"].ToString());
            if (code != 200)
            {
                return;
            }
            // TODO  这里之后添加个出货商品的model类,用来存储将要出的货
            
            isStartPolling = false;
            EnterPaySuccessPanel();
        });
    }



    // 进入支付成功界面 
    private void EnterPaySuccessPanel()
    {
        GameController.instance.currentState = AppState.LookMirror;
        UIController.instance.paySuccessPanelObj.SetActive(true);
        CloseShopCarPanel();
    }

    /// <summary>
    /// 打开比一比面板
    /// </summary>
    private void OpenComparePanel()
    {

        UIController.instance.comparePanelObj.SetActive(true);
        EventCenter.Broadcast(EGameEvent.Event_InitTwoScreen);
    }

    /// <summary>
    /// 关闭shopcar 面板, 清空数据
    /// </summary>
    private void CloseShopCarPanel()
    {
        glassList.Clear();
        glassPanelList.Clear();
        goodsInfoList.Clear();
        if (contentTran.childCount != 0)
        {
            foreach (Transform go in contentTran)
            {
                Destroy(go.gameObject);
            }
        }

        JudgeState();

      //  GameController.instance.currentState = AppState.OneScreen;
    }

    private void JudgeState()
    {
        HideDownObj();

        comRec.sizeDelta = new Vector2(720, 0);
        if (GameController.instance.currentState == AppState.LookMirror)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            string name = "unity_" + GameController.instance.needLoadModelId.ToString();
            EventCenter.Broadcast(EGameEvent.Event_CreateGlass, name);
            this.gameObject.SetActive(false);
        }
    }


   


    void OnDestroy()
    {
        RemoveEvent();
    }
}
