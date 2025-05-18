using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class SlingShot : MonoBehaviour
{
    [SerializeField]
    float dragRadius;
    Vector2 mouseClickPos;
    bool isDragging = false;
    float launchIntensity;
    public Vector2 launchDirection { get; private set; }
    [SerializeField]
    GameObject projectilePrefab;
    Projectile projectile;
    Texture2D circleTexture;
    public bool shooting { get; private set; }
    public bool coolDownRecharge { get; private set; }
    [SerializeField]
    GameObject trajectoryPoint;
    int numberOfPoints = 10;
    List<GameObject> trajectoryPoints = new List<GameObject>();
    [SerializeField]
    GameObject coolDownBar;
    [SerializeField]
    float coolDown;
    float cronometro;

    void Start()
    {
        circleTexture = MakeCircleTexture(256);
        cronometro = coolDown;
    }

    void Update()
    {
        if (cronometro < coolDown)
        {
            cronometro += Time.deltaTime;
        }
        else
        {
            coolDownRecharge = false;
        }

        if (Input.GetMouseButtonDown(0) && cronometro >= coolDown)
        {
            mouseClickPos = Input.mousePosition;
            isDragging = true;
            shooting = true;
            coolDownRecharge = false;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            shooting = false;

            GameObject newProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = newProjectile.GetComponent<Projectile>();

            projectile.LaunchProjectile(launchDirection, launchIntensity);
            coolDownRecharge = true;
            cronometro = 0f;
        }

        if (isDragging)
        {
            Vector2 currentMousePos = Input.mousePosition;
            float distance = Vector2.Distance(mouseClickPos, currentMousePos);

            launchIntensity = Mathf.Clamp01(distance / dragRadius);
            launchDirection = (mouseClickPos - currentMousePos).normalized;

            if (distance <= dragRadius)
            {
                Debug.Log($"Mouse dentro do círculo: {currentMousePos} | Intensidade: {launchIntensity:F2} | Direção: {launchDirection}");
            }

            DrawTrajectory(launchDirection, launchIntensity);
        }
        else
        {
            ClearTrajectory();
        }
    }

    void DrawTrajectory(Vector2 direction, float intensity)
    {
        ClearTrajectory(); // Garante que a trajetória anterior seja apagada

        Vector2 startVelocity = 10f * intensity * direction;
        Vector2 startPosition = transform.position;
        float timeStep = 0.1f;

        for (int i = 0; i < numberOfPoints; i++)
        {
            float t = i * timeStep;
            Vector2 pointPosition = startPosition + startVelocity * t + 0.5f * Physics2D.gravity * t * t;

            GameObject point = Instantiate(trajectoryPoint, pointPosition, Quaternion.identity);
            trajectoryPoints.Add(point);
        }
    }

    void ClearTrajectory()
    {
        foreach (GameObject point in trajectoryPoints)
        {
            Destroy(point);
        }
        trajectoryPoints.Clear();
    }

    void OnGUI()
    {
        if (isDragging && circleTexture != null)
        {
            float guiY = Screen.height - mouseClickPos.y;
            Rect rect = new Rect(mouseClickPos.x - dragRadius, guiY - dragRadius, dragRadius * 2, dragRadius * 2);
            GUI.DrawTexture(rect, circleTexture);
        }
    }

    Texture2D MakeCircleTexture(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Bilinear;

        Color[] pixels = new Color[size * size];
        Vector2 mouseClickPos = new Vector2(size / 2f, size / 2f);
        float dragRadius = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = x + y * size;
                float dist = Vector2.Distance(new Vector2(x, y), mouseClickPos);
                pixels[index] = dist <= dragRadius ? new Color(1, 0, 0, 0.3f) : new Color(0, 0, 0, 0);
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}