using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine.UI;
using System;


/// <summary>
///  注意： 在编辑器的环境下，可以用Res文件夹读取bundle，但是 打包exe后却不能用，所以得使用StreamingAssets文件夹
/// </summary>

public class ResUpdate 
{
    public static readonly string VERSION_FILE = "version.txt";
  //  public static string LOCAL_RES_URL = "";
   // public  string SERVER_RES_URL = "";
    public static string LOCAL_RES_PATH = "";

    private Dictionary<string,string> LocalResVersion;
    private Dictionary<string,string> ServerResVersion;
    private List<string> NeedDownFiles;
    private bool NeedUpdateLocalVersionFile = false;
  //  public ItemCreator itemCreator;

    public ResUpdate()
    {
      //  itemCreator = new ItemCreator();
        //初始化
        LocalResVersion = new Dictionary<string, string>();
        ServerResVersion = new Dictionary<string, string>();
        NeedDownFiles = new List<string>();

      //  LOCAL_RES_URL = "file://" + Application.dataPath + "/StreamingAssets/"; //"/Res/";
        //SERVER_RES_URL = "file:///C:/Res/";
        LOCAL_RES_PATH = Application.dataPath + "/StreamingAssets/";

        UpdateNeedRes();
       
    }

    /// <summary>
    /// 更新服务器最新资源，有则更，没有则不更， 到时候可以根据多长时间来调用检测更新
    /// </summary>
    public void UpdateNeedRes()
    {
        //加载本地version配置    
        GameController.instance.StartCoroutine(DownLoad(ToolScript.Local_ResUrl + VERSION_FILE, delegate (WWW localVersion)
        {
            //保存本地的version    
            ParseVersionFile(localVersion.text, LocalResVersion);
            //加载服务端version配置    
            GameController.instance.StartCoroutine(this.DownLoad(ToolScript.server_ResPath + VERSION_FILE, delegate (WWW serverVersion)
            {
                //保存服务端version    
                ParseVersionFile(serverVersion.text, ServerResVersion);
                //计算出需要重新加载的资源    
                CompareVersion();
                //加载需要更新的资源    
                DownLoadRes();
            }));

            //#if UNITY_EDITOR
            //            UnityEditor.AssetDatabase.Refresh();  
            //#endif
        }));
    }

//    void Start()
//    {
//      //  needLoadResName = "cube";
//        //初始化    
//        LocalResVersion = new Dictionary<string, string>();
//        ServerResVersion = new Dictionary<string, string>();
//        NeedDownFiles = new List<string>();

//        LOCAL_RES_URL = "file://" + Application.dataPath + "/StreamingAssets/"; //"/Res/";
//		//SERVER_RES_URL = "file:///C:/Res/";
//		LOCAL_RES_PATH = Application.dataPath + "/StreamingAssets/";


//        //加载本地version配置    
//       GameController.instance.StartCoroutine(DownLoad(LOCAL_RES_URL + VERSION_FILE, delegate (WWW localVersion)
//        {
//            //保存本地的version    
//            ParseVersionFile(localVersion.text, LocalResVersion);
//        //加载服务端version配置    
//        GameController.instance.StartCoroutine(this.DownLoad(ToolScript.server_ResPath + VERSION_FILE, delegate (WWW serverVersion)
//            {
//                //保存服务端version    
//                ParseVersionFile(serverVersion.text, ServerResVersion);
//                //计算出需要重新加载的资源    
//                CompareVersion();
//                //加载需要更新的资源    
//                DownLoadRes();
//            }));

////#if UNITY_EDITOR
////            UnityEditor.AssetDatabase.Refresh();  
////#endif
//        }));
//    }

    //依次加载需要更新的资源    
    private void DownLoadRes()
    {
        if (NeedDownFiles.Count == 0)
        {
            UpdateLocalVersionFile();
            return;
        }

        string file = NeedDownFiles[0];
        NeedDownFiles.RemoveAt(0);

        GameController.instance.StartCoroutine(this.DownLoad(ToolScript.server_ResPath + file, delegate (WWW w)
        {           
            //将下载的资源替换本地就的资源    
            ReplaceLocalRes(file, w.bytes);
            DownLoadRes();
        }));
    }

    private void ReplaceLocalRes(string fileName, byte[] data)
    {
        string filePath = LOCAL_RES_PATH + fileName;
        FileStream stream = new FileStream(filePath, FileMode.Create);
        stream.Write(data, 0, data.Length);
        stream.Flush();
        stream.Close();
       
    }

    //更新本地的version配置    
    private void UpdateLocalVersionFile()
    {
        if (NeedUpdateLocalVersionFile)
        {
            StringBuilder versions = new StringBuilder();
            foreach (var item in ServerResVersion)
            {
                versions.Append(item.Key).Append(",").Append(item.Value).Append("\n");
            }

            FileStream stream = new FileStream(LOCAL_RES_PATH + VERSION_FILE, FileMode.Create);
            byte[] data = Encoding.UTF8.GetBytes(versions.ToString());
            stream.Write(data, 0, data.Length);
            stream.Flush();
            stream.Close();
        }
        //加载显示对象    
      //  StartCoroutine(Show(needLoadResName));
    }

    private void CompareVersion()
    {
        foreach (var version in ServerResVersion)
        {
            string fileName = version.Key;
            string serverMd5 = version.Value;
            //新增的资源    
            if (!LocalResVersion.ContainsKey(fileName))
            {
                NeedDownFiles.Add(fileName);
            }
            else
            {
                //需要替换的资源    
                string localMd5;
                LocalResVersion.TryGetValue(fileName, out localMd5);
                if (!serverMd5.Equals(localMd5))
                {
                    NeedDownFiles.Add(fileName);
                }
            }
        }
        //本次有更新，同时更新本地的version.txt    
		Debug.Log(NeedDownFiles.Count);
        NeedUpdateLocalVersionFile = NeedDownFiles.Count > 0;
    }

    private void ParseVersionFile(string content, Dictionary<string,string> dict)
    {
        if (content == null || content.Length == 0)
        {
            return;
        }
        string[] items = content.Split(new char[] { '\n' });
        foreach (string item in items)
        {
            string[] info = item.Split(new char[] { ',' });
            if (info != null && info.Length == 2)
            {
                dict.Add(info[0], info[1]);
            }
        }

    }

    private IEnumerator DownLoad(string url, HandleFinishDownload finishFun)
    {
        WWW www = new WWW(url);
        yield return www;
        if (finishFun != null)
        {
            finishFun(www);
        }
        www.Dispose();
    }

    public delegate void HandleFinishDownload(WWW www);
}
