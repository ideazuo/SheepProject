using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>
/// 控制绵羊对象的移动和场景加载逻辑
/// 使用DOTween实现平滑移动动画，并管理异步场景加载过程
/// </summary>
public class Sheeps : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform targetTrans;
    private AsyncOperation ao;
    public bool ifLoadGameScene;

    void Start()
    {
        // 初始化场景异步加载（场景索引需根据实际构建设置配置）
        // 注意：allowSceneActivation初始设为false以控制场景切换时机
        ao = SceneManager.LoadSceneAsync(2);
        ao.allowSceneActivation = false;
        transform.DOLocalMove(targetTrans.localPosition, 2).SetEase(Ease.Linear).OnComplete
            (
                OnCompleteEvent
            );
    }

    private void OnCompleteEvent()
    {
        if (ifLoadGameScene)
        {
            ao.allowSceneActivation = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
