using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ͬɫ��Ʒ�ű����̳�ClearSweet�ű���
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

    //��дClear����
    public override void Clear()
    {
        base.Clear();
        sweet.gameManager.ClearColor(clearColorSweet);
    }
}
