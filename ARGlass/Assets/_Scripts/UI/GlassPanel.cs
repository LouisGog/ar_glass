using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class GlassPanel : MonoBehaviour {

   
    public string imageUrl;           // 需要加载的图片地址
    [HideInInspector]
    public Text nameText;
    [HideInInspector]
    public Text piece;
    [HideInInspector]
    public bool isSelect;
    [HideInInspector]
    public int id;

    private Button selectBt;
    public Button addBt;
    public Button reduceBt;
    public Text number;
    public Button deleteBt;

    private Transform numPab;
    [HideInInspector]
    public Image touxiang;
    private bool isLoad ;
   
    private Sprite normalSpt;           //未选中状态的sprite
    private Sprite selectSpt;           //选中状态的sprite

    private ShoppingPanel shoppingPanel;

    void Awake () {

        isLoad = false;
        isSelect = false;

        shoppingPanel = UIController.instance.shoppingPanel;

        selectBt = transform.Find("selectBt").GetComponent<Button>();
        touxiang = transform.Find("touxiang").GetComponent<Image>();
        nameText = transform.Find("name").GetComponent<Text>();
        piece = transform.Find("piece").GetComponent<Text>();

        numPab = transform.Find("numPab");
        addBt = numPab.Find("addBt").GetComponentInChildren<Button>();
        reduceBt = numPab.Find("reduceBt").GetComponentInChildren<Button>();
        number = numPab.Find("number").GetComponent<Text>();

        deleteBt = transform.Find("deleteBt").GetComponent<Button>();

        selectBt.onClick.AddListener(OnSelect);
        addBt.onClick.AddListener(AddNumber);
        reduceBt.onClick.AddListener(ReduceNumber);
        deleteBt.onClick.AddListener(DeleteThis);

	}


    void Start()
    {
        selectSpt = shoppingPanel.selectSprite;
        normalSpt = shoppingPanel.normalSprite;

    }

    private void Update()
    {
        if(imageUrl != null && !isLoad)
        {
            LoadImage();
            isLoad = true;
        }
    }


    // 加载眼镜信息对应的图片
    void LoadImage()
    {
        string truePath = AppConst.defultImageUrl + imageUrl;
        NetManager.Instance.OnReceiveImageEvent(truePath,null,(Texture2D t)=> {
            Sprite temp = Sprite.Create(t,new Rect(0,0,t.width,t.height),new Vector2(0,0));
            touxiang.sprite = temp;
        });
    }

    // 增加数量
    void AddNumber()
    {
        int n = int.Parse(number.text);
        n++;
        if(n >= 99)
        {
            n = 99;
        }
        number.text = n.ToString();
        if(isSelect)
        {
            // UpdatePrice(true);
            ExcuteClickEvent();
        }    
    }

    //减少数量
    void ReduceNumber()
    {
        int n = int.Parse(number.text);
        n--;
        if(n <= 1)
        {
            n = 1;
        }
        number.text = n.ToString();
        if(isSelect)
        {
            // UpdatePrice(false);
            ExcuteClickEvent();
            
        }
    }

    // 执行shoppingPanel的点击事件
    private void ExcuteClickEvent()
    {
        shoppingPanel.UpdateTotalPrice();
        shoppingPanel.HideDownObj();
    }

    // 选中当前眼镜对应的商品
   public void OnSelect()
    {
        isSelect = !isSelect;
        if(!isSelect)
        {
            EventCenter.Broadcast(EGameEvent.Event_CancelSelect);
        }
        Debug.Log(isSelect);

        if (isSelect)
        {
            selectBt.GetComponent<Image>().sprite = selectSpt;         
        }
        else
        {
            selectBt.GetComponent<Image>().sprite = normalSpt;          
        }

        ExcuteClickEvent();
    }

    //  删除时清除单个产品的数据
    void DeleteThis()
    {     
        if(isSelect)
        {
            isSelect = false;
            shoppingPanel.UpdateTotalPrice();
        }
        shoppingPanel.HideDownObj();
        shoppingPanel.glassPanelList.Remove(this);
        // TODO 把要删除的商品id ，上传服务器，更新服务器购物车信息
        AppConst.commdityIdList.Remove(this.id);

        WWWForm form = new WWWForm(); 
        form.AddField("id", this.id.ToString());
        byte[] data = form.data;
        NetManager.Instance.OnReceiveTextEvent(AppConst.delCartUrl, data, (string result) =>
        {
            string code = LitJson.JsonMapper.ToObject(result)["code"].ToString();
            if(code == "200")
            {
                Debug.Log("删除购物车成功");
                shoppingPanel.isUpdatePos = true;
                Destroy(this.gameObject);           
            }
            else
            {
                Debug.Log("商品不存在");
            }
        });

    }


}
