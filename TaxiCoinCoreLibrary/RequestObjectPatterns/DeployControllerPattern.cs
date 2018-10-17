namespace TaxiCoinCoreLibrary.RequestObjectPatterns
{
    public class DeployControllerPattern : IControllerPattern
    {
        public string Address { get; set; }
        public ulong Gas { get; set; } 
    }
}
