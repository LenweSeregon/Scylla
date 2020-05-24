using System;
using Scylla.CommonModules.Identification;
using Scylla.ObjectManagement;
using UnityEditor;

namespace Scylla.PersistenceManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IStorable
    {
        string Save();
        void Load(string stringData);
    }
}