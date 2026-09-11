using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PiGame.UI
{
    [DisallowMultipleComponent]
    public sealed class MainMenuUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject connectionPanel;
        [SerializeField] private GameObject lobbyPanel;

        [Header("Connection menu")]
        [SerializeField] private Button hostButton;
        [SerializeField] private Button clientButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Text statusText;

        private Coroutine selectionRoutine;

        private void Awake()
        {
            quitButton.onClick.AddListener(QuitGame);
        }

        private void OnEnable()
        {
            ShowConnectionPanel();
            selectionRoutine = StartCoroutine(SelectHostNextFrame());
        }

        private void OnDisable()
        {
            if (selectionRoutine != null)
            {
                StopCoroutine(selectionRoutine);
                selectionRoutine = null;
            }
        }

        private void OnDestroy()
        {
            if (quitButton != null)
            {
                quitButton.onClick.RemoveListener(QuitGame);
            }
        }

        public void ShowConnectionPanel()
        {
            connectionPanel.SetActive(true);
            lobbyPanel.SetActive(false);
            SetStatus("ESCOLHA HOST OU CLIENTE");
        }

        public void SetStatus(string message)
        {
            statusText.text = message;
        }

        private IEnumerator SelectHostNextFrame()
        {
            yield return null;
            EventSystem.current?.SetSelectedGameObject(hostButton.gameObject);
            selectionRoutine = null;
        }

        private static void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
