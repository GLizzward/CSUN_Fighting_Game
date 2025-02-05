using UnityEngine;
using System.Collections.Generic;

public class CharacterSoundFX : MonoBehaviour
{
    public AudioSource audioSource;

    [System.Serializable]
    public class SoundAction
    {
        public string actionName;   // e.g., "Punch", "Kick", "Jump"
        public AudioClip soundClip; // Sound associated with the action
    }

    public SoundAction[] soundActions; // List of actions and their sounds

    private Dictionary<string, AudioClip> actionSoundMap;

    void Start()
    {
        actionSoundMap = new Dictionary<string, AudioClip>();
        foreach (SoundAction soundAction in soundActions)
        {
            if (!actionSoundMap.ContainsKey(soundAction.actionName))
            {
                actionSoundMap[soundAction.actionName] = soundAction.soundClip;
            }
        }
    }

    public void PlaySound(string actionName)
    {
        if (audioSource != null && actionSoundMap.ContainsKey(actionName))
        {
            audioSource.PlayOneShot(actionSoundMap[actionName]);
        }
        else
        {
            Debug.LogWarning($"No sound mapped for action: {actionName}");
        }
    }
}