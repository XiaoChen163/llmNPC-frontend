using System;
using System.Collections.Generic;

/// <summary>
/// 轻量化对话缓存器（仅本次程序运行有效）
/// </summary>
public class DialogueCache
{
    // 缓存Key："用户名_NPC名"（确保唯一标识玩家与NPC的对话）
    private Dictionary<string, DialogueHistoryResponseDto> _dialogueCache = new Dictionary<string, DialogueHistoryResponseDto>();

    /// <summary>
    /// 获取缓存的对话历史
    /// </summary>
    /// <param name="username">玩家用户名</param>
    /// <param name="npcName">NPC名称</param>
    /// <param name="history">输出缓存的历史对话</param>
    /// <returns>是否存在缓存</returns>
    public bool TryGetDialogueHistory(string username, string npcName, out DialogueHistoryResponseDto history)
    {
        string key = GetCacheKey(username, npcName);
        return _dialogueCache.TryGetValue(key, out history);
    }

    /// <summary>
    /// 存入对话历史到缓存
    /// </summary>
    public void SetDialogueHistory(string username, string npcName, DialogueHistoryResponseDto history)
    {
        string key = GetCacheKey(username, npcName);
        if (_dialogueCache.ContainsKey(key))
        {
            _dialogueCache[key] = history; // 覆盖旧缓存
        }
        else
        {
            _dialogueCache.Add(key, history);
        }
    }

    /// <summary>
    /// 新增单条对话到缓存（服务器回复后调用）
    /// </summary>
    public void AddDialogueToCache(string username, string npcName, DialogueResponseDto userDialogue, DialogueResponseDto npcDialogue)
    {
        string key = GetCacheKey(username, npcName);
        if (!_dialogueCache.ContainsKey(key))
        {
            _dialogueCache[key] = new DialogueHistoryResponseDto();
        }
        _dialogueCache[key].Add(userDialogue);
        _dialogueCache[key].Add(npcDialogue);
    }

    /// <summary>
    /// 清除指定NPC的缓存
    /// </summary>
    public void ClearNpcDialogueCache(string username, string npcName)
    {
        string key = GetCacheKey(username, npcName);
        if (_dialogueCache.ContainsKey(key))
        {
            _dialogueCache.Remove(key);
        }
    }

    // 生成唯一缓存Key
    private string GetCacheKey(string username, string npcName)
    {
        return $"{username}_{npcName}";
    }

    private static DialogueCache _instance;
    public static DialogueCache Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DialogueCache();
            }
            return _instance;
        }
    }
}
