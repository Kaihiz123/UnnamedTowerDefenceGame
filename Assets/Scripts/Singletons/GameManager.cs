using UnityEngine;
using static MenuSettingsPanelScript;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerBaseHealthBar playerBaseHealthBar;

    public delegate void ShowEnemyHealthBar(bool show);
    public static event ShowEnemyHealthBar OnShowEnemyHealthBar;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ShowPlayerHealthBar(PlayerPrefs.GetInt(ISettings.Type.SHOWPLAYERHEALTHBAR.ToString()) == 1);
        ShowEnemyHealthBars(PlayerPrefs.GetInt(ISettings.Type.SHOWENEMYHEALTHBAR.ToString()) == 1);
    }

    public void ShowPlayerHealthBar(bool show)
    {
        playerBaseHealthBar.gameObject.SetActive(show);
    }

    public void ShowEnemyHealthBars(bool show)
    {
        OnShowEnemyHealthBar?.Invoke(show);
    }

    private void OnEnable()
    {
        MenuSettingsPanelScript.OnShowPlayerHealthBar += ShowPlayerHealthBar;
        MenuSettingsPanelScript.OnShowEnemyHealthBar += ShowEnemyHealthBars;
    }

    private void OnDisable()
    {
        MenuSettingsPanelScript.OnShowPlayerHealthBar -= ShowPlayerHealthBar;
        MenuSettingsPanelScript.OnShowEnemyHealthBar -= ShowEnemyHealthBars;
    }
}
