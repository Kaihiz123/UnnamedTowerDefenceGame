using UnityEngine;
using UnityEngine.UI;

public class CooldownTimerScript : MonoBehaviour
{
    float cooldownTime;
    float startTime;
    bool initialized = false;
    Image image;

    public void Init(float cooldownTime)
    {
        this.cooldownTime = cooldownTime;

        image = GetComponent<Image>();

        startTime = Time.time;
        initialized = true;
    }

    private void Update()
    {
        if (initialized)
        {
            float progress = (Time.time - startTime) / cooldownTime;
            
            if(progress >= 1f)
            {
                Destroy(gameObject);
            }
            else
            {
                image.fillAmount = progress;
            }
        }
    }
}
