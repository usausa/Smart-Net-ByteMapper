﻿namespace Smart.IO.Mapper
{
    using System.Collections.Generic;

    using Smart.ComponentModel;

    public interface IByteMapperConfig
    {
        IComponentContainer ResolveComponents();

        IDictionary<string, object> ResolveParameters();

        IDictionary<MapKey, MapEntry> ResolveEntries();
    }
}