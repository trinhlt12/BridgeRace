using UnityEditor;
using UnityEngine;

public class LevelBuilderTool : EditorWindow
{
    private enum BlockType
    {
        None,
        Ground,
        Brick,
        Bridge,
        Base,
        Caro
    }

    private BlockType currentBlockType = BlockType.None;

    private GameObject groundBlockPrefab;
    private GameObject brickBlockPrefab;
    private GameObject bridgeBlockPrefab;
    private GameObject baseBlockPrefab;
    private GameObject caroBlockPrefab;

    private Vector3? lastPlacedPosition = null;

    [MenuItem("Tools/Level Builder")]
    public static void ShowWindow()
    {
        GetWindow<LevelBuilderTool>("Level Builder");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        GUILayout.Label("🛠️ Level Builder Tool", EditorStyles.boldLabel);

        currentBlockType = (BlockType)EditorGUILayout.EnumPopup("Block Type", currentBlockType);

        GUILayout.Space(10);
        GUILayout.Label("🔧 Block Prefabs", EditorStyles.boldLabel);
        groundBlockPrefab      = (GameObject)EditorGUILayout.ObjectField("Ground Prefab", groundBlockPrefab, typeof(GameObject), false);
        brickBlockPrefab       = (GameObject)EditorGUILayout.ObjectField("Brick Prefab", brickBlockPrefab, typeof(GameObject), false);
        this.bridgeBlockPrefab = (GameObject)EditorGUILayout.ObjectField("Bridge Prefab", this.bridgeBlockPrefab, typeof(GameObject), false);
        baseBlockPrefab        = (GameObject)EditorGUILayout.ObjectField("Base Prefab", baseBlockPrefab, typeof(GameObject), false);
        caroBlockPrefab        = (GameObject)EditorGUILayout.ObjectField("Caro Prefab", caroBlockPrefab, typeof(GameObject), false);
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (e.alt && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
        {
            TryEraseBlockAtMousePosition(e);
        }
        else if (e.shift && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0)
        {
            TryPlaceBlockAtMousePosition(e);
        }

        if (e.type == EventType.MouseUp && e.button == 0)
        {
            lastPlacedPosition = null;
        }
    }

    private void TryPlaceBlockAtMousePosition(Event e)
    {
        GameObject prefabToPlace = GetPrefabForCurrentBlockType();
        if (prefabToPlace == null) return;

        Ray     ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Vector3 gridPos;
        float   blockHalfHeight = this.GetBlockHalfHeight(prefabToPlace);
        Vector3 prefabExtents   = GetBlockExtents(prefabToPlace);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            gridPos = new Vector3(
                Mathf.Floor(hit.point.x) + 0.5f,
                hit.collider.bounds.max.y + blockHalfHeight,
                Mathf.Floor(hit.point.z) + 0.5f);
        }
        else
        {
            Vector3 point = ray.origin + ray.direction * 10f;
            gridPos = new Vector3(
                Mathf.Floor(point.x) + 0.5f,
                blockHalfHeight,
                Mathf.Floor(point.z) + 0.5f);
        }

        int safety = 100;
        while (Physics.CheckBox(gridPos, prefabExtents) && safety-- > 0)
        {
            gridPos.y += blockHalfHeight;
        }

        if (lastPlacedPosition == null || Vector3.Distance(gridPos, lastPlacedPosition.Value) > 0.01f)
        {
            GameObject newBlock = (GameObject)PrefabUtility.InstantiatePrefab(prefabToPlace);
            newBlock.transform.position = gridPos;
            Undo.RegisterCreatedObjectUndo(newBlock, "Place Block");

            ApplySettingsForBlockType(newBlock);

            lastPlacedPosition = gridPos;
            e.Use();
        }
    }

    private Vector3 GetBlockExtents(GameObject prefab)
    {
        Collider collider = prefab.GetComponentInChildren<Collider>();
        if (collider != null)
        {
            return collider.bounds.extents;
        }

        Renderer renderer = prefab.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.extents;
        }

        return Vector3.one * 0.5f;
    }

    private float GetBlockHalfHeight(GameObject prefab)
    {
        var collider = prefab.GetComponentInChildren<BoxCollider>();
        if (collider != null)
        {
            return (collider.size.y * prefab.transform.localScale.y) / 2f;
        }

        var renderer = prefab.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.y / 2f;
        }

        return 0.5f;
    }


    private GameObject GetPrefabForCurrentBlockType()
    {
        return currentBlockType switch
        {
            BlockType.Ground => groundBlockPrefab,
            BlockType.Brick  => brickBlockPrefab,
            BlockType.Bridge => this.bridgeBlockPrefab,
            BlockType.Base   => baseBlockPrefab,
            BlockType.Caro   => caroBlockPrefab,
            _                => null
        };
    }

    private void ApplySettingsForBlockType(GameObject block)
    {
        string parentName = currentBlockType.ToString(); // Ground, Brick, etc.
        string layerName = parentName;

        var root = GameObject.Find(parentName);
        if (root == null)
        {
            root = new GameObject(parentName);
            Undo.RegisterCreatedObjectUndo(root, "Create Root Group");
        }

        block.transform.SetParent(root.transform);

    }

    private static void TryEraseBlockAtMousePosition(Event e)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 point = hit.point;
            Vector3 gridPos = new Vector3(
                Mathf.Floor(point.x) + 0.5f,
                Mathf.Floor(point.y) + 0.5f,
                Mathf.Floor(point.z) + 0.5f);

            Collider[] colliders = Physics.OverlapBox(gridPos, Vector3.one * 0.45f);

            foreach (var col in colliders)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(col.gameObject))
                {
                    Undo.DestroyObjectImmediate(col.gameObject);
                    e.Use();
                    break;
                }
            }
        }
    }
}