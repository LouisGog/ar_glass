using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 可以设定在支付成功后几秒钟后打开这个面板
/// </summary>

public class PaySuccessPanel : MonoBehaviour {

    private Transform bg;
    private Button tryBt;
    private Text timeText;
    private Button closeBt;

    private GameObject mirror;          // 镜子obj
    private bool isStartTimer ;         //支付成功后是否开始倒计时30s
    private bool isLookMirror;          //点击照一照后，判断是否开始照镜子

    private float timer ;
    int initValue ;         // 用来显示倒计时的时间

    void Awake () { 

        bg = transform.Find("bg");
        tryBt = bg.Find("try").GetComponent<Button>();
        timeText = bg.Find("time").GetComponent<Text>();
        closeBt = bg.Find("close").GetComponent<Button>();

        mirror = transform.Find("mirror").gameObject;
        //mirror.gameObject.SetActive(false);

        tryBt.onClick.AddListener(TryLook);
        closeBt.onClick.AddListener(ClosePanel);

      
	}

   
	void Update () {
		
        if(isStartTimer)
        {
            timer += Time.deltaTime;
            if(timer >= 1)
            {
                initValue -= 1;
                if (initValue < 0)
                {
                    initValue = 0;
                    ClosePanel();
                }
                timeText.text = "("+ initValue.ToString() + "s)";                         
                timer = 0;
            }
        }

        if (isLookMirror)
        {
            timer += Time.deltaTime;
            if (timer >= 1)
            {
                initValue -= 1;
                if (initValue < 0)
                {
                    ClosePanel();
                }
                timer = 0;
            }
        }
    }

    // 照镜子
    void  TryLook()
    {
        isStartTimer = false;
        initValue = 30;
        isLookMirror = true;

        // 这里显示镜子要不要设置为镜像模式
      //  GameController.instance.OpenCamera(mirror);
        if (!mirror.gameObject.activeSelf)
        {
            mirror.gameObject.SetActive(true);
        }
        
    }

    // 关闭支付成功界面
    void ClosePanel()
    {
        GameController.instance.currentState = AppState.OneScreen;
     //   GameController.instance.currentModel.SetActive(true);
        if(mirror.gameObject.activeSelf)
        {
            mirror.gameObject.SetActive(false);
        }
        string name = "unity_" + GameController.instance.needLoadModelId.ToString();
        EventCenter.Broadcast(EGameEvent.Event_CreateGlass,name);
        this.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        Init();
    }

    //初始化数据
    void Init()
    {
        timer = 0;
        isStartTimer = true;
        isLookMirror = false;
        initValue = 30;
        timeText.text = "(30s)";
    }
}
