using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 控制绵羊对象的移动和场景加载逻辑
/// 使用DOTween实现平滑移动动画，并管理异步场景加载过程
/// </summary>
public class SheepMove : MonoBehaviour
{
    public RectTransform rt;
    // 绵羊移动的目标点数组，每个元素是一个Transform对象
    public Transform[] movePointsTrans;
    // 存储移动目标点的本地位置信息
    private Vector3[] movePoints;
    // 是否在移动完成后加载新场景的标志
    public bool loadScene;
    // 异步场景加载操作对象，用于控制场景加载的时机
    private AsyncOperation ao;

    // Start is called before the first frame update
    void Start()
    {
        movePoints = new Vector3[movePointsTrans.Length];
        for (int i = 0; i < movePoints.Length; i++)
        {
            movePoints[i] = movePointsTrans[i].localPosition;
        }
        if (loadScene)
        {
            ao = SceneManager.LoadSceneAsync(1);
            ao.allowSceneActivation = false;
        }
        transform.DOLocalPath(movePoints, 3).SetEase(Ease.Linear).OnComplete
            (
            () =>
            {
                if (loadScene)
                {
                    ao.allowSceneActivation = true;
                }

            }
            );
    }

    // Update is called once per frame
    void Update()
    {
        if (rt.anchoredPosition.y >= 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }
    }
}
