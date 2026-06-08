using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Heat
{
    public class ChapterIdentifier : MonoBehaviour
    {
        [Header("Resources")]
        public Animator animator;
        [SerializeField] private RectTransform backgroundRect;
        public Image backgroundImage;
        public TextMeshProUGUI titleObject;
        public TextMeshProUGUI descriptionObject;
        public ButtonManager continueButton;
        public ButtonManager playButton;
        public ButtonManager replayButton;
        public GameObject completedIndicator;
        public GameObject unlockedIndicator;
        public GameObject lockedIndicator;

        [HideInInspector] public ChapterManager chapterManager;
        [HideInInspector] public bool isLocked;
        [HideInInspector] public bool isCurrent;

        public void UpdateBackgroundRect() 
        { 
            chapterManager.currentBackgroundRect = backgroundRect;
            chapterManager.DoStretch();
        }

        public void SetCurrent()
        {
           
        }

        public void SetLocked()
        {
            
        }

        public void SetUnlocked()
        {
           
        }

        public void SetCompleted()
        {
           
        }
    }
}