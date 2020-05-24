namespace Scylla.PersistenceManagement.Example
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class PersistenceTester : MonoBehaviour
	{
		private void Start()
		{
			StartCoroutine(Testing());
		}

		private IEnumerator Testing()
		{
			yield return new WaitForSeconds(1f);
			StorageManager.Instance.SpawnRuntimeInstance(gameObject.scene, RuntimeSource.Resource, "Player");
		}
	}
}
