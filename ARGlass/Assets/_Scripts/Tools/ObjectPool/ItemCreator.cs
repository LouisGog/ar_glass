using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class ItemCreator {

    public Dictionary<string, ObjectPool> basePools;

    private Transform poolRoot;


    public ItemCreator()
    {
        basePools = new Dictionary<string, ObjectPool>();
        poolRoot = new GameObject("PoolRoot").transform;

     //   MonoBehaviour.DontDestroyOnLoad(poolRoot.gameObject);
    }


    /// <summary>
    /// 创建一个对象池
    /// </summary>
    /// <param name="prefabObj"></param>
    public  void AddPool(GameObject prefabObj)
    {
        AddPool(prefabObj.name, prefabObj, poolRoot);

    }

    /// <summary>
    /// 创建一个池对象并添加到Dic中
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="prefabObj"></param>
    /// <param name="root"></param>
    public void AddPool(string prefabName ,GameObject prefabObj ,Transform root)
    {
       
        if (basePools.ContainsKey(prefabName))
        {
            return;
        }
        ObjectPool prefabPool = new ObjectPool(prefabObj.transform, root);
        //  这里修改一下，直接把生成的pab添加到某个池中，避免一下生成两个的情况
        prefabPool.despawnedTrans.Add(prefabObj.transform);
        basePools.Add(prefabName, prefabPool);
        
    }

    /// <summary>
    /// 从一个池中取出一个对象
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Transform SpawnObjByName(string name )
    {
        if(!basePools.ContainsKey(name))
        {
            GameObject newPoolObj = MonoBehaviour.Instantiate(LoadObj(name));
            newPoolObj.name = name;
            newPoolObj.AddComponent<GlassCtrl>();
            AddPool(newPoolObj);

            return basePools[name].Spwan();

        }
        return basePools[name].Spwan();
    }
     
    //public T SpawnObj<T>(this Transform trans,string name) where T: Component
    //{
    //    if (!basePools.ContainsKey(name))
    //    {
    //        T newPoolObj = MonoBehaviour.Instantiate(Resources.Load<T>(name));
    //        newPoolObj.name = name;
    //        AddPool(newPoolObj);
    //        return  basePools[name].Spwan();
    //    }
    //    return null;
    //}

    /// <summary>
    /// 将对象还给池
    /// </summary>
    /// <param name="nam"></param>
    /// <param name="tran"></param>
    /// <param name="parentChange"></param>
    public void DeSpawn(string nam, Transform tran, bool parentChange =true)
    {
        if(!basePools.ContainsKey(nam))
        {
            return;
        }

        basePools[nam].DeSpawn(tran,parentChange);
    }


    /// <summary>
    /// 从assetbundle中加载资源
    /// </summary>
    /// <param name="resName">当www加载的时候，路径前必须加file:// ，也就是加载文件的模式；但如果是通过内存加载，则只需要知道其路径即可</param>
    /// <returns></returns>
    private GameObject LoadObj(string resName)
    {
        string finalPath = ToolScript.Local_ResPath + resName + ".assetbundle";
        AssetBundle ab = AssetBundle.LoadFromMemory(File.ReadAllBytes(finalPath));  // 这种方式适合加载自己进行加密的ab包
        // LZ4格式的ab包，用LoadFromFile加载，可以将AssetBundle文件压缩，又可以兼顾加载速度，且节约内存
       // AssetBundle ab = AssetBundle.LoadFromFile(finalPath);  
        GameObject temp = (GameObject)ab.LoadAsset(resName);
        return temp;
    }

    #region  www加载资源ab包
    /// <summary>
    /// 从本地加载资源
    /// </summary>
    /// <param name="resName"></param>
    //private void LoadAssetModel(string resName)
    //{
    //    // GameController.instance.StartCoroutine(Show(resName));
    //    GameController.instance.StartCoroutine(Show(resName, () => {
    //        return basePools[resName].Spwan();
    //    }));
    //}

    ////从本地加载并显示资源    
    //private IEnumerator Show(string resName,Func<Transform> func)
    //{
    //    //  Debug.Log(LOCAL_RES_PATH + "cube.assetbundle");
    //    string path = ToolScript.Local_ResUrl + resName + ".assetbundle";//"sphere.assetbundle";
    //    Debug.Log(path);

    //    if (path != null)
    //    {
    //        yield return path;
    //    }
    //    WWW asset = new WWW(path);
    //    yield return asset;
    //    if (asset.isDone)
    //    {
    //        AssetBundle bundle = asset.assetBundle;

    //GameObject newObj = UnityEngine.Object.Instantiate(bundle.LoadAsset<GameObject>(resName));
    //        newObj.AddComponent<GlassCtrl>();
    //        newObj.name = resName;
    //        AddPool(newObj);
    //        Debug.Log(asset.assetBundle.name);
    //        newObj.transform.position = new Vector3(0, 0, -5);//Vector3.zero;
    //        bundle.Unload(false);

    //    }
    //    yield return new WaitForEndOfFrame();
    //    asset.Dispose();
    //    // 如果func不等于null， 则执行
    //    if (func != null)
    //    {
    //        func();
    //    }

    //}
    #endregion



}
