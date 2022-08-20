using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 清除同色甜品脚本（继承ClearSweet脚本）
/// </summary>
public class ClearColor : ClearSweet
{
    private ColorSweet.ColorType clearColorSweet;
    public ColorSweet.ColorType ClearColorSweet
    {
        get
        {
            return clearColorSweet;
        }
        set
        {
            clearColorSweet = value;
        }
    }

    //重写Clear方法
    public override void Clear()
    {
        base.Clear();
        sweet.gameManager.ClearColor(clearColorSweet);
    }
}
