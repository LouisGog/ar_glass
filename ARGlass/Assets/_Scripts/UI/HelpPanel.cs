using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpPanel : MonoBehaviour {

    private Transform bgTran;
    private Transform kefuPanelTran;
    private Transform tuihuanPanelTran;
    private Transform weixiuPanelTran;

    private Button kefuBt;
    private Button tuihuanBt;
    private Button weixiuBt;
    private Button closeAllBt;

    private Button closeKefuPanelBt;
    private Button closeTuihuanPanelBt;
    private Button closeWeixiuPanelBt;

    private string kefuUrl = "http://wear.arglassteam.com/img/help.png";                // 客服服务的接口地址
    private string tuihuanUrl = "http://wear.arglassteam.com/img/help2.png";            //退换货的接口地址
    private string weixiuUrl = "http://wear.arglassteam.com/img/service.png";           //维修的接口地址

	void Start () {

        bgTran = transform.Find("bg");
        kefuPanelTran = transform.Find("kefuPanel");
        tuihuanPanelTran = transform.Find("tuihuanPanel");
        weixiuPanelTran = transform.Find("weixiuPanel");

        kefuBt = bgTran.Find("kefu").GetComponent<Button>();
        tuihuanBt = bgTran.Find("tuihuan").GetComponent<Button>();
        weixiuBt = bgTran.Find("weixiu").GetComponent<Button>();
        closeAllBt = bgTran.Find("close").GetComponent<Button>();

        closeKefuPanelBt = kefuPanelTran.Find("close").GetComponent<Button>();
        closeTuihuanPanelBt = tuihuanPanelTran.Find("close").GetComponent<Button>();
        closeWeixiuPanelBt = weixiuPanelTran.Find("close").GetComponent<Button>();


        closeAllBt.onClick.AddListener(CloseHelpPanel);
        kefuBt.onClick.AddListener(OpenKeFuPanel);
        weixiuBt.onClick.AddListener(OpenWeixiuPanel);
        tuihuanBt.onClick.AddListener(OpenTuihuanPanel);

        closeKefuPanelBt.onClick.AddListener(CloseKefuPanel);
        closeTuihuanPanelBt.onClick.AddListener(CloseTuihuanPanel);
        closeWeixiuPanelBt.onClick.AddListener(CloseWeixiuPanel);
    }
	
    // 关闭帮助面板
	void CloseHelpPanel()
    {
        this.gameObject.SetActive(false);
    }

    // 打开客服面板
    void OpenKeFuPanel()
    {
        OpenPanel(kefuUrl, kefuPanelTran);
    }

    // 关闭客服面板
    void CloseKefuPanel()
    {
        ClosePanel(kefuPanelTran);
    }

    // 打开退换货面板
    void OpenTuihuanPanel()
    {
        OpenPanel(tuihuanUrl,tuihuanPanelTran);
    }
    // 关闭退换货面板
    void CloseTuihuanPanel()
    {
        ClosePanel(tuihuanPanelTran);
    }

    // 打开维修面板
    void OpenWeixiuPanel()
    {
        OpenPanel(weixiuUrl,weixiuPanelTran);
    }

    // 关闭维修面板
    void CloseWeixiuPanel()
    {
        ClosePanel(weixiuPanelTran);
    }

    // 打开面板
    void OpenPanel(string url,Transform tran)
    {
        NetManager.Instance.OnReceiveImageEvent(url, null, (Texture2D t) => {
            Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
            tran.GetComponent<Image>().sprite = temp;
            tran.gameObject.SetActive(true);
            bgTran.gameObject.SetActive(false);
        });
    }
    // 关闭面板
    void ClosePanel(Transform tran)
    {
        tran.gameObject.SetActive(false);
        bgTran.gameObject.SetActive(true);
    }
}
