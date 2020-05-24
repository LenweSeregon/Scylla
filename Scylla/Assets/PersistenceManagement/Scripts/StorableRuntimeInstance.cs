namespace Scylla.PersistenceManagement
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class StorableRuntimeInstance : MonoBehaviour
	{
		private StorageScene _sceneContainer;
		private Storable _storable;
		
		public void Init(StorageScene sceneContainer, Storable storable)
		{
			_sceneContainer = sceneContainer;
			_storable = storable;
		}
		
		private void OnDestroy()
		{
			if (StorageManager.Instance.DestroyedExplicitly(this.gameObject))
			{
				StorageManager.Instance.WipeStorable(_storable);
				_sceneContainer.DestroyRuntimeInstance(this, _storable);
			}
		}
	}
}
