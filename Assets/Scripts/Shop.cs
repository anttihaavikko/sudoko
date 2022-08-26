using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private SpeechBubble speechBubble;
    [SerializeField] private Character keeper;

    private void Start()
    {
        Invoke(nameof(ShowSpeech), 1.5f);
    }

    private void ShowSpeech()
    {
        speechBubble.Show(GetMessage());
    }

    private string GetMessage()
    {
        return new[]
        {
            "Care to give up some (coin) for fine goods?",
            "Would you like to (buy) something?",
            "What're ya (buyin)?",
            "What're ya (sellin')?",
            "Got some (coin), stranger?"
        }.Random();
    }

    public void OnLeave()
    {
        Invoke(nameof(ShowLeaveMessage), 0.75f);
    }

    private void ShowLeaveMessage()
    {
        keeper.SkillEffect();
        speechBubble.Show("Bye!");
    }
}