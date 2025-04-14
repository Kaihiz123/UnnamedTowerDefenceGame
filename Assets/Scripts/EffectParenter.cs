using UnityEngine;

public class EffectParenter : MonoBehaviour
{
    private static GameObject _effectsParentInstance;
    
    public static GameObject EffectsParent
    {
        get
        {
            if (_effectsParentInstance == null)
            {
                _effectsParentInstance = GameObject.Find("EffectsParent");
            }
            return _effectsParentInstance;
        }
    }

    void Start()
    {
        if (EffectsParent != null)
        {
            transform.SetParent(EffectsParent.transform);
        }
    }
}