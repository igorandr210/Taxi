namespace TaxiCoinCoreLibrary.RequestObjectPatterns
{
    public class DepositPattern : IControllerPattern
    {
        public ulong Value { get; set; }
        public ulong Gas { get; set; } = 2100000;
    }
}
