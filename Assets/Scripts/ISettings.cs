using UnityEngine;

public interface ISettings
{
    public enum Type
    {
        // actual settings
        MUSICVOLUME,
        SOUNDEFFECTVOLUME,

        // game settings
        STARTMONEY,
        STARTHEALTH,
        MAXHEALTH,
        DIFFICULTY,
        BREATHERBETWEENWAVES
    }

    public void ValueChanged(Type type, int value);
}
