using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ʒ�ƶ��ű�������ÿ����Ʒ�����ƶ�������ɺͿհ���Ʒ�����ǲ����ƶ��ģ��൱���ϰ���
/// </summary>
public class MoveSweet : MonoBehaviour
{
    //����
    //�õ�����ָ��ʱ��ֹ���Э��
    private IEnumerator moveCoroutine;
    
    //����
    private GameSweet sweet;


    private void Awake()
    {
        //��ȡ��Ʒ��Ϸ����
        sweet = GetComponent<GameSweet>();
    }

    /// <summary>
    /// �������߽���Э�̣�������Ʒ�ƶ�����
    /// </summary>
    /// <param name="moveToX">��Ʒ�µ�Xλ��</param>
    /// <param name="moveToY">��Ʒ�µ�Yλ��</param>
    public void Move(int newX,int newY,float time)
    {
        if(moveCoroutine != null)
        {
            //���Э�̲�λ�գ���Э��ͣ����ΪʲôҪ�رվ�Э�̣������Ʒ����Э�������ƶ�֮�󣬷��������滹���Լ��������ƶ���Ҳ������Ҫ��һ����Э�̣������ʱ��Э�̲��رգ��¾�Э��ͬʱ������������߼����ң���Ʒ�ƶ��᲻���ɣ�
            StopCoroutine(moveCoroutine);
        }

        //���������ƶ���Э�̳���
        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    /// <summary>
    /// ��Ʒ�ƶ�Э�̳���
    /// ����ѧϰ��
    /// ���Բ�ֵ����Lerp()��unity3D�о��������Բ�ֵ����Lerp()��������֮���ֵ������֮���������������֮�䡢��������֮�䡢����������֮�䡢������ɫ֮��
    /// ����������Ϊ���ֵ������֮������Χ��0-1�����������õ�����ֵҲ���� a + ��b - a�� * t
    /// Lerp�������á��仯����ú�ƽ��
    /// </summary>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator MoveCoroutine(int newX,int newY,float time)
    {
        sweet.X = newX;
        sweet.Y = newY;
        //ÿһ֡�ƶ�һ���
        Vector3 startPos = transform.position;
        Vector3 endPos = sweet.gameManager.CorrectGridPosition(newX, newY);

        for(float t = 0; t < time; t += Time.deltaTime)
        {
            sweet.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            //�ȴ�һ֡
            yield return 0;
        }

        //���Э�̽�����Ʒλ�û�û��ָ��λ�ã���ô����Ʒλ��ǿ���ƶ���Ŀ��λ��
        sweet.transform.position = endPos;
    }
}
