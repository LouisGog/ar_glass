using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassModel {

    // 眼镜模型名字
    public string name { get; set; }

    // 眼镜模型ID
    public string id { get; set; }

    // 眼镜价格
    public string piece { get; set; }

    //眼镜对应的图片地址
    public string imageUrl { get; set; }

    //眼镜数量
    public string num { get; set; }

    public GlassModel ()
    {

    }

    public GlassModel(string name, string id, string piece, string imageUrl, string num)
    {
        // createModel(name,id,piece,imageUrl,num);
        this.name = name;
        this.id = id;
        this.piece = piece;
        this.imageUrl = imageUrl;
        this.num = num;
    }

   
}
