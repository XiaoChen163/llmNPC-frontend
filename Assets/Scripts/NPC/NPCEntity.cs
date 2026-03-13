using UnityEngine;
using UnityNPCDialogue.Interfaces;
using UnityNPCDialogue.Network;
using UnityNPCDialogue.UI;

namespace UnityNPCDialogue.NPC
{
    [RequireComponent(typeof(HttpDialogueService))]
    public class NPCEntity : MonoBehaviour, IMoveable, IInteractable
    {
        [Header("NPC配置")]
        [SerializeField] private string _npcName = "星莹"; // NPC名称（与服务器一致）
        [SerializeField] private float _moveSpeed = 2f; // 移动速度

        [Header("UI引用")]
        [SerializeField] private DialogueUIManager _dialogueUI; // 对话UI预制体/实例

        [Header("玩家配置")]
        [SerializeField] private string _playerUsername = "Player1"; // 玩家用户名（可从玩家系统获取）

        private HttpDialogueService _httpService;

        private void Awake()
        {
            _httpService = GetComponent<HttpDialogueService>();
            // 初始化对话UI（隐藏状态）
            if (_dialogueUI != null)
            {
                _dialogueUI.Init(_playerUsername, _npcName, _httpService);
                _dialogueUI.HideDialogueUI();
            }
        }

        #region IMoveable 实现
        public void Move(Vector3 targetPos, float speed)
        {
            // 简单的移动逻辑（可扩展为导航网格/寻路）
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }

        // 测试用：随机移动
        public void RandomMove()
        {
            Vector3 randomPos = new Vector3(
                transform.position.x + Random.Range(-5f, 5f),
                transform.position.y,
                transform.position.z + Random.Range(-5f, 5f)
            );
            Move(randomPos, _moveSpeed);
        }
        #endregion

        #region IInteractable 实现
        public void Interact(Transform playerTransform)
        {
            if (_dialogueUI == null)
            {
                Debug.LogError("对话UI未赋值！");
                return;
            }

            // 显示对话UI，并定位到玩家附近
            _dialogueUI.ShowDialogueUI();
            _dialogueUI.transform.position = playerTransform.position + new Vector3(0, 2f, 0); // UI显示在玩家上方

            // 首次交互：从缓存/服务器获取历史对话
            _dialogueUI.LoadDialogueHistory();
        }
        #endregion

        // 外部调用：触发NPC移动（示例）
        public void TriggerMove()
        {
            RandomMove();
        }

        public string NpcName => _npcName;
    }
}