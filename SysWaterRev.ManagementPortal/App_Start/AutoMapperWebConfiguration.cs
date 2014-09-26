using AutoMapper;

namespace SysWaterRev.ManagementPortal
{
    public static class AutoMapperWebConfiguration
    {
        public static void Configure()
        {
            ConfigureMaping();
            Mapper.AssertConfigurationIsValid();
        }

        private static void ConfigureMaping()
        {
            Mapper.AddProfile(new AutoMapperProfile());
        }
    }
}