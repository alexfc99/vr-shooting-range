using UnityEngine;

public class VRSniperScope : MonoBehaviour
{
    [Header("Scope Camera")]
    [SerializeField] private Camera scopeCamera;
    [SerializeField] private RenderTexture scopeTexture;

    [Header("VR References")]
    [SerializeField] private Transform playerHead;   // XR Camera
    [SerializeField] private Transform scopePoint;    // punto en la mira

    [Header("Settings")]
    [SerializeField] private float activationDot = 0.9f;   // un poco más permisivo
    [SerializeField] private float maxDistance = 0.25f;

    private void Start()
    {
        if (scopeCamera != null)
        {
            scopeCamera.targetTexture = scopeTexture;
            scopeCamera.enabled = false;
        }
    }

    private void Update()
    {
        if (scopeCamera == null || playerHead == null || scopePoint == null)
            return;

        // Dirección desde scope hacia la cabeza (IMPORTANTE consistencia)
        Vector3 toEye = (playerHead.position - scopePoint.position).normalized;

        float dot = Vector3.Dot(scopePoint.forward, toEye);
        float distance = Vector3.Distance(playerHead.position, scopePoint.position);

        bool inRange = distance <= maxDistance;
        bool aligned = dot >= activationDot;

        bool shouldScope = inRange && aligned;

    
        Debug.Log($"DOT: {dot:F2} | DIST: {distance:F2} | SCOPE: {shouldScope}");

        // IMPORTANTE: estado único (evita bugs de "se queda encendido")
        if (scopeCamera.enabled != shouldScope)
        {
            scopeCamera.enabled = shouldScope;
            scopeTexture.Release();
        }
    }
}