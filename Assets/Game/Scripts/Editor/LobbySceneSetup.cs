#if UNITY_EDITOR
using System;
using PiGame.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PiGame.Editor
{
    public static class LobbySceneSetup
    {
        private const string ScenePath = "Assets/Game/Scenes/Lobby/scene_lobby.unity";
        private const string FontPath = "Assets/ThirdParty/Fonts/PressStart2P/font_press_start_2p.ttf";

        private static readonly Color BackgroundColor = new(0.025f, 0.018f, 0.055f, 1f);
        private static readonly Color PanelColor = new(0.055f, 0.05f, 0.105f, 0.94f);
        private static readonly Color ButtonColor = new(0.09f, 0.08f, 0.16f, 0.25f);
        private static readonly Color AccentColor = new(0.32f, 0.86f, 0.78f, 1f);
        private static readonly Color ExitColor = new(0.72f, 0.25f, 0.31f, 1f);

        [MenuItem("Tools/PI Multiplayer/Aplicar Etapa 2 - Menu Inicial")]
        public static void Apply()
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Font font = AssetDatabase.LoadAssetAtPath<Font>(FontPath);
            if (font == null)
            {
                throw new InvalidOperationException($"Fonte nao encontrada em {FontPath}.");
            }

            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            RemoveSceneObject(scene, "Canvas");
            RemoveSceneObject(scene, "EventSystem");

            Canvas canvas = CreateCanvas();
            Stretch(CreateImage(canvas.transform, "Background", BackgroundColor).GetComponent<RectTransform>());

            GameObject connectionPanel = CreateFullScreenRoot(canvas.transform, "ConnectionPanel");
            GameObject contentPanel = CreatePanel(connectionPanel.transform, "ContentPanel", new Vector2(480f, 540f));

            Button hostButton = CreateButton(contentPanel.transform, "HostButton", "HOST", new Vector2(0f, 90f), AccentColor, font);
            Button clientButton = CreateButton(contentPanel.transform, "ClientButton", "CLIENT", new Vector2(0f, 10f), AccentColor, font);
            Button quitButton = CreateButton(contentPanel.transform, "QuitButton", "SAIR", new Vector2(0f, -70f), ExitColor, font);
            Text statusText = CreateText(contentPanel.transform, "StatusText", "ESCOLHA HOST OU CLIENTE", 16,
                new Vector2(0f, -165f), new Vector2(420f, 70f), font, Color.white);

            ConfigureNavigation(hostButton, clientButton, quitButton);

            GameObject lobbyPanel = CreateFullScreenRoot(canvas.transform, "LobbyPanel");
            lobbyPanel.SetActive(false);

            MainMenuUI menu = canvas.gameObject.AddComponent<MainMenuUI>();
            SetReference(menu, "connectionPanel", connectionPanel);
            SetReference(menu, "lobbyPanel", lobbyPanel);
            SetReference(menu, "hostButton", hostButton);
            SetReference(menu, "clientButton", clientButton);
            SetReference(menu, "quitButton", quitButton);
            SetReference(menu, "statusText", statusText);

            CreateEventSystem();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("[LobbySceneSetup] Etapa 2 aplicada: menu inicial e navegacao configurados.");
        }

        private static Canvas CreateCanvas()
        {
            GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        private static void CreateEventSystem()
        {
            _ = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private static GameObject CreateFullScreenRoot(Transform parent, string name)
        {
            GameObject root = new(name, typeof(RectTransform));
            root.transform.SetParent(parent, false);
            Stretch(root.GetComponent<RectTransform>());
            return root;
        }

        private static GameObject CreatePanel(Transform parent, string name, Vector2 size)
        {
            GameObject panel = CreateImage(parent, name, PanelColor);
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = size;
            return panel;
        }

        private static Button CreateButton(Transform parent, string name, string label, Vector2 position,
            Color highlightColor, Font font)
        {
            GameObject buttonObject = CreateImage(parent, name, ButtonColor);
            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(350f, 58f);

            Image image = buttonObject.GetComponent<Image>();
            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(highlightColor.r, highlightColor.g, highlightColor.b, 0.75f);
            colors.selectedColor = highlightColor;
            colors.pressedColor = new Color(highlightColor.r * 0.8f, highlightColor.g * 0.8f, highlightColor.b * 0.8f, 1f);
            colors.disabledColor = new Color(0.25f, 0.25f, 0.3f, 0.4f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;
            buttonObject.AddComponent<UIButtonPulse>();

            CreateText(buttonObject.transform, "Label", label, 25, Vector2.zero, rect.sizeDelta, font, Color.white);
            return button;
        }

        private static Text CreateText(Transform parent, string name, string content, int fontSize,
            Vector2 position, Vector2 size, Font font, Color color)
        {
            GameObject textObject = new(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Text text = textObject.GetComponent<Text>();
            text.text = content;
            text.font = font;
            text.fontSize = fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            return text;
        }

        private static GameObject CreateImage(Transform parent, string name, Color color)
        {
            GameObject imageObject = new(name, typeof(RectTransform), typeof(Image));
            imageObject.transform.SetParent(parent, false);
            imageObject.GetComponent<Image>().color = color;
            return imageObject;
        }

        private static void ConfigureNavigation(Button host, Button client, Button quit)
        {
            SetNavigation(host, quit, client);
            SetNavigation(client, host, quit);
            SetNavigation(quit, client, host);
        }

        private static void SetNavigation(Button button, Selectable up, Selectable down)
        {
            Navigation navigation = button.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnUp = up;
            navigation.selectOnDown = down;
            button.navigation = navigation;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void RemoveSceneObject(Scene scene, string objectName)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name == objectName)
                {
                    UnityEngine.Object.DestroyImmediate(root);
                }
            }
        }

        private static void SetReference(UnityEngine.Object target, string propertyName, UnityEngine.Object value)
        {
            SerializedObject serializedObject = new(target);
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (property == null)
            {
                throw new InvalidOperationException($"Campo {propertyName} nao encontrado em {target.GetType().Name}.");
            }

            property.objectReferenceValue = value;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
#endif
