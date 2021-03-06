﻿namespace Scylla.PersistenceManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class StorageConstants
    {
        public static readonly string METADATA_VERSION = "Version";
        public static readonly string METADATA_CREATION = "DateCreation";
        public static readonly string METADATA_TIMESPAN = "TimePlayed";
        public static readonly string METADATA_LAST_UPDATE = "DateLastPlayed";

        public static readonly string STORAGE_SEPARATOR_GUID = "-";
        public static readonly string STORAGE_SEPARATOR_INFORMATION = "-";
        public static readonly string STORAGE_SCENE_PREFIX = "RuntimeScene";
    }
}