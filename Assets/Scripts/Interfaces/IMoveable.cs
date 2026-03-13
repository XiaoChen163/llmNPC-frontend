using UnityEngine;

namespace UnityNPCDialogue.Interfaces
{
    /// <summary>
    /// 移动接口
    /// </summary>
    public interface IMoveable
    {
        /// <summary>
        /// NPC移动方法
        /// </summary>
        /// <param name="targetPos">目标位置</param>
        /// <param name="speed">移动速度</param>
        void Move(Vector3 targetPos, float speed);
    }
}
