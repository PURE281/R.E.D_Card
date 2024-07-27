using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using zFramework.Extension;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<CardInfoBean> result = CsvUtility.Read<CardInfoBean>((GetPath() + "/StreamingAssets") + "/" + "CardDataTest1.csv");
        Debug.Log(result[0].id);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private static string GetPath()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {

        }
#if UNITY_EDITOR
        return Application.persistentDataPath;
#elif UNITY_ANDROID
			return Application.persistentDataPath;
#elif UNITY_IPHONE
			return GetiPhoneDocumentsPath();
#else
			return Application.dataPath;
#endif
    }
}
