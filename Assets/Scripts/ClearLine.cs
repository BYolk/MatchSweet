using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �̳�ClearSweet�ű����������һ���л�һ����
/// </summary>
public class ClearLine : ClearSweet
{
    #region
    //����
    public bool isRow;//�����ж�����������������,false��ʾ������
    #endregion

    /// <summary>
    /// ��дClear����
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
