using UnityEngine;

public interface ISettings
{
    public enum Type
    {
        // actual settings
        MASTERVOLUME,
        MUSICVOLUME,
        SOUNDEFFECTVOLUME,
        RESOLUTION,
        FULLSCREEN,

        // game settings
        STARTMONEY,
        STARTHEALTH,
        MAXHEALTH,
        DIFFICULTY,
        BREATHERBETWEENWAVES
    }

    public void ValueChanged<T>(Type type, T newValue) where T : struct;
}
