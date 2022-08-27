using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Leaderboards
{
    public class ScoreRow : MonoBehaviour
    {
        public TMP_Text namePart, scorePart, nameShadow, scoreShadow;
        public RawImage flag;
        [SerializeField] private Image background;
        [SerializeField] private Color evenColor;

        public void Setup(string nam, string sco, string locale, bool even)
        {
            namePart.text = nameShadow.text = nam;
            scorePart.text = scoreShadow.text = sco;
            FlagManager.SetFlag(flag, locale);

            if (even)
            {
                background.color = evenColor;
            }
        }
    }
}
