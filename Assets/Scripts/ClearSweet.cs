using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ʒ�����ű�
/// </summary>
public class ClearSweet : MonoBehaviour
{
    //������
    //�ж��Ƿ�������������ڲ������������
    private bool isClearing;

    public AudioClip destroyAudio;
    public bool IsClearing
    {
        get
        {
            return isClearing;
        }
    }

    //����
    //��ȡ��Ʒ��������
    public AnimationClip clearAnimation;
    //��Ʒ���󣨺��������������е���չ���������Խ���������Ϊprotected���ͷ�����չ��
    protected GameSweet sweet;

    private void Awake()
    {
        sweet = GetComponent<GameSweet>();
    }

    /// <summary>
    /// ������Ҫ����������������䣬����������Ϊvirtual�鷽��
    /// </summary>
    public virtual void Clear()
    {
        //״̬Ϊ�������
        isClearing = true;
        //�������Э��
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if(animator != null)
        {
            animator.Play(clearAnimation.name);
            //��ҵ÷֣��������������
            GameManager.Instance.playerScore++;
            AudioSource.PlayClipAtPoint(destroyAudio, transform.position);
            //�ȴ������������
            yield return new WaitForSeconds(clearAnimation.length);
            Destroy(gameObject);//���ٵ�ǰ����
        }
    }
}
