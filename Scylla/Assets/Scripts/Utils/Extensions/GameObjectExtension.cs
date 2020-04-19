namespace Scylla
{
    using UnityEngine;

    public static class GameObjectExtension
    {
        public static void ClearChildren(this GameObject gameObject)
        {
            foreach (Transform child in gameObject.transform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }
    }
}

