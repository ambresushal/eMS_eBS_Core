using AutoMapper;
using System.Collections.Generic;

namespace tmg.equinox.integration.domain
{
    public static class Automapper
    {
        public static Target MapToEntity<Target, Source>(this Source sourceModel)
        {
            Mapper.CreateMap<Source, Target>();
            return Mapper.Map<Source, Target>(sourceModel);
        }

        public static List<Target> MapToListEntity<Target, Source>(this List<Source> sourceModel)
        {
            Mapper.CreateMap<Source, Target>();
            return Mapper.Map < List<Source>, List<Target>>(sourceModel);
        }
    }
}
