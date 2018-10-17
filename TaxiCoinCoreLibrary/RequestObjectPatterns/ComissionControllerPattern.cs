namespace TaxiCoinCoreLibrary.RequestObjectPatterns
{
    public class ComissionControllerPattern : IControllerPattern
    {
        public ulong Comission { get; set; }
        public ulong Gas { get; set; } = 2100000;
    }
}
