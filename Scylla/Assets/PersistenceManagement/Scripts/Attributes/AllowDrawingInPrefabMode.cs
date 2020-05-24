namespace Scylla.PersistenceManagement
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	[AttributeUsage(AttributeTargets.Field)]
	internal class AllowDrawingInPrefabMode : PropertyAttribute
	{
		public readonly bool _allow;

		public AllowDrawingInPrefabMode(bool allow)
		{
			_allow = allow;
		}
	}
}
