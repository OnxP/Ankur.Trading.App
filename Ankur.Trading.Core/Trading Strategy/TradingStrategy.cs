using Ankur.Trading.Core.BackTest;
using Ankur.Trading.Core.Trading_Algorthm;
using Binance.API.Csharp.Client;

namespace Ankur.Trading.Core.Trading_Strategy
{
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
