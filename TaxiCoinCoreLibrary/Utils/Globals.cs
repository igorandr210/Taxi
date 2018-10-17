using TaxiCoinCoreLibrary.TokenAPI;

namespace TaxiCoinCoreLibrary.Utils
{
    public class Globals
    {
        public ContractFunctions ContractFunctions { get; set; }

        private static Globals Instance;

        private Globals()
        {
            
        }
        public static Globals GetInstance()
        {
            if (Instance == null)
                Instance = new Globals();
            return Instance;
        }
    }
}
