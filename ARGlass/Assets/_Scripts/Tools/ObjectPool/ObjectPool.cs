using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{

    public List<Transform> despawnedTrans;
    //public AnimalTransformInfo origianTran;

    public Transform prefab;
    public Transform root;

    public int curTranCount = 0;

    public int TranCount
    {
        get
        {
            return curTranCount;
        }
    }


    public ObjectPool(Transform _prefab, Transform _root)
    {
        despawnedTrans = new List<Transform>();
     //   origianTran = new AnimalTransformInfo();
        prefab = _prefab;
        root = _root;

        prefab.SetParent(root);
        prefab.gameObject.SetActive(false);
    }


    public int DespawnedCout
    {
        get
        {
            return despawnedTrans.Count;
        }
    }



    /// <summary>
    /// 从池中取出一个对象
    /// </summary>
    /// <returns></returns>
    public Transform Spwan()
    {
        if(despawnedTrans.Count == 0)
        {
            CreateInstance();
        }

        Transform tran = despawnedTrans[0];
        //origianTran.initPos = tran.position;
        //origianTran.initRoate = tran.rotation;
        //origianTran.initScale = tran.localScale;

        despawnedTrans.RemoveAt(0);
        tran.gameObject.SetActive(true);
        tran.parent = null;
        return tran;
    }


    public void DeSpawn(Transform tran, bool parentChange = true)
    {
        if(despawnedTrans.Contains(tran))
        {
            return;
        }
       
        if(parentChange)
        {
            tran.SetParent(root);
        }

        //tran.position = origianTran.initPos;
        //tran.rotation = origianTran.initRoate;
        //tran.localScale = origianTran.initScale;

        despawnedTrans.Add(tran);
        tran.gameObject.SetActive(false);
    }


    /// <summary>
    /// 创建新的一个对象
    /// </summary>
    public void CreateInstance()
    {
        GameObject obj = GameObject.Instantiate(prefab.gameObject);
        obj.transform.SetParent(root);
        despawnedTrans.Add(obj.transform);
        obj.name = prefab.name;
       // obj.name = string.Concat(prefab.name, curTranCount++);   修改名字
    }

    /// <summary>
    /// 在池中一次创建多个新的对象
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public IEnumerator CreateInstances(int count)
    {
        for(int i =0;i < count;i++)
        {
            CreateInstance();
            yield return null;
        }
    }


    /// <summary>
    /// 释放池
    /// </summary>
    public void Dispose()
    {
        curTranCount = 0;

        despawnedTrans.ForEach(delegate (Transform tran)
        {
            if(tran != null)
            {
                GameObject.Destroy(tran.gameObject);
            }
        });

        if(prefab != null)
        {
            GameObject.Destroy(prefab.gameObject);
        }

        despawnedTrans = null;
    }
}
