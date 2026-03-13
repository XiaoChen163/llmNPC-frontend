using UnityEngine;


/// <summary>
/// 交互接口
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// 可交互物接口
    /// </summary>
    /// <param name="playerTransform">记录发起互动的玩家</param>
    void Interact(Transform playerTransform);
}

