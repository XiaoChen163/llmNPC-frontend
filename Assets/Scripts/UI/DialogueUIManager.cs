using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityNPCDialogue.Cache;
using UnityNPCDialogue.DTOs;
using UnityNPCDialogue.Network;

namespace UnityNPCDialogue.UI
{
    public class DialogueUIManager : MonoBehaviour
    {
        [Header("UI元素")]
        [SerializeField] private TMP_Text _npcReplyText; // NPC回复文本
        [SerializeField] private TMP_InputField _playerInputField; // 玩家输入框
        [SerializeField] private Button _sendButton; // 发送按钮
        [SerializeField] private Button _historyButton; // 历史会话按钮
        [SerializeField] private GameObject _historyPanel; // 历史会话面板
        [SerializeField] private TMP_Text _historyContentText; // 历史会话文本

        [Header("逐字显示配置")]
        [SerializeField] private float _typewriterSpeed = 0.05f; // 逐字显示间隔

        private string _username; // 玩家用户名
        private string _npcName; // 当前交互的NPC名
        private HttpDialogueService _httpService;
        private bool _isWaitingForReply; // 是否等待服务器回复

        /// <summary>
        /// 初始化UI
        /// </summary>
        public void Init(string username, string npcName, HttpDialogueService httpService)
        {
            _username = username;
            _npcName = npcName;
            _httpService = httpService;

            // 绑定按钮事件
            _sendButton.onClick.AddListener(OnSendButtonClick);
            _historyButton.onClick.AddListener(ToggleHistoryPanel);

            // 初始隐藏历史面板
            _historyPanel.SetActive(false);
        }

        #region UI显示/隐藏
        public void ShowDialogueUI()
        {
            gameObject.SetActive(true);
        }

        public void HideDialogueUI()
        {
            gameObject.SetActive(false);
            _historyPanel.SetActive(false);
        }
        #endregion

        #region 历史对话加载
        /// <summary>
        /// 加载历史对话（优先缓存，无则请求服务器）
        /// </summary>
        public async void LoadDialogueHistory()
        {
            // 1. 尝试从缓存获取
            if (LightweightDialogueCache.Instance.TryGetDialogueHistory(_username, _npcName, out var history))
            {
                UpdateHistoryUI(history);
                return;
            }

            // 2. 缓存无数据，请求服务器
            var serverHistory = await _httpService.GetDialogueHistoryAsync(_username, _npcName);
            if (serverHistory != null)
            {
                // 存入缓存
                LightweightDialogueCache.Instance.SetDialogueHistory(_username, _npcName, serverHistory);
                // 更新UI
                UpdateHistoryUI(serverHistory);
            }
            else
            {
                Debug.LogWarning($"获取{_npcName}的历史对话失败，使用空记录");
                UpdateHistoryUI(new DialogueHistoryResponseDto());
            }
        }

        /// <summary>
        /// 更新历史会话UI
        /// </summary>
        private void UpdateHistoryUI(DialogueHistoryResponseDto history)
        {
            string historyText = "";
            foreach (var dialogue in history)
            {
                string rolePrefix = dialogue.Role == "user" ? "[玩家]：" : $"[{_npcName}]：";
                historyText += $"{rolePrefix}{dialogue.Content}\n\n";
            }
            _historyContentText.text = historyText;
        }
        #endregion

        #region 新对话处理
        private async void OnSendButtonClick()
        {
            string playerInput = _playerInputField.text.Trim();
            if (string.IsNullOrEmpty(playerInput) || _isWaitingForReply)
            {
                return;
            }

            // 1. 禁用输入和发送按钮，显示“思考中”
            SetInputInteractable(false);
            _npcReplyText.text = "思考中...";
            _isWaitingForReply = true;

            // 2. 构造新对话请求
            NewDialogueRequestDto request = new NewDialogueRequestDto
            {
                Username = _username,
                NpcName = _npcName,
                Content = playerInput
            };

            try
            {
                // 3. 发送请求（async/await处理网络）
                var npcReply = await _httpService.SendNewDialogueAsync(request);
                if (npcReply != null)
                {
                    // 4. 收到回复后，逐字显示
                    StartCoroutine(TypewriterCoroutine(npcReply.Content));

                    // 5. 构造玩家和NPC的对话DTO，存入缓存
                    DialogueResponseDto userDialogue = new DialogueResponseDto
                    {
                        Role = "user",
                        Content = playerInput
                    };
                    LightweightDialogueCache.Instance.AddDialogueToCache(_username, _npcName, userDialogue, npcReply);

                    // 6. 更新历史UI
                    LoadDialogueHistory(); // 重新加载缓存中的历史
                }
                else
                {
                    _npcReplyText.text = "对话失败，请重试";
                }
            }
            finally
            {
                // 7. 恢复输入状态（无论成功/失败）
                _playerInputField.text = "";
                _isWaitingForReply = false;
                SetInputInteractable(true);
            }
        }

        /// <summary>
        /// 逐字显示回复的协程
        /// </summary>
        private IEnumerator TypewriterCoroutine(string content)
        {
            _npcReplyText.text = "";
            foreach (char c in content)
            {
                _npcReplyText.text += c;
                yield return new WaitForSeconds(_typewriterSpeed);
            }
        }

        /// <summary>
        /// 设置输入框/按钮交互状态
        /// </summary>
        private void SetInputInteractable(bool interactable)
        {
            _playerInputField.interactable = interactable;
            _sendButton.interactable = interactable;
        }
        #endregion

        #region 历史面板控制
        private void ToggleHistoryPanel()
        {
            _historyPanel.SetActive(!_historyPanel.activeSelf);
        }
        #endregion
    }
}