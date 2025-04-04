using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static MenuSettingsPanelScript;

public class BloomActivator : MonoBehaviour
{

    Bloom bloom;

    private void Awake()
    {
        Volume volume = GetComponent<Volume>();
        volume.profile.TryGet(out bloom);

        bloom.active = PlayerPrefs.GetInt(ISettings.Type.BLOOM.ToString(), 1) == 1;
    }

    public void EnableBloom(bool enable)
    {
        bloom.active = enable;
    }

    private void OnEnable()
    {
        GameManager.OnEnableBloom += EnableBloom;
    }

    private void OnDisable()
    {
        GameManager.OnEnableBloom -= EnableBloom;
        bloom = null;
    }

}
