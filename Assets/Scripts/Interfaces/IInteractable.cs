using UnityEngine;

namespace UnityNPCDialogue.Interfaces
{
    /// <summary>
    /// 交互接口
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// 玩家与NPC交互
        /// </summary>
        /// <param name="playerTransform">玩家位置（用于定位对话UI）</param>
        void Interact(Transform playerTransform);
    }
}
