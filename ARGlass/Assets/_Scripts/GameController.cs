using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Threading;


public class GameController : MonoBehaviour {

	public static GameController instance;

    //[HideInInspector]
    //public string needLoadModelName;
    [HideInInspector]
    public int needLoadModelId;     //当前需要加载的眼镜模型的id

    [HideInInspector]
    public GameObject currentModel;             // 当前眼镜模型
    [HideInInspector]
    public GameObject anotherModel;               // 另一个镜头显示的眼镜模型

    private GameObject lastModel;               // 中间变量的眼镜模型

    public Image img;                   // 渲染摄像头画面的img
    
    public GameObject anotherCamera;        // 双屏显示的时候的另一个摄像头


    public AppState currentState;           // 当前状态
    private ResUpdate resUpdateTool;        // 热更新工具类
    private ItemCreator itemCreator;             // 对象池生成器

    private WebCamTexture webCam;           // 用来显示画面的webtexture
    FacePose f;                             // 用c#构造c++那边的类
    private double[] glassScale = new double[3] { 1, 1, 1 };        // 检测函数那里需要传递的眼镜大小的参数
 
    private double[] Mat4X4 = new double[16] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };        //检测函数那里需要传递的眼镜的姿态参数

    double[] glassPoints;       //  检测函数那里需要传递的眼镜上的两个鼻梁点，即眼镜模型的两个鼻梁点的坐标

    bool init;              // 人脸检测的初始化函数是否初始化 
    StringBuilder sb;           // 人脸初始化函数用到的读取文件的最终路径
   
    private string srcPath;         // 人脸初始化函数用到的读取文件在unity里的文件夹位置
    private string tarPath;         // 读取文件需要写入的位置

    void Awake()
    {
        instance = this;
        currentModel = null;
       
        //  c++里面要想读取字符串，得在字符串的尾部要加上 \0， 以\0 作为截止
        string path = Application.persistentDataPath + "/native\0";
        sb = new StringBuilder(path.Length);
        sb.Insert(0, path, path.Length);
        Application.targetFrameRate = 100;

        // 第一次启动的时候copy 文件到 Application.persistentDataPath 文件夹下
        srcPath = Application.streamingAssetsPath + "/needfile";
        tarPath = Application.persistentDataPath;
        if (!Directory.Exists(tarPath + "/native"))
        {
            CopyFileUtil.CopyFolder(srcPath, tarPath);
        }

        AddEvent();
    }

    // 添加事件
    private void AddEvent()
    {
        EventCenter.AddListener<string>(EGameEvent.Event_CreateGlass, LoadCurrentGlassByName);
        EventCenter.AddListener<bool>(EGameEvent.Event_DeleteGlass, ResetTwoScreenModel);
        EventCenter.AddListener(EGameEvent.Event_InitTwoScreen, InitCompareMode);
    }
    // 移除事件
    private void RemoveEvent()
    {
        EventCenter.RemoveListener< string>(EGameEvent.Event_CreateGlass, LoadCurrentGlassByName);
        EventCenter.RemoveListener<bool>(EGameEvent.Event_DeleteGlass, ResetTwoScreenModel);
        EventCenter.RemoveListener(EGameEvent.Event_InitTwoScreen, InitCompareMode);
    }

    private void Start()
    {
        resUpdateTool = new ResUpdate();
        itemCreator = new ItemCreator();
        currentState = AppState.OneScreen;
      
        //先是左边的点，然后右边的点  这个点很关键，会影响眼镜的旋转以及移动时候的误差
        glassPoints = new double[6] { -0.01f, 0f, 0.047f, 0.01f, 0f, 0.047f };
         
        f = new FacePose();

        StartCoroutine(CallCamera(img));

        Invoke("Init",0.5f);
        
    }


    // 当单屏的时候通过名字加载current模型
    private void LoadCurrentGlassByName(string name)
    {

        StartCoroutine(CreateCurrentGlass(name));
    }

    /// <summary>
    /// 名字加载another模型
    /// </summary>
    /// <param name="name"></param>
    public void LoadAnotherGlass(string name)
    {
        StartCoroutine(CreateAnotherGlass(name));
    }

    IEnumerator CreateCurrentGlass(string name)
    {
        GameObject glass = itemCreator.SpawnObjByName(name).gameObject;

        if(currentModel == null)
        {
            currentModel = glass;
            currentModel.name = name;
            yield return new WaitForEndOfFrame();
            currentModel.SetActive(true);
            Debug.Log(currentModel.name);
        }
        else
        {
            currentModel.SetActive(false);
            lastModel = currentModel;
            currentModel = glass;
            currentModel.name = name;
            yield return new WaitForEndOfFrame();
            currentModel.SetActive(true);
            itemCreator.DeSpawn(lastModel.name, lastModel.transform);
            lastModel = null;
        }

        SetLayer(currentModel,"one");
    }

    IEnumerator CreateAnotherGlass(string name)
    {
        GameObject glass = itemCreator.SpawnObjByName(name).gameObject;

        if (anotherModel == null)
        {
            anotherModel = glass;
            anotherModel.name = name;
            yield return new WaitForEndOfFrame();
            anotherModel.SetActive(true);
            Debug.Log(anotherModel.name);
        }

        SetLayer(anotherModel,"two");
    }

    //双屏模式下重置模型
    private void ResetTwoScreenModel(bool need = false)
    {
        if(need)
        {
            if(currentModel != null)
            {
                currentModel.SetActive(false);
                HideCurrentGlass();
            }  
        }    
        if(anotherModel != null)
        {
            anotherModel.SetActive(false);
            //  SetLayer(anotherModel, "one");
            itemCreator.DeSpawn(anotherModel.name, anotherModel.transform);
            anotherModel = null;
        }
        
    }

    // 设置模型的层
    private void SetLayer(GameObject obj, string name)
    {
        foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = LayerMask.NameToLayer(name);
           // Debug.Log(t.name);
        }
        
    }

    // 人脸初始化函数
    void Init()
    {
        // 这里初始化的时候，得获取设备摄像头拍摄画面的宽高的一半(即为相机的主点)，否则如果不正确的取值会造成中心点的偏移，导致匹配的眼镜的初始角度有个旋转角
         //前两个数字的参数是摄像机的焦距，改变大小后会影响眼镜的z值，后两个是相机的主点，一般来说相机的主点就是图片的中心
         //这里把图片上移，相当于是改变了相机的主点，所以y值应该增加相应移动的一半（即为相机此刻的主点位置）,上加下减， 左加右减
         // init = f.callNativeInitFaceDetectorWithPath(sb, 500, 500, 320, 240 + 250);
        init = f.callNativeInitFaceDetectorWithPath(sb, 500, 500, webCam.width / 2, webCam.height / 2);
        LoadCurrentGlassByName("unity_1");
        needLoadModelId = 1;
      //  StartThread();
        
    }
    #region 线程
    //Thread thread;
    //private void StartThread()
    //{
    //    thread = new Thread(new ThreadStart(zhuanhua));
    //    thread.IsBackground = true;
    //    thread.Start();

    //}
    //object locked = new object();
    //void zhuanhua()
    //{
    //    IntPtr pObjects = IntPtr.Zero;
    // //   Debug.Log("lvyifan ");
    //    while (true)
    //    {
    //        lock(locked)
    //        {

    //           if(!init)
    //            {
    //                continue;
    //            }
    //            if(dataQ.Count == 0)
    //            {
    //              //  Debug.Log("yingying");
    //                continue;
    //            }
    //            Debug.Log(dataQ.Count);
    //            pObjects = dataQ.Dequeue();

    //           // Debug.Log("asdfasfasf");
    //            double values = f.callNativtEstimateGlassPoseMat4X4(640, 480, 4, pObjects, glassPoints, Mat4X4, glassScale);
    //            pObjects = IntPtr.Zero;

    //            valueQ.Enqueue(values);
    //            Debug.Log(valueQ.Count + " asdflkajsdkfjals");
    //            Thread.Sleep(20);
    //        }
    //    }
    //}
    #endregion


    /// <summary>
    ///  初始化双屏模式
    /// </summary>
    private void  InitCompareMode()
    {
        if (!anotherCamera.activeSelf)
        {
            anotherCamera.SetActive(true);
        }
        currentState = AppState.TwoScreen;
    }

    /// <summary>
    /// 回收当前眼镜模型
    /// </summary>
    public void HideCurrentGlass()
    {
        if (currentModel != null)
        {
            itemCreator.DeSpawn(currentModel.name, currentModel.transform);
            currentModel = null;
        }
    }


    /// <summary>
    ///  返回 购物车面板
    /// </summary>
    public void BackToShoppingPanel()
    {
        HideCurrentGlass();
        if (anotherModel != null)
        {
            SetLayer(anotherModel, "one");
            itemCreator.DeSpawn(anotherModel.name, anotherModel.transform);
        }
        anotherModel = null;
        anotherCamera.SetActive(false);
        currentState = AppState.OneScreen;
    }

    // 打开cemera
    IEnumerator CallCamera( Image image )
    {
        yield return Application.HasUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            if (webCam != null)
                webCam.Stop();
            WebCamDevice[] camDevice = WebCamTexture.devices;
            //TODO  这里是开启第二个摄像头，仅测试用,之后到一体机上需要修改
            string name = camDevice[0].name;

            // webCam = new WebCamTexture(name, 640, 480,30);
            webCam = new WebCamTexture(name);

            if (image != null)
            {
                image.canvasRenderer.SetTexture(webCam);
            }
            webCam.Play();

        }
        yield return new WaitForEndOfFrame();
    
    }
 
    Color32[] colorData;            //图像数据
    GCHandle hObject;           //gc处理类
    IntPtr pObject;             
    [HideInInspector]
   public double value;         // c++检测人脸后返回回来的值

   // double[] tempPoint = new double[6];

   // Matrix4x4 mat;

    Vector3 rotateValue;        // 转化后获取到的眼镜旋转

    
    /// <summary>
    ///  把通过c++获取到的值赋给模型
    /// </summary>
    /// <param name="scaleValue"></param>
    /// <param name="Mat4X4"></param>
    void Assignment(double scaleValue,double[] Mat4X4)
    {
        float sx = (float)scaleValue;

        currentModel.transform.localScale = Vector3.one * sx * 0.88f;

        Vector3 forwardVec = new Vector3((float)Mat4X4[2], (float)Mat4X4[6], (float)Mat4X4[10]);
        Vector3 upwardsVec = new Vector3((float)Mat4X4[1], (float)Mat4X4[5], (float)Mat4X4[9]);
        Quaternion temQt = Quaternion.LookRotation(forwardVec,upwardsVec);

        rotateValue = temQt.eulerAngles;//getRotationFromMatrix(mat).eulerAngles;

        float rx = (float)Math.Round(rotateValue.x, 1);
        float ry = (float)Math.Round(rotateValue.y, 1);
        float rz = (float)Math.Round(rotateValue.z, 1);

        currentModel.transform.rotation = Quaternion.Euler(rx, ry, rz);//tempQ ;

        Vector3 temVec = new Vector3((float)Mat4X4[3],(float)Mat4X4[7],(float)Mat4X4[11]);
        currentModel.transform.position = temVec;//getPositionFromMatrix(mat);
    }


    //public Queue<IntPtr> dataQ = new Queue<IntPtr>();
    //public Queue<double> valueQ = new Queue<double>();

    private void Update()
    {
        if(currentModel == null)
        {
            return;
        }
        // 这里可以设置不等于单屏或双屏时就return,但有个问题是当用户在ui界面选择的时候，如果直接走了，则判断不了当前
        // value值， 所以这个得和产品商量 
        if (currentState == AppState.LookMirror)
        {
            return;
        }
        //if(currentState != AppState.OneScreen || currentState != AppState.TwoScreen)
        //{
        //    return;
        //}

        if (webCam != null)
        {

            if (init)
            {

                colorData = webCam.GetPixels32();
                //这里将标记colorData暂时不能被gc回收，并且固定对象的地址
                hObject = GCHandle.Alloc(colorData, GCHandleType.Pinned);
                //这里将拿到非托管内存的固定地址，传给c++

                pObject = hObject.AddrOfPinnedObject();
                //  dataQ.Enqueue(pObject);
               
                value = f.callNativtEstimateGlassPoseMat4X4(webCam.width, webCam.height, 4, pObject, glassPoints, Mat4X4, glassScale);
                
                pObject = IntPtr.Zero;
                //使用完毕后，将其handle free，这样c#可以正常gc这块内存
                hObject.Free();
                //if(valueQ.Count == 0)
                //{
                //    return;
                //}
                //value = valueQ.Dequeue();
                if (value != 0)
                {
                    Assignment(glassScale[0], Mat4X4);
                    #region nohelp
                    // sw.Stop();
                    // Debug.Log(string.Format("total: {0} ms", sw.ElapsedMilliseconds));

                    //sx = (float)glassScale[0];

                    //mat.m00 = (float)Mat4X4[0];
                    //mat.m01 = (float)Mat4X4[1];
                    //mat.m02 = (float)Mat4X4[2];
                    //mat.m03 = (float)Mat4X4[3];
                    //mat.m10 = (float)Mat4X4[4];
                    //mat.m11 = (float)Mat4X4[5];
                    //mat.m12 = (float)Mat4X4[6];
                    //mat.m13 = (float)Mat4X4[7];
                    //mat.m20 = (float)Mat4X4[8];
                    //mat.m21 = (float)Mat4X4[9];
                    //mat.m22 = (float)Mat4X4[10];
                    //mat.m23 = (float)Mat4X4[11];
                    //mat.m30 = (float)Mat4X4[12];
                    //mat.m31 = (float)Mat4X4[13];
                    //mat.m32 = (float)Mat4X4[14];
                    //mat.m33 = (float)Mat4X4[15];


                    //currentModel.transform.localScale = Vector3.one * sx * 0.9f;//* 0.93f;//0.8f ;

                    //rotateValue = getRotationFromMatrix(mat).eulerAngles;

                    //rx = (float)Math.Round(rotateValue.x, 1);
                    //ry = (float)Math.Round(rotateValue.y, 1);
                    //rz = (float)Math.Round(rotateValue.z, 1);


                    //currentModel.transform.rotation = Quaternion.Euler(rx , ry, rz);//tempQ ;

                    //currentModel.transform.position = getPositionFromMatrix(mat);//* 1.05f;//* 0.91f;//new Vector3(xpos, ypos, zpos);
                    #endregion
                    // TODO : 在这里判断当前状态， 如果是双屏就判断
                    if (currentState == AppState.TwoScreen)
                    {
                        if (anotherModel != null)
                        {
                            anotherModel.transform.localScale = currentModel.transform.localScale;
                            anotherModel.transform.rotation = currentModel.transform.rotation;
                            anotherModel.transform.position = currentModel.transform.position;
                        }

                    }

                }
            }
        }

    }


    //由Matrix4*4提取Quaternion和Vector3
    Quaternion getRotationFromMatrix(Matrix4x4 m)
    {
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }

    Vector3 getPositionFromMatrix(Matrix4x4 m)
    {
       
        return m.GetColumn(3);
    }

    //  由Quaternion,Vector3构造Matrix4x4
    Matrix4x4 GetMatrix(Vector3 postion, Quaternion rotation, Vector3 scale)
    {
        Matrix4x4 m = Matrix4x4.TRS(postion, rotation, scale);
        return m;
    }



    void OnApplicationQuit()
    {
        init = false;
       // thread.Abort();
        f.Destroy();
    }

    void OnDestroy()
    {
        RemoveEvent();
    }
}


