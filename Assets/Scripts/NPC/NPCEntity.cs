using UnityEngine;

[RequireComponent(typeof(HttpDialogueService))]
public class NPCEntity : MonoBehaviour, IMoveable, IInteractable
{
    [Header("NPC配置")]
    [SerializeField] private string _npcName = "星莹"; // NPC名称（与服务器一致）
    [SerializeField] private float _moveSpeed = 2f;

    [Header("UI引用")]
    [SerializeField] private DialogueUIManager _dialogueUI; //对话UI预制体

    private HttpDialogueService _httpService;

    private void Awake()
    {
        _httpService = GetComponent<HttpDialogueService>();
        // 初始化对话UI（隐藏状态）
        if (_dialogueUI != null)
        {
            _dialogueUI.Init(GlobalConfig.instance._playerUsername, _npcName, _httpService);
            _dialogueUI.HideDialogueUI();
        }
    }

    #region IMoveable 实现
    public void Move(Transform target)
    {
        Debug.Log($"{_npcName}正在移动到目标位置...{target.position}");
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
}
