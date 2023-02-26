using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//这个单例与SingletonMono不同的做法是，不通过在unity的生命周期中创建
//确保唯一性
//做法：如果对象为空时自己在场景中创建一个空对象
public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static T GetInstance()
    {
        if (instance == null) {
            GameObject obj = new GameObject();
            instance= obj.AddComponent<T>(); //这个就是脚本对象
            DontDestroyOnLoad(instance); //游戏对象如果切场景时移除，单例模式生产的对象也没了
        }
        return instance;
    }


   
}
