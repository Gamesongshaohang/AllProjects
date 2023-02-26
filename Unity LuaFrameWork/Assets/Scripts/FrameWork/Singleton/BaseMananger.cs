using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Test
{
    void main() {
        objMananger.GetInstance();
    }
}

//忘记泛型约束可以去看下
public class BaseMananger<T> where T:new()
{
    private static T instance;
    public static T GetInstance() {
        if (instance == null) { instance = new T(); }
        return instance;
      
    }
}

public class objMananger : BaseMananger<objMananger>
{

}
