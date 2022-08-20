using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 继承ClearSweet脚本，用于清除一整行或一整列
/// </summary>
public class ClearLine : ClearSweet
{
    #region
    //变量
    public bool isRow;//用于判断行消除还是列消除,false表示列消除
    #endregion

    /// <summary>
    /// 重写Clear方法
    /// </summary>
    public override void Clear()
    {
        base.Clear();

        if (isRow)
        {
            sweet.gameManager.ClearRow(sweet.Y);
        }
        else
        {
            sweet.gameManager.ClearColumn(sweet.X);
        }
    }
}
