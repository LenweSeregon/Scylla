namespace Scylla.ObjectManagement.GUID
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class GUIDManager
    {
        private struct GuidInfo
        {
            public GameObject go;
            public event Action<GameObject> OnAdd;
            public event Action OnRemove;

            public GuidInfo(GUIDEntity comp)
            {
                go = comp.gameObject;
                OnRemove = null;
                OnAdd = null;
            }

            public void HandleAddCallback()
            {
                if (OnAdd != null)
                {
                    OnAdd(go);
                }
            }

            public void HandleRemoveCallback()
            {
                if (OnRemove != null)
                {
                    OnRemove();
                }
            }
        }
        
        private static GUIDManager _instance;

        public static GUIDManager GetInstance()
        {
            if (_instance == null)
                _instance = new GUIDManager();
            
            return _instance;
        }
        
        private Dictionary<Guid, GuidInfo> _guidToObjectMap;
        
        private GUIDManager()
        {
            _guidToObjectMap = new Dictionary<Guid, GuidInfo>();
        }
        
        public static bool Add(GUIDEntity guidEntity)
        {
            return GetInstance().InternalAdd(guidEntity);
        }
        
        public static void Remove(System.Guid guid)
        {
            GetInstance().InternalRemove(guid);
        }
        
        public static GameObject ResolveGuid(Guid guid, Action<GameObject> onAddCallback, Action onRemoveCallback)
        {
            return GetInstance().ResolveGuidInternal(guid, onAddCallback, onRemoveCallback);
        }
        
        public static GameObject ResolveGuid(Guid guid, Action onDestroyCallback)
        {
            return GetInstance().ResolveGuidInternal(guid, null, onDestroyCallback);
        }
        
        public static GameObject ResolveGuid(Guid guid)
        {
            return GetInstance().ResolveGuidInternal(guid, null, null);
        }
        
        private bool InternalAdd(GUIDEntity guidEntity)
        {
            Guid guid = guidEntity.GetGuid();

            GuidInfo info = new GuidInfo(guidEntity);

            if (!_guidToObjectMap.ContainsKey(guid))
            {
                _guidToObjectMap.Add(guid, info);
                return true;
            }

            GuidInfo existingInfo = _guidToObjectMap[guid];
            if ( existingInfo.go != null && existingInfo.go != guidEntity.gameObject )
            {
                // normally, a duplicate GUID is a big problem, means you won't necessarily be referencing what you expect
                if (Application.isPlaying)
                {
                    Debug.AssertFormat(false, guidEntity, "Guid Collision Detected between {0} and {1}.\nAssigning new Guid. Consider tracking runtime instances using a direct reference or other method.", (_guidToObjectMap[guid].go != null ? _guidToObjectMap[guid].go.name : "NULL"), (guidEntity != null ? guidEntity.name : "NULL"));
                }
                else
                {
                    // however, at editor time, copying an object with a GUID will duplicate the GUID resulting in a collision and repair.
                    // we warn about this just for pedantry reasons, and so you can detect if you are unexpectedly copying these components
                    Debug.LogWarningFormat(guidEntity, "Guid Collision Detected while creating {0}.\nAssigning new Guid.", (guidEntity != null ? guidEntity.name : "NULL"));
                }
                return false;
            }

            // if we already tried to find this GUID, but haven't set the game object to anything specific, copy any OnAdd callbacks then call them
            existingInfo.go = info.go;
            existingInfo.HandleAddCallback();
            _guidToObjectMap[guid] = existingInfo;
            return true;
        }
        
        private void InternalRemove(Guid guid)
        {
            if (_guidToObjectMap.TryGetValue(guid, out var info))
            {
                info.HandleRemoveCallback();
            }

            _guidToObjectMap.Remove(guid);
        }
        
        // nice easy api to find a GUID, and if it works, register an on destroy callback
        // this should be used to register functions to cleanup any data you cache on finding
        // your target. Otherwise, you might keep components in memory by referencing them
        private GameObject ResolveGuidInternal(Guid guid, Action<GameObject> onAddCallback, Action onRemoveCallback)
        {
            GuidInfo info;
            if (_guidToObjectMap.TryGetValue(guid, out info))
            {
                if (onAddCallback != null)
                {
                    info.OnAdd += onAddCallback;
                }

                if (onRemoveCallback != null)
                {
                    info.OnRemove += onRemoveCallback;
                }
                _guidToObjectMap[guid] = info;
                return info.go;
            }

            if (onAddCallback != null)
            {
                info.OnAdd += onAddCallback;
            }

            if (onRemoveCallback != null)
            {
                info.OnRemove += onRemoveCallback;
            }

            _guidToObjectMap.Add(guid, info);
        
            return null;
        }
    }
}