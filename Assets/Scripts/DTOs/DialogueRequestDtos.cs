using Newtonsoft.Json;
using System;


// 获取历史上下文请求DTO
[Serializable]
public class DialogueHistoryRequestDto
{
[JsonProperty("username")]
public string Username { get; set; }

[JsonProperty("npc_name")]
public string NpcName { get; set; }
}

// 新增对话请求DTO
[Serializable]
public class NewDialogueRequestDto
{
[JsonProperty("username")]
public string Username { get; set; }

[JsonProperty("npc_name")]
public string NpcName { get; set; }

[JsonProperty("content")]
public string Content { get; set; }
}

// 删除对话请求DTO
[Serializable]
public class DeleteDialogueRequestDto
{
[JsonProperty("username")]
public string Username { get; set; }

[JsonProperty("npc_name")]
public string NpcName { get; set; }
}
