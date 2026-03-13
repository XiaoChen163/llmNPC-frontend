using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityNPCDialogue.DTOs;

namespace UnityNPCDialogue.Network
{
    /// <summary>
    /// 对话HTTP服务（封装与后端的交互）
    /// </summary>
    public class HttpDialogueService : MonoBehaviour
    {
        // 服务器基础地址（替换为你的ASP.NET Core服务器地址）
        [Header("服务器配置")]
        [SerializeField] private string _baseApiUrl = "http://localhost:5000/api/v1/dialogues";
        private HttpClient _httpClient;

        private void Awake()
        {
            // 初始化HttpClient（设置超时）
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        /// <summary>
        /// 获取历史对话上下文（GET请求）
        /// </summary>
        public async Task<DialogueHistoryResponseDto> GetDialogueHistoryAsync(string username, string npcName)
        {
            try
            {
                // 构造GET请求URL（带Query参数）
                string url = $"{_baseApiUrl}/history?username={Uri.EscapeDataString(username)}&npc_name={Uri.EscapeDataString(npcName)}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<DialogueHistoryResponseDto>(json);
                }
                else
                {
                    Debug.LogError($"获取历史对话失败：{response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"网络请求异常：{e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 发送新对话请求（POST请求）
        /// </summary>
        public async Task<DialogueResponseDto> SendNewDialogueAsync(NewDialogueRequestDto request)
        {
            try
            {
                // 序列化请求体
                string jsonRequest = JsonConvert.SerializeObject(request);
                StringContent content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(_baseApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<DialogueResponseDto>(json);
                }
                else
                {
                    Debug.LogError($"发送新对话失败：{response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"网络请求异常：{e.Message}");
                return null;
            }
        }

        /// <summary>
        /// 删除指定NPC的对话记录（DELETE请求）
        /// </summary>
        public async Task<bool> DeleteDialogueHistoryAsync(string username, string npcName)
        {
            try
            {
                // 构造DELETE请求（带请求体）
                string url = $"{_baseApiUrl}/history";
                DeleteDialogueRequestDto request = new DeleteDialogueRequestDto
                {
                    Username = username,
                    NpcName = npcName
                };
                string jsonRequest = JsonConvert.SerializeObject(request);
                StringContent content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

                // HttpClient没有直接的DeleteAsync带请求体，需手动构造请求
                HttpRequestMessage deleteRequest = new HttpRequestMessage(HttpMethod.Delete, url)
                {
                    Content = content
                };
                HttpResponseMessage response = await _httpClient.SendAsync(deleteRequest);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Debug.LogError($"删除对话记录失败：{response.StatusCode}");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"网络请求异常：{e.Message}");
                return false;
            }
        }

        private void OnDestroy()
        {
            // 释放HttpClient资源
            _httpClient?.Dispose();
        }
    }
}