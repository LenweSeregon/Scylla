namespace Scylla.PersistenceManagement
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class PersistenceObserver : MonoBehaviour
	{
		protected virtual void Start()
		{
			StorageManager.Instance.RegisterStorable(this);
		}

		protected virtual void OnDestroy()
		{
			if (StorageManager.Instance != null)
				StorageManager.Instance.UnregisterStorable(this);
		}

		public abstract void LoadRequest(Storage storage);
		public abstract void SaveRequest(Storage storage);
	}
}
