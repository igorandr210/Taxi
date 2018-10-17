namespace TaxiCoinCoreLibrary.RequestObjectPatterns
{
    public class CreatePaymentPattern : IControllerPattern
    {
        public ulong Gas { get; set; } = 2100000;
        public ulong Value { get; set; }
    }
}
