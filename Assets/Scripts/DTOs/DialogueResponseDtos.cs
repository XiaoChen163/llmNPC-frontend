using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UnityNPCDialogue.DTOs
{
    // 单条对话响应DTO（匹配服务器返回的单个对话对象）
    [Serializable]
    public class DialogueResponseDto
    {
        [JsonProperty("role")]
        public string Role { get; set; } // "user" 或 "assistant"

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    // 历史对话列表响应（服务器返回的JSON数组）
    public class DialogueHistoryResponseDto : List<DialogueResponseDto> { }
}