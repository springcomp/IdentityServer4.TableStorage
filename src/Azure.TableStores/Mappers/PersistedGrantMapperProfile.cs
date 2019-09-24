// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Net;
using AutoMapper;
using Models = IdentityServer4.Models;

namespace SpringComp.IdentityServer.TableStorage.Mappers
{
    /// <summary>
    /// AutoMapper Config for PersistedGrant
    /// Between Model and Entity
    /// <seealso cref="https://github.com/AutoMapper/AutoMapper/wiki/Configuration">
    /// </seealso>
    /// </summary>
    public class PersistedGrantMapperProfile:Profile
    {
        /// <summary>
        /// <see cref="PersistedGrantMapperProfile">
        /// </see>
        /// </summary>
        public PersistedGrantMapperProfile()
        {
            // entity to model
            CreateMap<Entities.PersistedGrant, Models.PersistedGrant>(MemberList.Destination)
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => WebUtility.UrlDecode(src.PartitionKey)))
                .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.RowKey));

            // model to entity
            CreateMap<Models.PersistedGrant, Entities.PersistedGrant>(MemberList.Source)
                .ForMember(dest => dest.PartitionKey, opt => opt.MapFrom(src => WebUtility.UrlEncode(src.Key)))
                .ForMember(dest => dest.RowKey, opt => opt.MapFrom(src => src.SubjectId));
        }
    }
}
