using Ankur.Trading.Core.BackTest;
using Ankur.Trading.Core.Trading_Algorthm;
using Binance.API.Csharp.Client;

namespace Ankur.Trading.Core.Trading_Strategy
{
    //This class should process the results and make a trade according to the rules.
    //It has to keep track of open positions
    //Needs to get the latest price to determine if the results are accurate
    //- in a large system with lots of trading pairs there could be a delay from the processing the algorthm
    //  and actually making the trade, in addition the price can move enough during this processing time to
    //  invalidate the results of the algorthm.
    public class TradingStrategy
    {
        private BinanceClient _binanceClient;
        private BackTestRequest _request;

        public TradingStrategy(BinanceClient binanceClient, BackTestRequest request)
        {
            _binanceClient = binanceClient;
            _request = request;
        }

        public void Process(AlgorthmResults analysisResults)
        {
            throw new System.NotImplementedException();
        }
    }
}
