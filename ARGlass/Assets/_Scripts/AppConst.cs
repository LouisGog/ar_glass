using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AppConst
{

    public static List<int> commdityIdList = new List<int>();                           //加入购物车的物品id
    public static string appName = "";                                                   // app名字

    public const string defultImageUrl = "http://wear.arglassteam.com";                     // 加载图片的默认地址前缀

    public const string commdityUrl = "http://wear.arglassteam.com/index";                       //商品列表Url
    public const string infoUrl = "http://wear.arglassteam.com/index/products/";                 //商品详情Url
    public const string shopCartUrl = "http://wear.arglassteam.com/api/index/cart";                  // 请求购物车Url
    public const string delCartUrl = "http://wear.arglassteam.com/api/index/delCart";                  // 删除购物车商品
    public const string payUrl = "http://wear.arglassteam.com/api/index/order/checkOut";               // 支付获取二维码请求地址
    public const string loginUrl = "http://wear.arglassteam.com/api/login";                              // 用户授权接口

    public const string pengdingStateUrl = "http://wear.arglassteam.com/api/index/pengdingStatus/";
    public static string order_id = "";
    // public static float price = 0;                                         //购物车物品的价格
   // public static int selectNum = 0;                                        //  选择的商品数量


    public static string dataToken = "";                     // 设备token
    public static string dataIndex_Url = "";                 // 商品列表处使用
}

public class GoodsInfo
{
    public int id;
    public int num;

    public GoodsInfo(int temId, int temNum)
    {
        this.id = temId;
        this.num = temNum;
    }

   
}
public class GoodsList
{
    public List<GoodsInfo> order = new List<GoodsInfo>();
}


