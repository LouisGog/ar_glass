using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using UnityEngine.Networking;

public delegate void DidReceiveTextSuccess(string text);
public delegate void DidReceiveImageSuccess(Texture2D image);
public delegate void DidReceiveError(string text);


public class NetManager : MonoBehaviour {

    public static NetManager Instance = null;

    //public static NetManager Instance
    //{
    //    get
    //    {
    //        if(instance == null)
    //        {
    //            GameObject go = new GameObject("NetManager");
    //            go.AddComponent<NetManager>();
    //        }
    //        return instance;
    //    }
    //}


    void Awake()
    {
        Instance = this;
        GetToken();
    }

    // 获取设备token 以及商品列表地址后缀
    void GetToken()
    {
        // 获取机器编码和key值，获取设备token以及之后商品列表处的url后缀
        WWWForm form = new WWWForm();
        form.AddField("code", "2017112901");
        form.AddField("key", "5a1d263740cec");
 
        StartCoroutine(ReceiveText(AppConst.loginUrl, form, (string result) => {

            JsonData rootData = JsonMapper.ToObject(result);

            string code = rootData["code"].ToString();
            if(code == "200")
            {
                JsonData data = rootData["data"];
                AppConst.dataToken = data["token"].ToString();
                AppConst.dataIndex_Url = data["index_key"].ToString();
            }
            else if(code == null)
            {
                Debug.Log("读取 token 出错！");
                GetToken();
            }
           
           // Debug.Log(AppConst.dataIndex_Url + "   0");
        }, null));
       
    }
    /// <summary>
    /// 接受text
    /// </summary>
    /// <param name="path"></param>
    /// <param name="form"></param>
    /// <param name="didReceiveText"></param>
    /// <param name="receiveError"></param>
    public void OnReceiveTextEvent(string path, byte[] postData ,  DidReceiveTextSuccess didReceiveText,  DidReceiveError receiveError = null)
    {
        StartCoroutine(GetTextInfoWithHeader(path, postData,didReceiveText,receiveError));
    }

    /// <summary>
    /// 接受image
    /// </summary>
    /// <param name="path"></param>
    /// <param name="form"></param>
    /// <param name="didReceiveImage"></param>
    /// <param name="receiveError"></param>
    public void OnReceiveImageEvent(string path, WWWForm form , DidReceiveImageSuccess didReceiveImage,  DidReceiveError receiveError = null)
    {
        StartCoroutine(ReceiveImage(path,null ,didReceiveImage,receiveError));
    }

    /// <summary>
    /// 获取付款二维码
    /// </summary>
    /// <param name="path"></param>
    /// <param name="form"></param>
    /// <param name="didReceiveText"></param>
   public void GetTextWithoutHeader(string path,WWWForm form, DidReceiveTextSuccess didReceiveText)
    {
        
        StartCoroutine(ReceiveText(path,form,didReceiveText));
    }

    IEnumerator ReceiveText(string path, WWWForm form , DidReceiveTextSuccess didReceiveTextSuccess, DidReceiveError receiveError = null)
    {
        WWW www;
        if (form != null)
        {
            www = new WWW(path,form);
        }
        else
        {
            www = new WWW(path);
        }
        yield return new WaitForEndOfFrame();
        if(www.error != null)
        {
            if(receiveError != null)
            {
                receiveError(www.error);
            }
        }
        else
        {
          
           yield return www;
            if(www.isDone)
            {
                didReceiveTextSuccess(www.text);
            }
        }
        yield return new WaitForEndOfFrame();
        www.Dispose();
    }

    IEnumerator ReceiveImage(string path, WWWForm form , DidReceiveImageSuccess didReceiveImageSuccess, DidReceiveError receiveError = null)
    {
        WWW www;
        if (form != null)
        {
            www = new WWW(path, form);
        }
        else
        {
            www = new WWW(path);
        }
        yield return new WaitForEndOfFrame();
        if (www.error != null)
        {
            if (receiveError != null)
            {
                receiveError(www.error);
            }
        }
        else
        {

            yield return www;
            if (www.isDone)
            {
                didReceiveImageSuccess(www.texture);
            }
        }
        yield return new WaitForEndOfFrame();
        www.Dispose();
    }
	


   
    IEnumerator GetTextInfoWithHeader(string path, byte[] postData, DidReceiveTextSuccess didReceiveTextSuccess, DidReceiveError receiveError = null)

    {
        WWW www;
        Dictionary<string, string> headers = new Dictionary<string, string>();
        string autho = "Bearer " + AppConst.dataToken;
      //  Debug.Log(autho + "AUTHO");
        headers["Authorization"] = autho;//"Bearer " + AppConst.dataToken;
        headers["Accept"] = "application/json";
        if (postData != null)
        {
            www = new WWW(path,postData,headers);
           
        }
        else
        {
            www = new WWW(path,null,headers);
        }
        yield return new WaitForEndOfFrame();
        if (www.error != null)
        {
            if (receiveError != null)
            {
                receiveError(www.error);
            }
        }
        else
        {

            yield return www;
            if (www.isDone)
            {
                didReceiveTextSuccess(www.text);
            }
        }
        yield return new WaitForEndOfFrame();
        www.Dispose();

    }

  

}