public class FacePose
{
    private IntPtr m_Object = IntPtr.Zero;
    public FacePose()
    {
        m_Object = createFacePose();
    }
    ~FacePose()
    {
        Destroy();
    }
    public void Destroy()
    {
        if (m_Object != IntPtr.Zero)
        {
            destroyFacePose(m_Object);
            m_Object = IntPtr.Zero;
        }
    }

    public bool callNativeInitFaceDetector(double fx, double fy, double cx, double cy)
    {
        if (m_Object == IntPtr.Zero)
            throw new Exception("No native object");
        bool n = nativtCallInitFaceDetector(m_Object, fx, fy, cx, cy);

        Debug.Log(n);
        Debug.Log("init!");
        return n;
    }



    public bool callNativeInitFaceDetectorWithPath(StringBuilder sb, double fx, double fy, double cx, double cy)
    {
        if (m_Object == IntPtr.Zero)
            throw new Exception("No native object");
        bool n = nativtCallInitFaceDetectorWithPath(m_Object, sb, fx, fy, cx, cy);

       // Debug.Log(n);
        //sDebug.Log("init!");
        return n;
    }


    public double callNativtEstimateGlassPoseMat4X4(int imageWidh, int imageHeight, int imageChannel, IntPtr data, double[] glassPoints, double[] Mat4X4, double[] scale)
    {
        if (m_Object == IntPtr.Zero)
            throw new Exception("No native object");
        double value = nativtCallEstimateGlassPoseMat4X4(m_Object, imageWidh, imageHeight, imageChannel, data, glassPoints, Mat4X4, scale);

        return value;
    }

