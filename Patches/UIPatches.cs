using HarmonyLib;
using Reptile;
using TMPro;
using UnityEngine;

namespace ReplaySystem.Patches
{
    public class UI : MonoBehaviour
    {
        public static UI Instance = null;
        public static TextMeshProUGUI m_label1 = null;
        private float m_notificationTimer = 5f;
        private bool m_active;



        private void Awake()
        {
            Instance = this;

            SetupLabel();
            SetupLabelPause();
        }

        public void ShowNotification(string textbef)
        {
            m_label1.text = textbef;
            m_notificationTimer = 5f;
            m_label1.gameObject.SetActive(true);
        }


        public void HideNotification()
        {
            m_label1.gameObject.SetActive(false);
        }

        private void SetupLabel()
        {
            m_label1 = new GameObject("EmotePrev").AddComponent<TextMeshProUGUI>();
            var uiManager = Core.Instance.UIManager;
            var gameplay = Traverse.Create(uiManager).Field<GameplayUI>("gameplay").Value;
            var rep = gameplay.contextLabel;
            m_label1.font = rep.font;
            //m_label2.alpha = 0.3f;
            //m_label1.alpha = 0.3f;
            m_label1.fontSize = 32;
            m_label1.fontMaterial = rep.fontMaterial;
            m_label1.alignment = TextAlignmentOptions.TopRight;
            var rect1 = m_label1.rectTransform;
            rect1.anchorMin = new Vector2(0.1f, 0.5f);
            //1f = hug right, 0f = hug bottom
            rect1.anchorMax = new Vector2(0.88f, 0.97f);
            rect1.pivot = new Vector2(0, 1);
            rect1.anchoredPosition = new Vector2(1f, 0.2f);
            m_label1.rectTransform.SetParent(gameplay.gameplayScreen.GetComponent<RectTransform>(), false);
            //BunchOfEmotesPlugin.m_label = m_label;
        }


        private void SetupLabelPause()
        {

        }
    }

}
