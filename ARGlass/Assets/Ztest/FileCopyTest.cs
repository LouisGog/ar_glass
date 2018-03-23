using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// copy文件 ，把从streamingAssets拷贝到persistentDataPath,然后删除本地文件，只有第一次会执行
/// </summary>

public class FileCopyTest : MonoBehaviour {


    private string srcPath;
    private string tarPath;


    void Start () {

        //  srcPath = Application.streamingAssetsPath + "/needfile";
        //   tarPath = Application.persistentDataPath;
        //   Debug.Log(tarPath);
        //   CopyFileUtil.CopyFolder(srcPath,tarPath);

        movefolder(@"C:\Users\xloong\Desktop\web2", @"C:\Users\xloong\Desktop\web");

        srcPath = @"C:\Users\xloong\Desktop\shidai";
      //  Invoke("destoryFile",3);
	}
	

    void destoryFile()
    {
        DirectoryInfo dirInfo = new DirectoryInfo(srcPath);
        deleteDirs(dirInfo);
        dirInfo = null;
        
    }

    /// <summary>
    ///  删除文件
    /// </summary>
    /// <param name="dirs"></param>
    void deleteDirs(DirectoryInfo dirs)
    {
        if((!dirs.Exists )|| dirs == null )
        {
            return;
        }

        DirectoryInfo[] subDir = dirs.GetDirectories();
        if(subDir != null)
        {
            for(int i= 0; i< subDir.Length; i++)
            {
                if(subDir[i] != null)
                {
                    deleteDirs(subDir[i]);
                }
            }
            subDir = null;
        }

        FileInfo[] files = dirs.GetFiles();
        if(files != null)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if(files[i] != null)
                {
                    Debug.Log("删除文件");
                    files[i].Delete();
                    files[i] = null;
                }
            }
            files = null;
        }
        Debug.Log("删除文件夹");
        dirs.Delete();
    }

    /// <summary>
    ///  修改文件夹名字
    /// </summary>
    /// <param name="SourceFolderName"></param>
    /// <param name="TargetFolderName"></param>
    public void movefolder(string SourceFolderName, string TargetFolderName)
    {
        try
        {
            Directory.Move(SourceFolderName, TargetFolderName);
            Debug.Log("完成修改");
            //return true;
        }
        catch
        {
            //return false;
            Debug.Log("异常");
        }
    }
}
