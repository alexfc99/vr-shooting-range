using System;
using UnityEngine;
using UnityEngine.UIElements;

public class StartGameScript : MonoBehaviour
{
    public AudioClip clip;
    public event Action<int> OnScoreChanged;
    public AudioSource source;
    public GameObject uiDocument;
    [SerializeField] private GameObject StartSignal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartGame()
    {
        PlayMusic();
        StartSignal.SetActive(false);
        uiDocument.SetActive(true);
        OnScoreChanged?.Invoke(0);
    }
    public void PlayMusic()
    {
        if (clip == null) return;
        source.clip = clip;
        source.loop = true;
        source.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
