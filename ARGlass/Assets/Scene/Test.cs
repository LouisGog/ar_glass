using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {

    public Button fenpingBt;


    private WebCamTexture webCam;
    private Image img;
	private Transform fenpingTran;
	private List<Image> fenpingImgs;

	private bool isClick =false;

	void Start () {
      
		fenpingImgs = new List<Image> ();
		img = transform.Find("defaultImg").GetComponent<Image>();
		fenpingTran = transform.Find ("fenping");
		for (int i = 0; i < fenpingTran.childCount; i++) {
			fenpingImgs.Add (fenpingTran.GetChild (i).GetComponent<Image> ());
		}

		fenpingBt.onClick.AddListener(FenPing);
		StartCoroutine(CallCamera(null,img));
	}
	
	
	void Update () {
		

	}





    void FenPing()
    {
		isClick = !isClick;
		if (isClick) {
			img.gameObject.SetActive (false);
			fenpingTran.gameObject.SetActive (true);
			StartCoroutine (CallCamera (fenpingImgs, null));
			fenpingBt.GetComponentInChildren<Text> ().text = "还原";
		} else {
			img.gameObject.SetActive (true);
			fenpingTran.gameObject.SetActive (false);
			StartCoroutine(CallCamera(null,img));
			fenpingBt.GetComponentInChildren<Text> ().text = "分屏";
		}
    }

	IEnumerator CallCamera(List<Image> tempList = null,Image image = null )
    {
        yield return Application.HasUserAuthorization(UserAuthorization.WebCam);
        if(Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            if (webCam != null)
                webCam.Stop();
            WebCamDevice[] camDevice = WebCamTexture.devices;
            string name = camDevice[0].name;
            webCam = new WebCamTexture(name,Screen.width,Screen.height,60);

			if (tempList == null && image != null) {
				image.canvasRenderer.SetTexture (webCam);
			} else if (tempList != null && image == null) {
				foreach (Image img in tempList) {
					img.canvasRenderer.SetTexture(webCam);
				}
			}
			  
            webCam.Play();

        }
    }

}
