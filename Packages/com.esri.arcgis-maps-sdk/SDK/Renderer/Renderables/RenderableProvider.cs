using Esri.HPFramework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Esri.ArcGISMapsSDK.Renderer.Renderables
{
    internal class RenderableProvider : IRenderableProvider
    {
        private readonly Dictionary<GameObject, IRenderable> gameObjectToRenderableMap = new();
        private readonly Dictionary<uint, IRenderable> activeRenderables = new();
        private readonly List<IRenderable> freeRenderables = new();

        private bool areMeshCollidersEnabled = false;

        private readonly GameObject unused = null;
        private readonly GameObject parent;

        // Keep track of per-layer parents
        private readonly Dictionary<uint, GameObject> layerParents = new();

        public IReadOnlyDictionary<uint, IRenderable> Renderables => activeRenderables;

        public bool AreMeshCollidersEnabled
        {
            get => areMeshCollidersEnabled;
            set
            {
                if (areMeshCollidersEnabled != value)
                {
                    areMeshCollidersEnabled = value;
                    foreach (var activeRenderable in activeRenderables)
                    {
                        activeRenderable.Value.IsMeshColliderEnabled = value;
                    }
                }
            }
        }

        public IEnumerable<IRenderable> TerrainMaskingMeshes =>
            Renderables.Values.Where(sc => sc.IsVisible && sc.MaskTerrain);

        public RenderableProvider(int initSize, GameObject parent, bool areMeshCollidersEnabled)
        {
            this.parent = parent;
            this.areMeshCollidersEnabled = areMeshCollidersEnabled;

            // Pool container
            unused = new GameObject("UnusedPoolGOs")
            {
				//
                hideFlags = HideFlags.None // show in hierarchy
            };
            unused.transform.SetParent(parent.transform, false);

            for (var i = 0; i < initSize; i++)
            {
                var renderable = new Renderable(CreateGameObject(i));
                renderable.RenderableGameObject.transform.SetParent(unused.transform, false);
                freeRenderables.Add(renderable);
            }
        }

        public IRenderable CreateRenderable(uint id, uint layerId)
        {
            IRenderable renderable;

            if (freeRenderables.Count > 0)
            {
                renderable = freeRenderables[0];
                renderable.IsVisible = false;
                freeRenderables.RemoveAt(0);
            }
            else
            {
                renderable = new Renderable(
                    CreateGameObject(
                        activeRenderables.Count + freeRenderables.Count,
                        $"Ahmed's Layer {layerId}_Renderable_{id}"
                    )
                );
            }

            // Make sure this layer has a parent node in hierarchy
            if (!layerParents.TryGetValue(layerId, out var layerParent))
            {
                layerParent = new GameObject($"Ahmed's layer_{layerId}")
                {
                    hideFlags = HideFlags.None
                };
                layerParent.transform.SetParent(parent.transform, false);
                layerParents[layerId] = layerParent;
            }

            renderable.RenderableGameObject.transform.SetParent(layerParent.transform, false);
            renderable.IsMeshColliderEnabled = areMeshCollidersEnabled;

            // Assign a descriptive name
            renderable.Name = $"Ahmed's layer {layerId}_Renderable_{id}";
            renderable.LayerId = layerId;

            // ---El layers beta3t el map by default bteb'a wakhda layer id 0 , w ay haga tanya bteb'a akbar mel zer
			// fa estaghlet da w ay layer akbar mel 0 akeed msh el map  ---
            if (layerId > 0)
            {
                renderable.RenderableGameObject.layer = 3;
            }

            activeRenderables.Add(id, renderable);
            gameObjectToRenderableMap.Add(renderable.RenderableGameObject, renderable);

            return renderable;
        }

        public void DestroyRenderable(uint id)
        {
            var activeRenderable = activeRenderables[id];

            activeRenderable.RenderableGameObject.transform.SetParent(unused.transform, false);
            activeRenderable.IsVisible = false;
            activeRenderable.Mesh = null;

            gameObjectToRenderableMap.Remove(activeRenderable.RenderableGameObject);
            activeRenderables.Remove(id);
            freeRenderables.Add(activeRenderable);
        }

        public void Release()
        {
            foreach (var activeRenderable in activeRenderables)
            {
                activeRenderable.Value.Destroy();
            }

            foreach (var freeRenderable in freeRenderables)
            {
                freeRenderable.Destroy();
            }

            activeRenderables.Clear();
            freeRenderables.Clear();

            if (unused)
            {
                if (Application.isEditor)
                {
                    Object.DestroyImmediate(unused);
                }
                else
                {
                    Object.Destroy(unused);
                }
            }

            // Destroy per-layer parents
            foreach (var kv in layerParents)
            {
                if (kv.Value)
                {
                    if (Application.isEditor)
                        Object.DestroyImmediate(kv.Value);
                    else
                        Object.Destroy(kv.Value);
                }
            }

            layerParents.Clear();
        }

        private static GameObject CreateGameObject(int id, string customName = null)
        {
            string goName = string.IsNullOrEmpty(customName) ? $"ArcGISGameObject_{id}" : customName;

            var gameObject = new GameObject(goName)
            {
                hideFlags = HideFlags.None // show in hierarchy
            };

            gameObject.SetActive(false);

            var renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.shadowCastingMode = ShadowCastingMode.TwoSided;
            renderer.enabled = true;

            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<HPTransform>();
            gameObject.AddComponent<MeshCollider>();

            return gameObject;
        }

        public IRenderable GetRenderableFrom(GameObject gameObject)
        {
            gameObjectToRenderableMap.TryGetValue(gameObject, out var renderable);
            return renderable;
        }
    }
}
