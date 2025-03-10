using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public Button btnCard;
    public Image imgCard;
    public Sprite[] clickSprites;
    public Sprite[] coveredSprites;
    public int id;
    public RectTransform rtf;
    public List<Card> aboveCardList = new List<Card>();//���ǵ�ǰ���Ƶ���������
    public List<Card> coverCardList = new List<Card>();//��ǰ���Ƹ��ǵ���������

    // Start is called before the first frame update
    void Start()
    {
        btnCard.onClick.AddListener(CardClickEvent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// ���ÿ�������ͼ����ID
    /// </summary>
    public void SetCardSprite()
    {
        id = Random.Range(1,15);
        imgCard.sprite = clickSprites[id - 1];
        SpriteState ss= btnCard.spriteState;
        ss.disabledSprite = coveredSprites[id - 1];
        btnCard.spriteState=ss;
    }
    /// <summary>
    /// ���õ�ǰ�������������Ƶĸ��ǹ�ϵ
    /// </summary>
    /// <param name="targetCard">Ŀ�꿨��</param>
    public void SetCoverCardState(Card targetCard)
    {
        //��������Ļ�������
        Vector2 cardPos= GUIUtility.GUIToScreenPoint(rtf.anchoredPosition); 
        //Ŀ�꿨����Ļ�������
        Vector2 targetCardPos= GUIUtility.GUIToScreenPoint(targetCard.rtf.anchoredPosition);
        if (Mathf.Abs(cardPos.x-targetCardPos.x)<Deck.Instance.cardWidth
            &&Mathf.Abs(cardPos.y-targetCardPos.y)<Deck.Instance.cardHeight)
        {
            //��������Ŀ�꿨��
            targetCard.AddAboveCard(this);
            targetCard.CloseButtonClickState();
            coverCardList.Add(targetCard);
        }
    }
    /// <summary>
    /// �رհ�ť����״̬
    /// </summary>
    public void CloseButtonClickState()
    {
        btnCard.interactable = false;
    }
    /// <summary>
    /// ���Ƶ���¼�
    /// </summary>
    public void CardClickEvent()
    {
        Deck.Instance.PlayClickSound();
        transform.SetSiblingIndex(500);
        //��Ҫ�Ƴ����б����Ǹ��ǵĿ��Ƴ��е���������������(���ǵ�ǰ����)
        for (int i = 0; i < coverCardList.Count; i++)
        {
            coverCardList[i].RemoveAboveCard(this);
            coverCardList[i].JudgeCanClickState();
        }
        int posID = -1;
        Transform targetTrans= Deck.Instance.GetPickDeckTargetTrans(id,out posID);
        transform.DOMove(targetTrans.position,0.5f).OnComplete
            (
                () => 
                {
                    if (targetTrans.childCount>0)
                    {
                        transform.SetParent(Deck.Instance.GetEmptyPickDeckTargetTrans(posID));
                    }
                    else
                    {
                        transform.SetParent(targetTrans);
                    }                   
                    transform.localPosition = Vector3.zero;
                    Deck.Instance.JudgeClearCard();
                }
            );
    }
    /// <summary>
    /// Ŀ�꿨�Ƹ����˵�ǰ���ƣ���������ȥ����Ŀ�꿨�Ƶķ�����
    /// </summary>
    public void AddAboveCard(Card targetCard)
    {
        aboveCardList.Add(targetCard);
    }
    /// <summary>
    /// �Ƴ���ǰ���Ʊ�Ŀ�꿨�Ƹ��ǵ����ã����������������ǵ����ǵĿ���(Ŀ�꿨��)ȥ���ã�
    /// </summary>
    /// <param name="aboveCard"></param>
    public void RemoveAboveCard(Card aboveCard)
    {
        aboveCardList.Remove(aboveCard);
    }
    /// <summary>
    /// �жϵ�ǰ�Ƿ���Ե��
    /// </summary>
    public void JudgeCanClickState()
    {
        btnCard.interactable = aboveCardList.Count <= 0;
    }
}
