using UnityEngine;

public class GlobalConfig : MonoBehaviour
{
    public static GlobalConfig instance;

    [Header("·þÎñÆ÷URL")]
    public string baseApiUrl = "http://localhost:5000/api/v1/dialogues";
    [Header("Íæ¼ÒÅäÖÃ")]
    public string _playerUsername = "XiaoChen";



    void Awake()
    {
        if (instance == null)
        {
            instance = new GlobalConfig();
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }


}
