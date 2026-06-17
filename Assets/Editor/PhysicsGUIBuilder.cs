using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class PhysicsGUIBuilder
{
    // [MenuItem("Tools/Build Physics Runtime GUI")]
    public static void BuildGUI()
    {
        // --- Canvas ---
        GameObject canvasGO = new GameObject("PhysicsCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        // Ensure an EventSystem exists in the scene
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // --- Panel ---
        GameObject panelGO = new GameObject("PhysicsPanel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        Image panelImage = panelGO.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.6f);
        RectTransform panelRT = panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0f, 1f);
        panelRT.anchorMax = new Vector2(0f, 1f);
        panelRT.pivot = new Vector2(0f, 1f);
        panelRT.anchoredPosition = new Vector2(20, -20);
        panelRT.sizeDelta = new Vector2(340, 420);

        VerticalLayoutGroup vlg = panelGO.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(15, 15, 15, 15);
        vlg.spacing = 8;
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;

        // --- Title ---
        CreateLabel(panelGO.transform, "Title", "Physics Settings", 20);

        // --- Rows ---
        Slider gravX = CreateSliderRow(panelGO.transform, "GravityX_Row", "Gravity X", -20f, 20f, out TextMeshProUGUI gravXText);
        Slider gravY = CreateSliderRow(panelGO.transform, "GravityY_Row", "Gravity Y", -20f, 20f, out TextMeshProUGUI gravYText);
        Slider gravZ = CreateSliderRow(panelGO.transform, "GravityZ_Row", "Gravity Z", -20f, 20f, out TextMeshProUGUI gravZText);
        Slider bounce = CreateSliderRow(panelGO.transform, "Bounciness_Row", "Bounciness", 0f, 1f, out TextMeshProUGUI bounceText);
        Slider friction = CreateSliderRow(panelGO.transform, "Friction_Row", "Friction", 0f, 1f, out TextMeshProUGUI frictionText);

        // --- Reset Button ---
        GameObject buttonGO = new GameObject("ResetButton", typeof(RectTransform));
        buttonGO.transform.SetParent(panelGO.transform, false);
        Image btnImage = buttonGO.AddComponent<Image>();
        btnImage.color = new Color(0.25f, 0.25f, 0.25f, 1f);
        Button resetButton = buttonGO.AddComponent<Button>();
        LayoutElement btnLayout = buttonGO.AddComponent<LayoutElement>();
        btnLayout.preferredHeight = 35;

        GameObject btnTextGO = new GameObject("Text", typeof(RectTransform));
        btnTextGO.transform.SetParent(buttonGO.transform, false);
        TextMeshProUGUI btnText = btnTextGO.AddComponent<TextMeshProUGUI>();
        btnText.text = "Reset";
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.fontSize = 16;
        StretchToParent(btnTextGO.GetComponent<RectTransform>());

        // --- Attach controller script ---
        PhysicsRuntimeGUI controller = canvasGO.AddComponent<PhysicsRuntimeGUI>();
        controller.gravityXSlider = gravX;
        controller.gravityYSlider = gravY;
        controller.gravityZSlider = gravZ;
        controller.bouncinessSlider = bounce;
        controller.frictionSlider = friction;
        controller.gravityXText = gravXText;
        controller.gravityYText = gravYText;
        controller.gravityZText = gravZText;
        controller.bouncinessText = bounceText;
        controller.frictionText = frictionText;
        controller.resetButton = resetButton;

        Selection.activeGameObject = canvasGO;
        Debug.Log("Physics Runtime GUI built successfully.");
    }

    private static TextMeshProUGUI CreateLabel(Transform parent, string name, string text, int fontSize)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Left;
        LayoutElement le = go.AddComponent<LayoutElement>();
        le.preferredHeight = fontSize + 10;
        return tmp;
    }

    private static Slider CreateSliderRow(Transform parent, string rowName, string labelText, float min, float max, out TextMeshProUGUI valueText)
    {
        GameObject rowGO = new GameObject(rowName, typeof(RectTransform));
        rowGO.transform.SetParent(parent, false);
        HorizontalLayoutGroup hlg = rowGO.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 8;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = false;
        LayoutElement rowLayout = rowGO.AddComponent<LayoutElement>();
        rowLayout.preferredHeight = 28;

        // Label
        GameObject labelGO = new GameObject("Label", typeof(RectTransform));
        labelGO.transform.SetParent(rowGO.transform, false);
        TextMeshProUGUI label = labelGO.AddComponent<TextMeshProUGUI>();
        label.text = labelText;
        label.fontSize = 14;
        label.alignment = TextAlignmentOptions.Left;
        LayoutElement labelLayout = labelGO.AddComponent<LayoutElement>();
        labelLayout.preferredWidth = 90;

        // Slider
        GameObject sliderGO = CreateSliderObject(rowGO.transform, min, max);
        Slider slider = sliderGO.GetComponent<Slider>();
        LayoutElement sliderLayout = sliderGO.AddComponent<LayoutElement>();
        sliderLayout.preferredWidth = 140;

        // Value text
        GameObject valueGO = new GameObject("ValueText", typeof(RectTransform));
        valueGO.transform.SetParent(rowGO.transform, false);
        valueText = valueGO.AddComponent<TextMeshProUGUI>();
        valueText.text = "0.00";
        valueText.fontSize = 14;
        valueText.alignment = TextAlignmentOptions.Right;
        LayoutElement valueLayout = valueGO.AddComponent<LayoutElement>();
        valueLayout.preferredWidth = 50;

        return slider;
    }

    private static GameObject CreateSliderObject(Transform parent, float min, float max)
    {
        GameObject sliderGO = new GameObject("Slider", typeof(RectTransform));
        sliderGO.transform.SetParent(parent, false);
        Slider slider = sliderGO.AddComponent<Slider>();

        // Background
        GameObject bgGO = new GameObject("Background", typeof(RectTransform));
        bgGO.transform.SetParent(sliderGO.transform, false);
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        StretchToParent(bgGO.GetComponent<RectTransform>());

        // Fill Area
        GameObject fillAreaGO = new GameObject("Fill Area", typeof(RectTransform));
        fillAreaGO.transform.SetParent(sliderGO.transform, false);
        RectTransform fillAreaRT = fillAreaGO.GetComponent<RectTransform>();
        fillAreaRT.anchorMin = new Vector2(0, 0.25f);
        fillAreaRT.anchorMax = new Vector2(1, 0.75f);
        fillAreaRT.offsetMin = new Vector2(5, 0);
        fillAreaRT.offsetMax = new Vector2(-5, 0);

        GameObject fillGO = new GameObject("Fill", typeof(RectTransform));
        fillGO.transform.SetParent(fillAreaGO.transform, false);
        Image fillImage = fillGO.AddComponent<Image>();
        fillImage.color = new Color(0.3f, 0.6f, 1f, 1f);
        RectTransform fillRT = fillGO.GetComponent<RectTransform>();
        fillRT.anchorMin = new Vector2(0, 0);
        fillRT.anchorMax = new Vector2(1, 1);
        fillRT.sizeDelta = Vector2.zero;

        // Handle Slide Area
        GameObject handleAreaGO = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleAreaGO.transform.SetParent(sliderGO.transform, false);
        RectTransform handleAreaRT = handleAreaGO.GetComponent<RectTransform>();
        handleAreaRT.anchorMin = Vector2.zero;
        handleAreaRT.anchorMax = Vector2.one;
        handleAreaRT.offsetMin = new Vector2(5, 0);
        handleAreaRT.offsetMax = new Vector2(-5, 0);

        GameObject handleGO = new GameObject("Handle", typeof(RectTransform));
        handleGO.transform.SetParent(handleAreaGO.transform, false);
        Image handleImage = handleGO.AddComponent<Image>();
        handleImage.color = Color.white;
        handleGO.GetComponent<RectTransform>().sizeDelta = new Vector2(14, 0);

        slider.fillRect = fillRT;
        slider.handleRect = handleGO.GetComponent<RectTransform>();
        slider.targetGraphic = handleImage;
        slider.minValue = min;
        slider.maxValue = max;
        slider.direction = Slider.Direction.LeftToRight;

        return sliderGO;
    }

    private static void StretchToParent(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}