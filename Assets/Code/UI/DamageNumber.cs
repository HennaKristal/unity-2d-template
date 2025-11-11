using UnityEngine;
using TMPro;
using System.Collections;

public class DamageNumber : MonoBehaviour
{
    [Header("Normal Hits")]
    [SerializeField] private float normalDuration = 1f;
    [SerializeField] private float normalFontSize = 0.2f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private float normalMoveDistance = 0.5f;

    [Header("Critical Hits")]
    [SerializeField] private float criticalDuration = 1.5f;
    [SerializeField] private float criticalFontSize = 0.3f;
    [SerializeField] private Color criticalColor = new Color(1f, 1f, 0f);
    [SerializeField] private float criticalMoveDistance = 0.5f;

    [Header("Weak Spot Hits")]
    [SerializeField] private float weakSpotDuration = 1.2f;
    [SerializeField] private float weakSpotFontSize = 0.3f;
    [SerializeField] private Color weakSpotColor = Color.red;
    [SerializeField] private float weakSpotMoveDistance = 0.5f;

    [Header("Weak Spot Critical Hits")]
    [SerializeField] private float weakSpotCriticalDuration = 1.8f;
    [SerializeField] private float weakSpotCriticalFontSize = 0.35f;
    [SerializeField] private Color weakSpotCriticalColor = Color.red;
    [SerializeField] private float weakSpotCriticalMoveDistance = 0.5f;

    private static Transform canvasParent;
    private TextMeshProUGUI textMesh;
    private Vector3 targetDirection;
    private float fadeDuration;
    private float moveDistance;


    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();

        canvasParent = GameObject.FindGameObjectWithTag("DamageNumberCanvas")?.transform;

        if (canvasParent == null)
        {
            Debug.LogWarning("DamageNumber Canvas is missing");
            this.enabled = false;
            return;
        }

        transform.SetParent(canvasParent, false);
    }


    public void Initialize(int damage, bool isCritical, bool isWeakSpot)
    {
        textMesh.text = damage.ToString();

        if (isCritical && isWeakSpot)
        {
            textMesh.color = weakSpotCriticalColor;
            textMesh.fontSize = weakSpotCriticalFontSize;
            fadeDuration = weakSpotCriticalDuration;
            moveDistance = weakSpotCriticalMoveDistance;
        }
        else if (isCritical)
        {
            textMesh.color = criticalColor;
            textMesh.fontSize = criticalFontSize;
            fadeDuration = criticalDuration;
            moveDistance = criticalMoveDistance;
        }
        else if (isWeakSpot)
        {
            textMesh.color = weakSpotColor;
            textMesh.fontSize = weakSpotFontSize;
            fadeDuration = weakSpotDuration;
            moveDistance = weakSpotMoveDistance;
        }
        else
        {
            textMesh.color = normalColor;
            textMesh.fontSize = normalFontSize;
            fadeDuration = normalDuration;
            moveDistance = normalMoveDistance;
        }

        StartCoroutine(FadeOutAndMove());
    }


    private IEnumerator FadeOutAndMove()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        // Choose a random direction within 360 degrees
        float angle = Random.Range(0f, 360f);
        targetDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * moveDistance;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / fadeDuration;

            // Text movement
            float easedProgress = Mathf.Sqrt(progress);
            transform.position = Vector3.Lerp(startPosition, startPosition + targetDirection, easedProgress);

            // Fading out
            float alpha = Mathf.Lerp(1f, 0f, progress);
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alpha);

            yield return null;
        }

        Destroy(gameObject);
    }
}
