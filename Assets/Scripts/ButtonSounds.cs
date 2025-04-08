using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioClip soundHover;
    [SerializeField] private AudioClip soundClick;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (soundHover != null)
            AudioManager.Instance.PlayUISoundEffect(soundHover);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (soundClick != null)
            AudioManager.Instance.PlayUISoundEffect(soundClick);
    }
}
