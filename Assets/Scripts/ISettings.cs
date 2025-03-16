using UnityEngine;

public interface ISettings
{
    public enum Type
    {
        MUSICVOLUME,
        SOUNDEFFECTVOLUME,

    }

    public void ValueChanged(Type type, int value);
}
