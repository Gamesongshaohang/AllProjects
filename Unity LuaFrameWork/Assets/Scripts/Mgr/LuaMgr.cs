using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;


/// <summary>
/// Lua解析器
/// @desc:C#解析运行Lua环境代码
/// </summary>
public class LuaMgr : BaseMananger<LuaMgr>
{
    //方法1,执行lua语言的函数 DoString
    //方法2,释放垃圾 Tick
    //方法3,销毁
    //方法4,重定向 重新定义规则执行lua（包括查找lua的路径）,不指定的话有默认路径

    private LuaEnv luaEnv;

    /// <summary>
    /// lua解析器初始化
    /// </summary>
    public void Init() {
        if (luaEnv == null)
        {
            luaEnv = new LuaEnv();
            luaEnv.AddLoader(MyCustomLoader); //指定查找lua脚本为路径方法
        }
    
    }

    private byte[] MyCustomLoader(ref string filepath)
    {
        //现在路径是本地的真正开发肯定是ab包中的加载的
        string path = Application.dataPath + "/Lua/" + filepath + ".lua";
        if (File.Exists(path))
        {
            return File.ReadAllBytes(path);
        } //读取文件信息并返回btye
        else
        {
            Debug.Log("MyCustomLoader 查找lua文件失败，文件名为" + filepath);
            return null;
        }
    }

    /// <summary>
    /// 执行lua语句
    /// </summary>
    /// <param name="str"></param>
    public void DoString(string str) {
        if (luaEnv == null)
        {
            Debug.Log("解析器未初始化");
            return;
        }
        luaEnv.DoString(string.Format("require('{0}')", str));
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Tick() {
        if (luaEnv == null)
        {
            Debug.Log("解析器未初始化");
            return;
        }
        luaEnv.Tick();
    }

    /// <summary>
    /// 销毁解析器
    /// </summary>
    public void Dispose()
    {
        luaEnv.Dispose();
        luaEnv = null;
    }

}
