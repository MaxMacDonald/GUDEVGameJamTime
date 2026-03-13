using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SpawnTimelineWindow : EditorWindow
{
    private EnemySpawner spawner;
    private Vector2 scrollPos;
    private float timelineWidth = 2000f;
    private float timelineHeight = 120f;
    private float headerHeight = 40f;
    private float labelWidth = 100f;
    private float padding = 20f;

    [MenuItem("Tools/Spawn Timeline")]
    public static void ShowWindow()
    {
        GetWindow<SpawnTimelineWindow>("Spawn Timeline");
    }

    void OnGUI()
    {
        // Find spawner if not assigned
        if (spawner == null)
            spawner = FindObjectOfType<EnemySpawner>();

        if (spawner == null)
        {
            EditorGUILayout.HelpBox("No EnemySpawner found in scene. Please open a scene with an EnemySpawner.", MessageType.Warning);
            return;
        }

        if (spawner.spawnEvents == null || spawner.spawnEvents.Count == 0)
        {
            EditorGUILayout.HelpBox("No spawn events defined in EnemySpawner.", MessageType.Info);
            return;
        }

        // Header
        EditorGUILayout.Space(10);
        GUILayout.Label("Spawn Timeline", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // Get time range
        float maxTime = 0f;
        foreach (var e in spawner.spawnEvents)
            maxTime = Mathf.Max(maxTime, e.time);
        maxTime += 5f; // padding at end

        // Draw timeline in scroll view
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // Reserve space for the timeline
        Rect timelineRect = GUILayoutUtility.GetRect(
            timelineWidth + labelWidth + padding * 2,
            timelineHeight + headerHeight + padding * 2
        );

        DrawTimeline(timelineRect, maxTime);

        EditorGUILayout.EndScrollView();

        // Legend
        EditorGUILayout.Space(10);
        GUILayout.Label("Legend", EditorStyles.boldLabel);
        DrawLegend();
    }

    void DrawTimeline(Rect rect, float maxTime)
    {
        // Background
        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

        float timelineStartX = rect.x + labelWidth + padding;
        float timelineEndX = rect.x + rect.width - padding;
        float timelineY = rect.y + headerHeight + padding;
        float usableWidth = timelineEndX - timelineStartX;

        // Draw time markers
        int markerCount = Mathf.FloorToInt(maxTime);
        for (int t = 0; t <= markerCount; t++)
        {
            float x = timelineStartX + (t / maxTime) * usableWidth;
            EditorGUI.DrawRect(new Rect(x, timelineY - 10, 1, timelineHeight + 10), new Color(0.4f, 0.4f, 0.4f));
            GUI.Label(new Rect(x - 10, rect.y + padding, 30, 20),
                t + "s",
                new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(0.6f, 0.6f, 0.6f) } }
            );
        }

        // Group events by time
        Dictionary<float, List<EnemySpawner.SpawnEvent>> groupedEvents = new Dictionary<float, List<EnemySpawner.SpawnEvent>>();
        foreach (var spawnEvent in spawner.spawnEvents)
        {
            if (!groupedEvents.ContainsKey(spawnEvent.time))
                groupedEvents[spawnEvent.time] = new List<EnemySpawner.SpawnEvent>();
            groupedEvents[spawnEvent.time].Add(spawnEvent);
        }

        // Draw grouped events
        foreach (var kvp in groupedEvents)
        {
            float time = kvp.Key;
            List<EnemySpawner.SpawnEvent> events = kvp.Value;
            float x = timelineStartX + (time / maxTime) * usableWidth;

            // Draw vertical line
            EditorGUI.DrawRect(new Rect(x - 1, timelineY - 5, 2, timelineHeight + 5), new Color(0.5f, 0.5f, 0.5f));

            // Draw all enemies from all events at this time, row by row
            float iconSize = 24f;
            float iconSpacing = 2f;
            int iconsPerRow = 5;
            int iconIndex = 0;

            foreach (var spawnEvent in events)
            {
                Color enemyColor = GetEnemyColor(spawnEvent.enemyPrefab);

                for (int i = 0; i < spawnEvent.count; i++)
                {
                    int col = iconIndex % iconsPerRow;
                    int row = iconIndex / iconsPerRow;

                    float iconX = x - (iconSize * Mathf.Min(iconsPerRow, spawnEvent.count) / 2f) + col * (iconSize + iconSpacing);
                    float iconY = timelineY + row * (iconSize + iconSpacing);

                    Rect iconRect = new Rect(iconX, iconY, iconSize, iconSize);

                    Texture2D preview = GetEnemyPreview(spawnEvent.enemyPrefab);
                    if (preview != null)
                    {
                        GUI.DrawTexture(iconRect, preview);
                    }
                    else
                    {
                        EditorGUI.DrawRect(iconRect, enemyColor);
                        GUI.Label(iconRect,
                            GetEnemyInitial(spawnEvent.enemyPrefab),
                            new GUIStyle(EditorStyles.boldLabel)
                            {
                                alignment = TextAnchor.MiddleCenter,
                                normal = { textColor = Color.white }
                            }
                        );
                    }

                    iconIndex++;
                }
            }

            // Time label
            GUI.Label(
                new Rect(x - 15, timelineY + timelineHeight - 15, 40, 20),
                time + "s",
                new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = new Color(0.8f, 0.8f, 0.8f) }
                }
            );
        }

        // Baseline
        EditorGUI.DrawRect(new Rect(timelineStartX, timelineY - 1, usableWidth, 2), new Color(0.5f, 0.5f, 0.5f));
    }

    void DrawLegend()
    {
        if (spawner.spawnEvents == null) return;

        // Collect unique enemy types
        Dictionary<string, (Color color, GameObject prefab)> uniqueEnemies = new Dictionary<string, (Color, GameObject)>();
        foreach (var e in spawner.spawnEvents)
        {
            if (e.enemyPrefab == null) continue;
            string name = e.enemyPrefab.name;
            if (!uniqueEnemies.ContainsKey(name))
                uniqueEnemies[name] = (GetEnemyColor(e.enemyPrefab), e.enemyPrefab);
        }

        EditorGUILayout.BeginHorizontal();
        foreach (var kvp in uniqueEnemies)
        {
            // Color swatch
            Rect swatchRect = GUILayoutUtility.GetRect(16, 16, GUILayout.Width(16), GUILayout.Height(16));
            EditorGUI.DrawRect(swatchRect, kvp.Value.color);
            GUILayout.Space(4);
            GUILayout.Label(kvp.Key, EditorStyles.label);
            GUILayout.Space(16);
        }
        EditorGUILayout.EndHorizontal();
    }

    Color GetEnemyColor(GameObject prefab)
    {
        if (prefab == null) return Color.grey;

        // Assign consistent colors based on enemy type name
        string name = prefab.name.ToLower();
        if (name.Contains("shooter")) return new Color(0.2f, 0.6f, 1f);
        if (name.Contains("seeker")) return new Color(1f, 0.4f, 0.2f);
        if (name.Contains("bomber")) return new Color(1f, 0.8f, 0.1f);
        if (name.Contains("hex")) return new Color(0.6f, 0.2f, 1f);

        // Generate a color from the hash of the name for unknown types
        Random.InitState(prefab.name.GetHashCode());
        return new Color(Random.value, Random.value, Random.value);
    }

    Texture2D GetEnemyPreview(GameObject prefab)
    {
        if (prefab == null) return null;
        return AssetPreview.GetAssetPreview(prefab);
    }

    string GetEnemyInitial(GameObject prefab)
    {
        if (prefab == null) return "?";
        return prefab.name.Substring(0, 1).ToUpper();
    }
}