namespace _GAME.Scripts.FSM.Bridge
{
    using System;
    using System.Collections.Generic;
    using Unity.AI.Navigation;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.Splines;

    public class BridgeBuilder : MonoBehaviour
    {
        [Header(("Spline Settings"))] [SerializeField] private SplineContainer splineContainer;

        [SerializeField] private int splineIndex = 0;

        [Header("Step Settings")] [SerializeField] private GameObject bridgeStepPrefab;
        [SerializeField]                           private Material   defaultMaterial;
        [SerializeField]                           private float      maxDistanceBetweenSteps = 0.2f;
        private                                            float      stepLength;
        private                                            float      stepHeight;

        [Header("Bridge Properties")] [SerializeField] private float          bridgeWidth = 2f;
        [SerializeField]                               private Transform      bridgeTransform;
        [SerializeField]                               private NavMeshSurface navMesh;

        private List<GameObject> bridgeSteps = new List<GameObject>();

        private void Awake()
        {
            stepLength = bridgeStepPrefab.GetComponent<BoxCollider>().size.z;
            stepHeight = bridgeStepPrefab.GetComponent<BoxCollider>().size.y;
        }

        private void Start()
        {
            BuildBridge();
        }

        private void BuildBridge()
        {
            if (this.splineContainer == null || this.bridgeStepPrefab == null)
            {
                Debug.LogError("SplineContainer or BridgeStepPrefab is not assigned.");
                return;
            }

            ClearExistingBridge();

            var spline        = this.splineContainer.Splines[splineIndex];
            var splineLength  = spline.GetLength();
            var numberOfSteps = Mathf.CeilToInt(splineLength / this.maxDistanceBetweenSteps) + 1;

            for (var i = 0; i < numberOfSteps; i++)
            {
                var normalizedDistance = (float)i / (numberOfSteps - 1);

                SplineUtility.Evaluate(spline, normalizedDistance, out var position, out var tangent,
                    out var up);

                var step = CreateBridgeStep(position, tangent, up);
                bridgeSteps.Add(step);
            }

            if (Application.isPlaying && this.navMesh != null)
            {
                this.navMesh.BuildNavMesh();
            }
        }

        private GameObject CreateBridgeStep(float3 position, float3 tangent, float3 up)
        {
            var step = Instantiate(this.bridgeStepPrefab, transform);
            step.transform.SetParent(bridgeTransform);
            step.transform.localPosition = position;

            var right = Vector3.Cross(math.normalize(tangent), math.normalize(up));
            step.transform.rotation = Quaternion.LookRotation(tangent, up);

            step.transform.localScale = new Vector3(this.bridgeWidth, this.stepHeight, this.stepLength);

            var bridgeStepComponent = step.GetComponent<BridgeStep>();
            if (bridgeStepComponent == null)
            {
                bridgeStepComponent = step.AddComponent<BridgeStep>();
            }
            return step;
        }

        private void ClearExistingBridge()
        {
            foreach (var step in this.bridgeSteps)
            {
                if (step != null)
                {
                    #if UNITY_EDITOR
                    if (!Application.isPlaying)
                        DestroyImmediate(step);
                    else
                        Destroy(step);
                    #else
                        Destroy(step);
                    #endif
                }
            }

            bridgeSteps.Clear();
        }

        #if UNITY_EDITOR
        public void RebuildInEditor()
        {
            if (stepLength == 0f || stepHeight == 0f)
            {
                stepLength = bridgeStepPrefab.GetComponent<BoxCollider>().size.z;
                stepHeight = bridgeStepPrefab.GetComponent<BoxCollider>().size.y;
            }

            BuildBridge();

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
        #endif
    }
}