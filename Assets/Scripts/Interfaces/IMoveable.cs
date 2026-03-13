using UnityEngine;


/// <summary>
/// 移动接口
/// </summary>
public interface IMoveable
{
    /// <summary>
    /// 移动方法
    /// </summary>
    /// <param name="target">目标</param>
    void Move(Transform target);
}

