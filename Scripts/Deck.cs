using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Deck : MonoBehaviour
{
    public static Deck Instance { get; set; }
    public int cardWidth = 213;
    public int cardHeight = 213;
    public GameObject cardGo;
    private int row = 7;
    private int column = 7;
    private int layer = 8;
    private List<Card> cards = new List<Card>();
    public RectTransform deckTrans;//卡牌需要生成到的位置的transform引用
    public Transform[] pickDeckPosTrans;//捡牌堆的格子位置
    public RectTransform centerDeckTrans;//中间牌堆的基础位置（原点）
    public RectTransform leftColumnDeckTrans;
    public RectTransform rightColumnDeckTrans;
    public RectTransform leftDownDeckTrans;
    public RectTransform rightDownDeckTrans;
    public int[] pickDeckCardIDs;//存放当前选中卡牌堆里的卡牌ID（跟当前位置一一对应）
    private int totalCardNum = 168;
    private int createCardNum = 0;

    public GameObject gameOverPanelGo;
    public AudioSource audioSource;
    public AudioClip clickSound;

    private int[,,] centerDeck = new int[,,]//层 行 列
    {
        //后5层
        {
            {0,0,0,0},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,0,0,0}
        },
        {
            {0,0,0,0},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,0,0,0}
        },
        {
            {0,0,0,0},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,0,0,0}
        },
        {
            {0,0,0,0},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,0,0,0}
        },
        {
            {0,0,0,0},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,2,2,2},
            {0,0,0,0}
        },
        //上两层
        {
            {0,0,0,0},
            {0,1,1,1},
            {0,0,1,1},
            {0,0,1,1},
            {0,1,2,2},
            {0,2,2,1},
            {0,0,0,0}
        },
        {
            {0,0,0,0},
            {0,1,1,3},
            {0,0,2,2},
            {3,3,3,3},
            {0,0,0,3},
            {0,0,3,3},
            {0,0,0,0}
        },
        //最上层
        {
            {0,3,1,2},
            {0,0,0,0},
            {3,2,2,2},
            {3,3,3,3},
            {0,0,2,2},
            {0,0,0,0},
            {0,0,1,3}
        }
    };

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pickDeckCardIDs = new int[7] { -1,-1,-1,-1,-1,-1,-1};

        //生成中间的一叠
        //层
        for (int k = 0; k < layer; k++)
        {
            //行
            for (int j = 0; j < row; j++)
            {
                //随机当前列是否偏移
                bool ifMoveX = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
                int dirX = 0;
                if (ifMoveX)
                {
                    //偏移方向是左还是右
                    //dirX = UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
                }
                //随机当前行是否偏移
                bool ifMoveY = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
                int dirY = 0;
                if (ifMoveY)
                {
                    //偏移方向是上还是下
                    //dirY = UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
                }
                //准备临时数组存贮前半部分随机到的卡牌状态，以对称生成后半部分状态
                CREATESTATE[] halfState = new CREATESTATE[column / 2];
                //列
                for (int i = 0; i < column; i++)
                {                    
                    GameObject go = null;
                    CREATESTATE cs;
                    if (i<=column/2)
                    {
                        //前半部分直接从三维数组取1 2 3 4 5 6 7
                        //                      2 1 0   0 1 2
                        cs = (CREATESTATE)centerDeck[k, j, i];
                        if (i!=column/2)
                        {
                            halfState[column / 2 - i - 1] = cs;
                        }
                    }
                    else
                    {
                        cs = halfState[i - column / 2 - 1];
                    }
                    switch (cs)
                    {
                        case CREATESTATE.NONE:
                            break;
                        case CREATESTATE.CREATE:
                            go = CreateCardGo(i, j, dirX, dirY);
                            break;
                        case CREATESTATE.RANDOM:
                            go = CreateCardGo(i, j, dirX, dirY);
                            if (UnityEngine.Random.Range(0,2)==0?true:false)
                            {
                                //随机生成
                                
                            }
                            break;
                        case CREATESTATE.ONLYCREATE:
                            go = CreateCardGo(i, j, 0, 0);
                            break;
                        default:
                            break;
                    }
                    if (go)
                    {
                        Card card = go.GetComponent<Card>();
                        card.SetCardSprite();
                        SetCoverState(card);
                        cards.Add(card);
                        createCardNum++;
                        go.name = "I:" + i.ToString() + " J:" + j.ToString() + " K:" + k.ToString();
                    }        
                }
            }
        }

        //生成其他四叠
        int createNum = (totalCardNum - createCardNum) / 4;
        int leftNum = totalCardNum - createCardNum - createNum * 4;
        //左竖
        for (int i = createNum + leftNum; i>0 ; i--)
        {
            CreateCard(leftColumnDeckTrans,0,-i);
        }
        //右竖
        for (int i = 0; i < createNum; i++)
        {
            CreateCard(rightColumnDeckTrans,0,-i);
        }
        //左下
        for (int i = 0; i < createNum; i++)
        {
            CreateCard(leftDownDeckTrans,i,0);
        }
        //右下
        for (int i = createNum; i >0 ; i--)
        {
            CreateCard(rightDownDeckTrans, i, 0);
        }
    }
    /// <summary>
    /// 生成旁边四叠卡牌
    /// </summary>
    /// <param name="zeroTrans"></param>
    /// <param name="indexX"></param>
    /// <param name="indexY"></param>
    private void CreateCard(RectTransform zeroTrans,int indexX,int indexY)
    {
        GameObject go= Instantiate(cardGo,deckTrans);
        RectTransform rft = go.GetComponent<RectTransform>();
        rft.anchoredPosition = zeroTrans.anchoredPosition +
            new Vector2(cardWidth*indexX*0.15f,cardHeight*indexY*0.15f);
        Card card = go.GetComponent<Card>();
        card.SetCardSprite();
        SetCoverState(card);
        cards.Add(card);
    }


    /// <summary>
    /// 产生卡牌游戏物体
    /// </summary>
    private GameObject CreateCardGo(int column,int row,int dirX,int dirY)
    {
        GameObject go = Instantiate(cardGo, deckTrans);
        go.GetComponent<RectTransform>().anchoredPosition
            = centerDeckTrans.anchoredPosition +
            new Vector2(cardWidth * (column + 0.5f * dirX), -cardHeight * (row + 0.5f * dirY));
        return go;
    }
    /// <summary>
    /// 设置当前新生成的卡牌与其他卡牌的覆盖关系
    /// </summary>
    /// <param name="card"></param>
    private void SetCoverState(Card card)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            card.SetCoverCardState(cards[i]);
        }
    }
    /// <summary>
    /// 获取当前捡牌堆的目标位置
    /// </summary>
    /// <param name="currentID">当前新选中的牌</param>
    /// <param name="posID">格子位置ID索引</param>
    /// <returns></returns>
    public Transform GetPickDeckTargetTrans(int currentID, out int posID)
    {
        posID = -1;
        for (int i = 0; i < pickDeckCardIDs.Length; i++)
        {
            //当前捡牌堆中的卡牌有没有与新选中的卡牌ID相等
            if (pickDeckCardIDs[i]==currentID&&i+1<=pickDeckCardIDs.Length)
            {
                posID = i + 1;
                return pickDeckPosTrans[i + 1];
            }
        }
        //当前捡牌堆没有与新选卡牌相同的ID
        Transform sf= GetEmptyPickDeckTargetTrans();
        if (sf)
        {
            return sf;
        }
        return null;
    }

    /// <summary>
    /// 获取选中卡牌堆的空位置
    /// </summary>
    /// <param name="posID">当前选中卡牌堆的格子位置索引ID</param>
    /// <returns></returns>
    public Transform GetEmptyPickDeckTargetTrans(int posID=-1)
    {
        for (int i = 0; i < pickDeckCardIDs.Length; i++)
        {
            if (pickDeckCardIDs[i]==-1)
            {
                //pickDeckCardIDs[i] = posID;
                //有两个或两个以下相同ID的卡牌，需要调整位置
                if (posID!=-1)
                {
                    pickDeckPosTrans[i].SetSiblingIndex(posID);
                }
                return pickDeckPosTrans[i];
            }
        }
        //Debug.Log("游戏结束");
        return null;
    }
    /// <summary>
    /// 排序卡牌位置和ID
    /// </summary>
    public void SortCardAndCardID()
    {
        SortCard();
        SortID();
    }
    /// <summary>
    /// 卡牌排序
    /// </summary>
    public void SortCard()
    {
        Transform[] tempTrans = new Transform[pickDeckPosTrans.Length];
        for (int i = 0; i < pickDeckPosTrans.Length; i++)
        {
            int siblingIndex= pickDeckPosTrans[i].GetSiblingIndex();
            tempTrans[siblingIndex] = pickDeckPosTrans[i];
        }
        for (int i = 0; i < pickDeckPosTrans.Length; i++)
        {
            pickDeckPosTrans[i] = tempTrans[i];
        }
    }
    /// <summary>
    /// ID排序
    /// </summary>
    public void SortID()
    {
        for (int i = 0; i < pickDeckCardIDs.Length; i++)
        {
            if (pickDeckPosTrans[i].childCount > 0)
            {
                pickDeckCardIDs[i] = pickDeckPosTrans[i].GetChild(0).GetComponent<Card>().id;
            }
            else
            {
                pickDeckCardIDs[i] = -1;
            }
        }
    }
    /// <summary>
    /// 卡牌消除判定方法
    /// </summary>
    public void JudgeClearCard()
    {
        SortCardAndCardID();
        //清除判定
        ClearCards();
        Invoke("SortGridPos",0.1f);
    }
    /// <summary>
    /// 消除
    /// </summary>
    public void ClearCards()
    {
        int sameCount = 0;
        int judgeID = -1;
        int startIndex = -1;
        for (int i = 0; i <pickDeckCardIDs.Length; i++)
        {
            //空格子
            if (pickDeckCardIDs[i]==-1)
            {
                break;
            }
            //当前三消判定开始的第一个元素
            if (sameCount==0)
            {
                sameCount++;
                judgeID = pickDeckCardIDs[i];
                startIndex = i;
            }
            else
            {
                if (judgeID == pickDeckCardIDs[i])
                {
                    sameCount++;
                }
                else
                {
                    sameCount = 1;
                    judgeID = pickDeckCardIDs[i];
                    startIndex = i;
                }
            }
            if (sameCount>=3)
            {
                for (int j = startIndex; j < startIndex+3; j++)
                {
                    pickDeckCardIDs[j] = -1;
                    Destroy(pickDeckPosTrans[j].GetChild(0).gameObject);
                }
                PlayClickSound();
                break;
            }
            if (i>= pickDeckCardIDs.Length-1)
            {
                gameOverPanelGo.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 排序消除后的格子位置（消除后补齐）
    /// </summary>
    private void SortGridPos()
    {
        for (int i = 0; i < pickDeckPosTrans.Length; i++)
        {
            if (pickDeckPosTrans[i].childCount<=0)
            {
                //空格子
                pickDeckPosTrans[i].SetSiblingIndex(6);
            }
        }
        SortCardAndCardID();
    }

    public void ReturnToMainScene()
    {
        SceneManager.LoadScene(1);
    }

    public void Replay()
    {
        SceneManager.LoadScene(2);
    }

    public void PlayClickSound()
    {
        audioSource.PlayOneShot(clickSound);
    }
}
/// <summary>
/// 卡牌的生成状态
/// </summary>
public enum CREATESTATE
{
    NONE,//该位置不生成卡牌
    CREATE,//生成并且位置可能偏移
    RANDOM,//可能生成也可能偏移
    ONLYCREATE//生成一定不偏移
}
