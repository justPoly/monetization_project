using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Papae.UnitySDK.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        ///   Instantiates a new game object and parents it to this one.
        ///   Resets position, rotation and scale and inherits the layer.
        /// </summary>
        /// <param name="parent">Game object to add the child to.</param>
        /// <returns>New child.</returns>
        public static GameObject AddChild(this GameObject parent)
        {
            return parent.AddChild("New Game Object");
        }

        /// <summary>
        ///   Instantiates a new game object and parents it to this one.
        ///   Resets position, rotation and scale and inherits the layer.
        /// </summary>
        /// <param name="parent">Game object to add the child to.</param>
        /// <param name="name">Name of the child to add.</param>
        /// <returns>New child.</returns>
        public static GameObject AddChild(this GameObject parent, string name)
        {
            var go = AddChild(parent, (GameObject)null);
            go.name = name;
            return go;
        }

        /// <summary>
        ///   Instantiates a prefab and parents it to this one.
        ///   Resets position, rotation and scale and inherits the layer.
        /// </summary>
        /// <param name="parent">Game object to add the child to.</param>
        /// <param name="prefab">Prefab to instantiate.</param>
        /// <returns>New prefab instance.</returns>
        public static GameObject AddChild(this GameObject parent, GameObject prefab)
        {
            var go = prefab != null ? UnityEngine.Object.Instantiate(prefab) : new GameObject();
            if (go == null || parent == null)
            {
                return go;
            }

            var transform = go.transform;
            transform.SetParent(parent.transform);
            transform.LocalReset();
            go.layer = parent.layer;
            return go;
        }

        /// <summary>
        /// A shortcut for creating a new game object with a number of components and adding it as a child
        /// </summary>
        /// <param name="components">A list of component types</param>
        /// <returns>The new game object</returns>
        public static GameObject AddChild(this GameObject parent, params Type[] components)
        {
            return AddChild(parent, "Game Object", components);
        }

        /// <summary>
        /// A shortcut for creating a new game object with a number of components and adding it as a child
        /// </summary>
        /// <param name="name">The name of the new game object</param>
        /// <param name="components">A list of component types</param>
        /// <returns>The new game object</returns>
        public static GameObject AddChild(this GameObject parent, string name, params Type[] components)
        {
            var obj = new GameObject(name, components);
            if (parent != null)
            {
                if (obj.transform is RectTransform) obj.transform.SetParent(parent.transform, true);
                else obj.transform.parent = parent.transform;
            }
            return obj;
        }

        /// <summary>
        /// A shortcut for adding a given game object as a child
        /// </summary>
        /// <returns>This gameobject</returns>
        public static GameObject AddChild(this GameObject parent, GameObject child, bool worldPositionStays = false)
        {
            child.transform.SetParent(parent.transform, worldPositionStays);
            return parent;
        }

        /// <summary>
        /// A shortcut for creating a new game object then adding a component then adding it to a parent object
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <returns>The new component</returns>
        public static T AddComponent<T>(this GameObject parent) where T : Component
        {
            return AddComponent<T>(parent, typeof(T).Name);
        }

        /// <summary>
        /// A shortcut for creating a new game object then adding a component then adding it to a parent object
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <param name="name">Name of the child game object</param>
        /// <returns>The new component</returns>
        public static T AddComponent<T>(this GameObject parent, string name) where T : Component
        {
            var obj = AddChild(parent, name, typeof(T));
            return obj.GetComponent<T>();
        }

        public static T AddInvisibleComponent<T>(this GameObject parent) where T : MonoBehaviour
        {
            T newComponent = parent.AddComponent<T>();
            newComponent.hideFlags = HideFlags.HideInInspector;

            return newComponent;
        }

        /// <summary>
        /// Assigns a layer to this GameObject and all its children recursively.
        /// </summary>
        /// <param name="gameObject">The GameObject to start at.</param>
        /// <param name="layer">The layer to set.</param>
        public static void AssignLayer(this GameObject gameObject, int layer, bool alsoToChildren = true)
        {
            gameObject.layer = layer;

            var transforms = gameObject.GetComponentsInChildren<Transform>();
            if (alsoToChildren)
            {
                for (var i = 0; i < transforms.Length; i++)
                {
                    transforms[i].gameObject.layer = layer;
                }
            }
        }

        /// <summary>
        /// Activates then immediately deactivates the target gameObject.
        /// Useful when wanting to call Awake before deactivating a gameObject.
        /// </summary>
        /// <param name="go"></param>
        public static void AwakeAndDeactivate(this GameObject go)
        {
            go.SetActive(true);
            go.SetActive(false);
        }


        /// <summary>
        ///   Destroys all children of a object.
        /// </summary>
        /// <param name="gameObject">Game object to destroy all children of.</param>
        public static void DestroyChildren(this GameObject gameObject)
        {
            foreach (var child in gameObject.GetChildren())
            {
                // Hide immediately.
                child.SetActive(false);

                if (UnityEngine.Application.isEditor && !UnityEngine.Application.isPlaying)
                {
                    UnityEngine.Object.DestroyImmediate(child);
                }
                else
                {
                    UnityEngine.Object.Destroy(child);
                }
            }
        }

        public static void DestroySafely(this GameObject gameObject)
        {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(gameObject);
#else
            GameObject.Destroy(gameObject);
#endif
        }

        /// <summary>
        /// Set enabled property MonoBehaviour of type T if it exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="enable"></param>
        /// <returns>True if the component exists</returns>
        public static bool EnableComponentIfExists<T>(this GameObject obj, bool enable = true) where T : MonoBehaviour
        {
            var t = obj.GetComponent<T>();

            if (t == null)
                return false;

            t.enabled = enable;

            return true;
        }

        /// <summary>
        /// Finds the first child, grandchild etc with the specified name
        /// </summary>
        /// <returns>The in all children.</returns>
        /// <param name="gameObj">Parent.</param>
        /// <param name="identifier">Part.</param>
        public static GameObject FindInAllChildren(this GameObject gameObj, string identifier)
        {
            foreach (GameObject go in gameObj.GetComponentsInChildren<GameObject>().Where(go => go.name == identifier))
            {
                return go;
            }

            Debug.LogWarning("Component " + identifier + " not found as child of " + gameObj.name);
            return null;
        }

        /// <summary>
        /// Finds the first child, grandchild etc with the specified tag
        /// </summary>
        /// <returns>The child by tag.</returns>
        /// <param name="gameObj">Parent.</param>
        /// <param name="tag">Tag.</param>
        public static GameObject FindChildByTag(this Transform gameObj, string tag)
        {
            foreach (GameObject go in gameObj.GetComponentsInChildren<GameObject>())
            {
                if (go != gameObj && go.tag == tag)
                {
                    return go;
                }
            }

            Debug.LogWarning("Tag " + tag + " not found in any children of " + gameObj.name);
            return null;
        }

        /// <summary>
        /// Finds all the children, grandchildren with the specified tags
        /// </summary>
        /// <returns>The children by tag.</returns>
        /// <param name="gameObj">Parent.</param>
        /// <param name="tag">Tag.</param>
        public static GameObject[] FindChildrenByTag(this GameObject gameObj, string tag)
        {
            return gameObj.GetComponentsInChildren<GameObject>().Where(go => go != gameObj && go.tag == tag).ToArray();
        }

        /// <summary>
        ///   Selects all ancestors (parent, grandparent, etc.) of a game object.
        /// </summary>
        /// <param name="gameObject">Game object to select the ancestors of.</param>
        /// <returns>All ancestors of the object.</returns>
        public static IEnumerable<GameObject> GetAncestors(this GameObject gameObject)
        {
            var parent = gameObject.transform.parent;

            while (parent != null)
            {
                yield return parent.gameObject;
                parent = parent.parent;
            }
        }

        /// <summary>
        ///   Selects all ancestors (parent, grandparent, etc.) of a game object,
        ///   and the game object itself.
        /// </summary>
        /// <param name="gameObject">Game object to select the ancestors of.</param>
        /// <returns>
        ///   All ancestors of the game object,
        ///   and the game object itself.
        /// </returns>
        public static IEnumerable<GameObject> GetAncestorsAndSelf(this GameObject gameObject)
        {
            yield return gameObject;

            foreach (var ancestor in gameObject.GetAncestors())
            {
                yield return ancestor;
            }
        }

        /// <summary>
        ///   Selects all children of a game object.
        /// </summary>
        /// <param name="gameObject">Game object to select the children of.</param>
        /// <returns>All children of the game object.</returns>
        public static IEnumerable<GameObject> GetChildren(this GameObject gameObject)
        {
            return (from Transform child in gameObject.transform select child.gameObject);
        }

        /// <summary>
        ///   Selects all descendants (children, grandchildren, etc.) of a game object.
        /// </summary>
        /// <param name="gameObject">Game object to select the descendants of.</param>
        /// <returns>All descendants of the game object.</returns>
        public static IEnumerable<GameObject> GetDescendants(this GameObject gameObject)
        {
            foreach (var child in gameObject.GetChildren())
            {
                yield return child;

                // Depth-first.
                foreach (var descendant in child.GetDescendants())
                {
                    yield return descendant;
                }
            }
        }

        /// <summary>
        ///   Selects all descendants (children, grandchildren, etc.) of a
        ///   game object, and the game object itself.
        /// </summary>
        /// <param name="gameObject">Game object to select the descendants of.</param>
        /// <returns>
        ///   All descendants of the game object,
        ///   and the game object itself.
        /// </returns>
        public static IEnumerable<GameObject> GetDescendantsAndSelf(this GameObject gameObject)
        {
            yield return gameObject;

            foreach (var descendant in gameObject.GetDescendants())
            {
                yield return descendant;
            }
        }

        /// <summary>
        ///   Returns the full path of a game object, i.e. the names of all
        ///   ancestors and the game object itself.
        /// </summary>
        /// <param name="gameObject">Game object to get the path of.</param>
        /// <returns>Full path of the game object.</returns>
        public static string GetHierarchyPath(this GameObject gameObject)
        {
            return
                gameObject.GetAncestorsAndSelf()
                    .Reverse()
                    .Aggregate(string.Empty, (path, go) => path + "/" + go.name)
                    .Substring(1);
        }

        /// <summary>
        /// When <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object)"/> is called on a prefab named
        /// "Original", the resulting instance will be named "Original(Clone)". This method returns the name
        /// without "(Clone)" by stripping everything after and including the first "(Clone)" it finds. If no
        /// "(Clone)" is found, the name is returned unchanged.
        /// </summary>
        /// <param name="gameObject">The GameObject to return the original name of.</param>
        public static string GetNameWithoutClone(this GameObject gameObject)
        {
            var gameObjectName = gameObject.name;

            var clonePartIndex = gameObjectName.IndexOf("(Clone)", StringComparison.Ordinal);
            if (clonePartIndex == -1)
                return gameObjectName;

            return gameObjectName.Substring(0, clonePartIndex);
        }

        /// <summary>
        ///   Gets the component of type <typeparamref name="T" /> if the game object has one attached,
        ///   and adds and returns a new one if it doesn't.
        /// </summary>
        /// <typeparam name="T">Type of the component to get or add.</typeparam>
        /// <param name="gameObject">Game object to get the component of.</param>
        /// <returns>
        ///   Component of type <typeparamref name="T" /> attached to the game object.
        /// </returns>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }

        public static T GetRequiredComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            Assert.IsNotNull(component);

            return component;
        }

        public static T GetRequiredComponent<T>(this Component component) where T : Component
        {
            return component.gameObject.GetRequiredComponent<T>();
        }

        public static GameObject GetRootParentOrSelf(this GameObject gameObject)
        {
            return gameObject.transform.GetParentsAndSelf().Select(x => x.gameObject).LastOrDefault();
        }

        /// <summary>
        ///   Indicates whether the a game object is an ancestor of another one.
        /// </summary>
        /// <param name="gameObject">Possible ancestor.</param>
        /// <param name="descendant">Possible descendant.</param>
        /// <returns>
        ///   <c>true</c>, if the game object is an ancestor of the other one, and
        ///   <c>false</c> otherwise.
        /// </returns>
        public static bool IsAncestorOf(this GameObject gameObject, GameObject descendant)
        {
            return gameObject.GetDescendants().Contains(descendant);
        }

        /// <summary>
        ///   Indicates whether the a game object is a descendant of another one.
        /// </summary>
        /// <param name="gameObject">Possible descendant.</param>
        /// <param name="ancestor">Possible ancestor.</param>
        /// <returns>
        ///   <c>true</c>, if the game object is a descendant of the other one, and
        ///   <c>false</c> otherwise.
        /// </returns>
        public static bool IsDescendantOf(this GameObject gameObject, GameObject ancestor)
        {
            return gameObject.GetAncestors().Contains(ancestor);
        }


        /// <summary>
        ///   Filters a sequence of game objects by layer.
        /// </summary>
        /// <param name="gameObjects">Game objects to filter.</param>
        /// <param name="layer">Layer to get the game objects of.</param>
        /// <returns>Game objects on the specified layer.</returns>
        public static IEnumerable<GameObject> OnLayer(this IEnumerable<GameObject> gameObjects, int layer)
        {
            return gameObjects.Where(gameObject => Equals(gameObject.layer, layer));
        }

        /// <summary>
        ///   Filters a sequence of game objects by layer.
        /// </summary>
        /// <param name="gameObjects">Game objects to filter.</param>
        /// <param name="layerName">Layer to get the game objects of.</param>
        /// <returns>Game objects on the specified layer.</returns>
        public static IEnumerable<GameObject> OnLayer(this IEnumerable<GameObject> gameObjects, string layerName)
        {
            var layer = LayerMask.NameToLayer(layerName);
            return gameObjects.Where(gameObject => Equals(gameObject.layer, layer));
        }

        /// <summary>
        /// Removed components of type T if it exists on the GameObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static void RemoveComponentsIfExists<T>(this GameObject obj) where T : Component
        {
            var t = obj.GetComponents<T>();

            for (var i = 0; i < t.Length; i++)
            {
                UnityEngine.Object.Destroy(t[i]);
            }
        }

        /// <summary>
        ///   Sets the layer of the game object.
        /// </summary>
        /// <param name="gameObject">Game object to set the layer of.</param>
        /// <param name="layerName">Name of the new layer.</param>
        public static void SetLayer(this GameObject gameObject, string layerName)
        {
            var layer = LayerMask.NameToLayer(layerName);
            gameObject.layer = layer;
        }

        /// <summary>
        ///   Sets the layers of all queried game objects.
        /// </summary>
        /// <param name="gameObjects">Game objects to set the layers of.</param>
        /// <param name="layerName">Name of the new layer.</param>
        /// <returns>Query for further execution.</returns>
        public static IEnumerable<GameObject> SetLayers(this IEnumerable<GameObject> gameObjects, string layerName)
        {
            var layer = LayerMask.NameToLayer(layerName);
            foreach (var o in gameObjects)
            {
                o.layer = layer;
            }
            return gameObjects;
        }

        /// <summary>
        ///   Sets the tags of all queried game objects.
        /// </summary>
        /// <param name="gameObjects">Game objects to set the tags of.</param>
        /// <param name="tag">Name of the new tag.</param>
        /// <returns>Query for further execution.</returns>
        public static IEnumerable<GameObject> SetTags(this IEnumerable<GameObject> gameObjects, string tag)
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.tag = tag;
            }
            return gameObjects;
        }

        /// <summary>
        ///   Tries to get the component on the same <see cref="GameObject"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the component to get.</typeparam>
        /// <param name="obj"><see cref="GameObject"/> to try to get the component of.</param>
        /// <param name="component">Found component, or <c>null</c> otherwise.</param>
        /// <returns><c>true</c> if a component was found, and <c>false</c> otherwise.</returns>
        public static bool TryGetComponent<T>(this GameObject obj, out T component)
        {
            component = obj.GetComponent<T>();
            return component != null;
        }

        /// <summary>
        ///   Filters a sequence of game objects by tag.
        /// </summary>
        /// <param name="gameObjects">Game objects to filter.</param>
        /// <param name="tag">Tag to get the game objects of.</param>
        /// <returns>Game objects with the specified tag.</returns>
        public static IEnumerable<GameObject> WithTag(this IEnumerable<GameObject> gameObjects, string tag)
        {
            return gameObjects.Where(gameObject => Equals(gameObject.tag, tag));
        }



        public static void CreateMeshColliders(this GameObject gameObject, bool actOnChildren)
        {
            // Create mesh colliders on a specific gameObject

            // First, remove existing colliders
            RemoveComponents<Collider>(gameObject);

            // Now, create mesh colliders if meshes exist
            var mesh = gameObject.GetComponent<MeshFilter>();
            if (mesh != null)
            {
                var meshCollider = gameObject.AddComponent<MeshCollider>();
                meshCollider.convex = false;
                meshCollider.isTrigger = false;
            }

            // Recurse into children if desired
            if (actOnChildren)
            {
                foreach (Transform child in gameObject.transform)
                {
                    child.gameObject.CreateMeshColliders(true);
                }
            }
        }

        public static void SetRenderersVisibility(this GameObject gameObject, bool visible)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = visible;
            }
        }

        public static void SetCollidersEnabled(this GameObject gameObject, bool visible)
        {
            var colliders = gameObject.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = visible;
            }
        }

        public static void RemoveComponents<T>(this GameObject gameObject) where T : Component
        {
            var components = gameObject.GetComponents<T>();
            foreach (Component component in components)
            {
                DestroySafely(component);
            }
        }

        //Renderer function
        public static void SetAlpha(GameObject gameObject, float alpha)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);
                }
            }
        }

        // Component function
        public static void DestroySafely(Component component)
        {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(component);
#else
            GameObject.Destroy(component);
#endif
        }
    }
}
