using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public static class ToolScript  {

    public const string AppName = "/Res/";

    private static string SERVER_RES_URL = "";
    private static string LOCAL_RES_URL = "";
    private static string LOCAL_RES_PATH = "";
    private static string theRootPath = "";

	/// <summary>
	/// WindowsAssetbundle资源
	/// </summary>
	public static string WindowsAssetBundelPath
	{
		set { }
		get
		{
			string temp = rootPath + "/WindowsAssetbundle";
			SetDirectory(temp);
			return temp;
		}
	}


	/// <summary>
	/// 查看目录是否存在,如果不存在则创建
	/// </summary>
	/// <param name="path"></param>
	public static void SetDirectory(string path)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
	}

    /// <summary>
    ///   打包资源后的资源路径
    /// </summary>
	public static string rootPath
	{
		set { }
		get
		{
			if (theRootPath == "")
			{
				theRootPath = Application.persistentDataPath;
			}

			return theRootPath;
		}
	}

    //  服务器资源地址
    public static string server_ResPath
    {
        set { }
        get
        {
            if(SERVER_RES_URL == "")
            {
                SERVER_RES_URL = "file:///C:/WindowsAssetbundle/"; //"http://192.168.1.123" + "/WindowsAssetbundle/";
            }
            return SERVER_RES_URL;
        }
    }

    /// <summary>
    /// 本地资源加载路径
    /// </summary>
    public static string Local_ResUrl
    {
        set { }
        get
        {
            if(LOCAL_RES_URL == "")
            {
                LOCAL_RES_URL = "file://" + Application.dataPath + "/StreamingAssets/";
            }
            return LOCAL_RES_URL;
        }
    }

    /// <summary>
    /// 本地资源地址
    /// </summary>
    public static string Local_ResPath
    {
        set { }
        get
        {
           
             return   LOCAL_RES_PATH = Application.dataPath + "/StreamingAssets/";
            
            
        }
    }
}
