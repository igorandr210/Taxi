namespace TaxiCoinCoreLibrary.RequestObjectPatterns
{
    public class DefaultControllerPattern : IControllerPattern
    {
        public ulong Gas { get; set; } = 2100000;
    }
}