    [DllImport("facePose")]
    private static extern IntPtr createFacePose();
    [DllImport("facePose")]
    private static extern void destroyFacePose(IntPtr obj);

    [DllImport("facePose")]
    private static extern bool nativtCallInitFaceDetector(IntPtr obj, double fx, double fy, double cx, double cy);


    [DllImport("facePose")]
    private static extern bool nativtCallInitFaceDetectorWithPath(IntPtr obj, StringBuilder sb, double fx, double fy, double cx, double cy);


    [DllImport("facePose")]
    private static extern double nativtCallEstimateGlassPoseMat4X4(IntPtr obj, int imageWidth, int imageHeight, int imageChannel, IntPtr data,
    double[] glassPoints, double[] Mat4X4, double[] scale);
}

// 不同路径下的文件拷贝
public static class CopyFileUtil
{


    public static void CopyFolder(string srcPath, string tarPath)
    {
        if (!Directory.Exists(srcPath))
        {
            //Directory.CreateDirectory(srcPath);
            Debug.Log("return");
            return;
        }
        if (!Directory.Exists(tarPath))
        {
            Directory.CreateDirectory(tarPath);
        }

        CopyFile(srcPath, tarPath);
        string[] directionName = Directory.GetDirectories(srcPath);
        foreach (string dirPath in directionName)
        {
            string directionPathTemp = tarPath + "\\" + dirPath.Substring(srcPath.Length + 1);
            CopyFolder(dirPath, directionPathTemp);
        }
    }




    public static void CopyFile(string srcPath, string tarPath)
    {
        string[] filesList = Directory.GetFiles(srcPath);
        foreach (string f in filesList)
        {
            string fTarPath = tarPath + "\\" + f.Substring(srcPath.Length + 1);
            if (File.Exists(fTarPath))
            {
                File.Copy(f, fTarPath, true);
            }
            else
            {
                File.Copy(f, fTarPath);
            }
        }

    }
}


public enum AppState
{
    Login,                  // 登录授权
    OneScreen,              // 单屏
    TwoScreen,              // 双屏
    Ads,                    // 广告
    LookMirror,              //  照镜子 

    UI                      //  ui界面
}