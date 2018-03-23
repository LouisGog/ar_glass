using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

/// <summary>
///  先打包，然后生成配置文件
/// </summary>
public class ExportAssetBundles : Editor{


   
    //private static string path = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/bundle";
    private static string resPath = Application.dataPath + "/StreamingAssets";//"/Res";         // 配置文件的生成路径

    [MenuItem("Build/ExportResource")]
	static void ExportResource()
	{
        // 打开保存面板，获取用户选择的路径  
        //string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "assetbundle");  
      
		Caching.CleanCache ();

		// 选择的要保存的对象  
		UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
		if (selection.Length <= 0) {  
			Debug.Log ("请选择被创建的物体");
			return;
		}
		for (int i = 0; i < selection.Length; i++) {
			
			string targetPath = ToolScript.WindowsAssetBundelPath;
			targetPath += "/" + selection[i].name + ".assetbundle";
			Debug.Log (targetPath);
			// LZ4格式打包  
 
			if(BuildPipeline.BuildAssetBundle(selection[0], selection, targetPath,
				BuildAssetBundleOptions.CollectDependencies |BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows))
			{
				Debug.Log(selection[i].name + "Windows资源打包成功");
			}
			else
			{
				Debug.Log(selection[i].name + "Windows资源打包失败");
			}
		}
		  
		//	BuildPipeline.BuildAssetBundles(path,BuildAssetBundleOptions.None,BuildTarget.StandaloneWindows);
			

	
        Debug.Log("build success");
    }



    [MenuItem("Build/GenerateVersionText")]
    static void GenerateVersionText()
    {

		Caching.CleanCache ();

        // 获取Res文件夹下所有文件的相对路径和MD5值  
		string[] files = Directory.GetFiles(ToolScript.WindowsAssetBundelPath, "*", SearchOption.AllDirectories);
        StringBuilder versions = new StringBuilder();
        for (int i = 0, len = files.Length; i < len; i++)
        {
            string filePath = files[i];
			//Debug.Log ("file" + " i "+filePath );
            string extension = filePath.Substring(files[i].LastIndexOf("."));
			if (extension == ".assetbundle")
            {

				string relativePath = filePath.Replace(ToolScript.WindowsAssetBundelPath , "").Replace("\\", "/");
                string md5 = MD5File(filePath);
				string temp = relativePath.Replace ("/", "");
				Debug.Log (relativePath);			
				versions.Append(temp).Append(",").Append(md5).Append("\n");
                
            }
        }

		if (!Directory.Exists (resPath)) {
			Directory.CreateDirectory (resPath);
		}
        // 生成配置文件  
        FileStream stream = new FileStream(resPath + "/version.txt", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(versions.ToString());
        stream.Write(data, 0, data.Length);
        stream.Flush();
        stream.Close();

		AssetDatabase.Refresh ();
    }


    public static string MD5File(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }
}
