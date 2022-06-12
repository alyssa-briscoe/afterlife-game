using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    public TextAsset inkJSON;
    private Story story;

    public Text textPrefab;
    public Button buttonPrefab;

    public List<AudioClip> _audioClips;
    private Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        story = new Story(inkJSON.text);

        StartCoroutine(refreshUI());
        InitializeAudioClips();
        _audioSource = GetComponent<AudioSource>();
    }

    IEnumerator refreshUI()
    {

        eraseUI();

        Text storyText = Instantiate(textPrefab) as Text;
        storyText.text = loadStoryChunk();
        storyText.transform.SetParent(this.transform, false);

        foreach (Choice choice in story.currentChoices)
        {
            Button choiceButton = Instantiate(buttonPrefab) as Button;
            choiceButton.transform.SetParent(this.transform, false);

            Text choiceText = choiceButton.GetComponentInChildren<Text>();
            choiceText.text = choice.text;

            choiceButton.onClick.AddListener(delegate {
                chooseStoryChoice(choice);
                
            });
        } while (_audioSource.isPlaying)
            yield return null;
   
        foreach (var tag in story.currentTags)
        {
            if (tag.StartsWith("Clip."))
            {
                var clipName = tag.Substring("Clip.".Length, tag.Length - "Clip.".Length);
                PlayClip(clipName);
                
            } 
        }   
    }

    void eraseUI()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }

    void chooseStoryChoice(Choice choice)
    {
        story.ChooseChoiceIndex(choice.index);
        StartCoroutine(refreshUI());
        
    }

    // Update is called once per frame
    void Update()
    {
  
    }

    void PlayClip(string clipName)
    {
        if(_clips.TryGetValue(clipName.ToLower(), out var clip))
        {
            _audioSource.PlayOneShot(clip);
        }
    }

    void InitializeAudioClips()
    {
        foreach(var clip in _audioClips)
        {
            _clips.Add(clip.name.ToLower().Replace(" ", " "), clip);
        }
    }
    string loadStoryChunk()
    {
        string text = "";

        if (story.canContinue)
        {
            text = story.ContinueMaximally();
        } 

        return text;

        


    }
}