## 人话在前

现在我想用unity作为该服务器的前端，通过unity提供的http请求工具向服务器发送请求，收到并展示服务器的回复。
现在，请你完成以下内容的设计：

1. NPC对话DTO

    发送玩家和NPC对话请求到服务器
    接收服务器返回的数据

2. NPC对话界面（使用TMP_Pro）

    一个文本框显示NPC当前的回复
    一个输入区域接受玩家输入
    一个发送按钮
    一个显示历史会话按钮，点击后显示一个列表，显示玩家和当前NPC的对话记录

    行为：在收到回复之前，会禁用玩家输入框并且在NPC回复的文本框中显示“思考中”，直到收到回复
    收到回复后启动一个协程，用于逐个字显示回复

3. NPC行为控制

（目前不用写）

1. 实体接口（分成两个）

    移动接口
    交互接口

2. NPC实体
   
    分别实现移动接口和交互接口
    实现移动接口用于NPC移动
    实现交互接口用于玩家交互，当玩家交互时，显示玩家和NPC对话的界面

3. 轻量化缓存器
   
    提供缓存服务，在本次程序运行时，缓存玩家和NPC对话

注意：
- 当玩家首次和NPC对话时，发送获取历史上下文请求，得到返回值后，将其存入轻量化缓存器并加入UI的历史会话列表中，当玩家第二次和NPC交互时（本次程序运行时），直接从缓存器获取当前NPC的历史会话而不发送请求。
- 玩家产生新对话后，先不放入缓存器，等到收到服务器回复后再放入缓存，避免丢包产生数据不一致。

- 使用 C# 的 async/await 处理网络 I/O，使用 Unity 的 Coroutine 处理引擎相关异步


## 一、整体项目结构

```plaintext
Assets/
├── Scripts/
│   ├── DTOs/                  # 数据传输对象（与服务器交互的模型）
│   │   ├── DialogueRequestDtos.cs  # 请求DTO（历史/新对话/删除）
│   │   └── DialogueResponseDtos.cs # 响应DTO（单条/列表对话）
│   ├── Network/               # 网络请求工具
│   │   └── HttpDialogueService.cs  # 封装HTTP请求（async/await）
│   ├── Interfaces/            # 实体接口
│   │   ├── IMoveable.cs       # 移动接口
│   │   └── IInteractable.cs   # 交互接口
│   ├── NPC/                   # NPC实体
│   │   └── NPCEntity.cs       # 实现移动+交互接口，控制NPC行为
│   ├── UI/                    # 对话UI
│   │   └── DialogueUIManager.cs # 管理对话界面（TMP_Pro）
│   └── Utils/                 # 通用工具
│       └── DialogueCache.cs   # 内存级对话缓存
└── UI/                        # UI预制体
    └── DialogueUI.prefab      # 对话界面预制体（包含TMP输入框/文本/按钮）
```