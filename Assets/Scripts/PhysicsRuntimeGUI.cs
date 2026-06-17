using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PhysicsRuntimeGUI : MonoBehaviour
{
    [Header("Gravity Sliders")]
    public Slider gravityXSlider;
    public Slider gravityYSlider;
    public Slider gravityZSlider;

    [Header("Material Sliders")]
    public Slider bouncinessSlider;
    public Slider frictionSlider;

    [Header("Value Display Text")]
    public TextMeshProUGUI gravityXText;
    public TextMeshProUGUI gravityYText;
    public TextMeshProUGUI gravityZText;
    public TextMeshProUGUI bouncinessText;
    public TextMeshProUGUI frictionText;

    [Header("Tag Targeting")]
    public string targetTag = "PhysicsAffected";

    [Header("Reset")]
    public Button resetButton;
    private Vector3 defaultGravity = new Vector3(0, -9.81f, 0);
    private float defaultBounciness = 0.5f;
    private float defaultFriction = 0.5f;

    private List<Collider> affectedColliders = new List<Collider>();

    void Start()
    {
        RefreshAffectedObjects();

        gravityXSlider.value = Physics.gravity.x;
        gravityYSlider.value = Physics.gravity.y;
        gravityZSlider.value = Physics.gravity.z;
        bouncinessSlider.value = defaultBounciness;
        frictionSlider.value = defaultFriction;

        gravityXSlider.onValueChanged.AddListener(OnGravityChanged);
        gravityYSlider.onValueChanged.AddListener(OnGravityChanged);
        gravityZSlider.onValueChanged.AddListener(OnGravityChanged);
        bouncinessSlider.onValueChanged.AddListener(OnBouncinessChanged);
        frictionSlider.onValueChanged.AddListener(OnFrictionChanged);

        if (resetButton != null)
            resetButton.onClick.AddListener(ResetToDefaults);

        UpdateAllText();
    }

    // Call this if objects with the tag are spawned/destroyed during play
    public void RefreshAffectedObjects()
    {
        affectedColliders.Clear();
        GameObject[] tagged = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject obj in tagged)
        {
            Collider col = obj.GetComponent<Collider>();
            if (col == null) continue;

            if (col.material == null || !col.material.name.EndsWith("(Instance)"))
            {
                PhysicsMaterial newMat = new PhysicsMaterial(col.sharedMaterial != null ? col.sharedMaterial.name : "RuntimeMat");
                col.material = newMat;
            }

            affectedColliders.Add(col);
        }
    }

    void OnGravityChanged(float _)
    {
        Physics.gravity = new Vector3(gravityXSlider.value, gravityYSlider.value, gravityZSlider.value);
        UpdateAllText();
    }

    void OnBouncinessChanged(float value)
    {
        foreach (Collider col in affectedColliders)
        {
            if (col != null)
                col.material.bounciness = value;
        }
        UpdateAllText();
    }

    void OnFrictionChanged(float value)
    {
        foreach (Collider col in affectedColliders)
        {
            if (col != null)
            {
                col.material.dynamicFriction = value;
                col.material.staticFriction = value;
            }
        }
        UpdateAllText();
    }

    void ResetToDefaults()
    {
        gravityXSlider.value = defaultGravity.x;
        gravityYSlider.value = defaultGravity.y;
        gravityZSlider.value = defaultGravity.z;
        bouncinessSlider.value = defaultBounciness;
        frictionSlider.value = defaultFriction;
    }

    void UpdateAllText()
    {
        gravityXText.text = $"X: {gravityXSlider.value:F2}";
        gravityYText.text = $"Y: {gravityYSlider.value:F2}";
        gravityZText.text = $"Z: {gravityZSlider.value:F2}";
        bouncinessText.text = $"Bounciness: {bouncinessSlider.value:F2}";
        frictionText.text = $"Friction: {frictionSlider.value:F2}";
    }
}