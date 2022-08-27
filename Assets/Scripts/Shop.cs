using System;
using AnttiStarterKit.Animations;
using AnttiStarterKit.Extensions;
using AnttiStarterKit.Managers;
using Equipment;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private SpeechBubble speechBubble;
    [SerializeField] private Character keeper;
    [SerializeField] private GameObject openMouth;
    [SerializeField] private GameObject mouth;

    private void Start()
    {
        Invoke(nameof(ShowSpeech), 1.5f);

        speechBubble.onVocal += () =>
        {
            AudioManager.Instance.PlayEffectFromCollection(11, keeper.transform.position, 1.5f);
            mouth.SetActive(false);
            openMouth.gameObject.SetActive(true);
            
            this.StartCoroutine(() =>
            {
                mouth.SetActive(true);
                openMouth.gameObject.SetActive(false);
            }, 0.1f);
        };
    }

    private void ShowSpeech()
    {
        speechBubble.Show(GetMessage());
    }

    private string GetMessage()
    {
        return new[]
        {
            "Care to give up some (coin) for fine (goods)?",
            "Would you like to (buy) something?",
            "What're ya (buyin)?",
            "What're ya (sellin')?",
            "Got some (coin), stranger?"
        }.Random();
    }
    
    private string GetNoFundsMessage()
    {
        return new[]
        {
            "Looks like you (don't) have enough (coin)!",
            "It appears you lack the (coin) for that!",
            "You seem to me low on (coin)!",
            "Your (purse) seems a bit light!"
        }.Random();
    }
    
    private string GetThanksMessage()
    {
        return new[]
        {
            "Thanks!",
            "Thank you!",
            "Thank you so much!",
            "Was a pleasure (dealing) with you!",
            "Thanks for doing (business)!"
        }.Random();
    }
    
    private string GetByeMessage()
    {
        return new[]
        {
            "Bye!",
            "Bye bye!",
            "Thanks for stopping by!"
        }.Random();
    }

    public void OnLeave()
    {
        Invoke(nameof(ShowLeaveMessage), 0.75f);
    }

    private void ShowLeaveMessage()
    {
        keeper.SkillEffect();
        speechBubble.Show(GetByeMessage());
    }

    public void IndicateLowGold()
    {
        speechBubble.Show(GetNoFundsMessage());
    }

    public void Thank()
    {
        keeper.SkillEffect();
        speechBubble.Show(GetThanksMessage());
    }
    
    private string GetSellPriceMessage(string amount)
    {
        return new[]
        {
            $"I'll give you ({amount}) coin for that!",
            $"That's worth ({amount}) coin!",
            $"I could give you ({amount}) coin for it!",
            $"If you want to sell it, I'll give you ({amount}) coin!"
        }.Random();
    }

    public void ShowPriceFor(Equip equip)
    {
        speechBubble.Show(GetSellPriceMessage(equip.GetPrice().AsScore()), true);
    }
}