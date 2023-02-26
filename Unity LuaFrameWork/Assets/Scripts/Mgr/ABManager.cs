
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ABManager : SingletonMono<ABManager>
{
    //主包
    private AssetBundle mainAB = null;
    //主包依赖获取配置文件
    private AssetBundleManifest manifest = null;

    //选择存储 AB包的容器
    //AB包不能够重复加载 否则会报错
    //字典：存储加载过的AB包
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 获取AB包加载路径
    /// </summary> /// <summary>
    private string PathUrl
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    /// <summary>
    /// 主包名 根据平台不同 报名不同
    /// </summary>
    private string MainName
    {
        get
        {
        #if UNITY_IOS
                    return "IOS";
        #elif UNITY_ANDROID
                    return "Android";
        #else
                    return "PC";
        #endif
                }
    }

    public void LoadAB(string abName) {
        //加载主包
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainName);
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }

        //获取依赖包
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            if (!abDic.ContainsKey(strs[i]))
            {
                //加载所有的依赖包
                AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                abDic.Add(strs[i], ab);
            }
        }
        //加载目标包
        if (!abDic.ContainsKey(abName))
        {
            AssetBundle ab = AssetBundle.LoadFromFile(PathUrl + abName);
            abDic.Add(abName, ab);
        }
    }


    //同步加载----------------------------------------------------------------------------------
    //同步加载，不指定类型
    public Object LoadRes(string abName,string resName)
    {

        LoadAB(abName);
        //得到加载出来的资源
        Object obj = abDic[abName].LoadAsset(resName);
        //如果是GameObject 因为GameObject 100%都是需要实例化的
        //所以我们直接实例化
        if (obj is GameObject)
            return Instantiate(obj);
        else
            return obj;
    }

    //同步加载，指定类型
    //指定类型可以确保同名的资源，当成泛型也行不过更新新lua没有泛型
    public Object LoadRes(string abName, string resName,System.Type type)
    {
        LoadAB(abName);
        //得到加载出来的资源
        Object obj = abDic[abName].LoadAsset(resName, type);
        //如果是GameObject 因为GameObject 100%都是需要实例化的
        //所以我们直接实例化
        if (obj is GameObject)
            return Instantiate(obj);
        else
            return obj;
    }

    //同步加载，泛型
    public T LoadRes<T>(string abName, string resName) where T:Object
    {
        LoadAB(abName);
        //得到加载出来的资源
        T obj = abDic[abName].LoadAsset<T>(resName);
        //如果是GameObject 因为GameObject 100%都是需要实例化的
        //所以我们直接实例化
        if (obj is GameObject)
            return Instantiate(obj);
        else
            return obj ;
    }



    //异步加载----------------------------------------------------------------------------------

    /// <summary>
    /// 泛型异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack));
    }
    //正儿八经的 协程函数
    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        //加载依赖包
        LoadAB(abName);
     
        //异步加载包中资源
        AssetBundleRequest abq = abDic[abName].LoadAssetAsync<T>(resName);
        yield return abq;

        if (abq.asset is GameObject)
            callBack(Instantiate(abq.asset) as T);
        else
            callBack(abq.asset as T);
    }

    /// <summary>
    /// Type异步加载资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="type"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, type, callBack));
    }

    private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        //加载依赖包
        LoadAB(abName);

        //异步加载包中资源
        AssetBundleRequest abq = abDic[abName].LoadAssetAsync(resName, type);
        yield return abq;

        if (abq.asset is GameObject)
            callBack(Instantiate(abq.asset));
        else
            callBack(abq.asset);
    }

    /// <summary>
    /// 名字 异步加载 指定资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public void LoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
    }

    private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
        //加载依赖包
        LoadAB(abName);
  
        //异步加载包中资源
        AssetBundleRequest abq = abDic[abName].LoadAssetAsync(resName);
        yield return abq;

        if (abq.asset is GameObject)
            callBack(Instantiate(abq.asset));
        else
            callBack(abq.asset);
    }


    //指定包卸载
    //卸载AB包的方法
    public void UnLoadAB(string name)
    {
        if (abDic.ContainsKey(name))
        {
            abDic[name].Unload(false);
            abDic.Remove(name);
        }
    }

    //清空AB包的方法
    public void ClearAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        //卸载主包
        mainAB = null;
    }

}
