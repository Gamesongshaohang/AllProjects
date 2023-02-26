using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Test1 : SingletonMono<Test1> {
    protected override void Awake()
    {
        base.Awake();  
    }
}


//继续mono有个局限性，因为脚本可以挂载多个游戏对象上都会执行Awake方法，而instance是等于最后创建的那个脚本对象，就不是创建了一次了
//需要我们主观的确保它的唯一性而且也需要挂载了这个脚本的游戏对象在场景上跑起来才行，所以SingletonAutoMono解决这个问题
public class SingletonMono<T> : MonoBehaviour where T: MonoBehaviour //这里T限制类型为mono的子类
{
    private static T instance;
    public static T GetInstance() {
        //继承mono的类不能通过new实例化对象，只能通过脚本的拖曳或者Api AddComponent
        //之后u3d内部帮我们实例化类对象
        return instance;
    }


    //因为子类也可能用到Awaken方法，会被重写所以这个要改成虚函数保留instance的是实例化
    protected virtual void Awake()
    {
 
        instance = this as T; //this显示转换为T类型
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
