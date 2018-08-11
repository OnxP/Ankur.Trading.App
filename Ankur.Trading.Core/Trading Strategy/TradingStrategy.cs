using Ankur.Trading.Core.BackTest;
using Ankur.Trading.Core.Broker;
using Ankur.Trading.Core.Trades;
using Ankur.Trading.Core.Trading_Algorthm;
using Binance.API.Csharp.Client;
using Ankur.Trading.Core.Broker;
using System.Collections.Generic;

namespace Ankur.Trading.Core.Trading_Strategy
{
    //This class should process the results and make a trade according to the rules.
    //It has to keep track of open positions
    //Needs to get the latest price to determine if the results are accurate
    //- in a large system with lots of trading pairs there could be a delay from the processing the algorthm
    //  and actually making the trade, in addition the price can move enough during this processing time to
    //  invalidate the results of the algorthm.
    //at the moment this class will work with only a single trading pair.
    //base class for other trading stragies.
    public class TradingStrategy
    {
        private BackTestRequest _request;
        private Position CurrentPosition { get; set; }
        private Broker.Broker broker { get; set; }
        private IList<Position> positionHistory { get; set; }


        public TradingStrategy(BinanceClient binanceClient, BackTestRequest request)
        {
            broker = new Broker.Broker(binanceClient,request);
            _request = request;
            CurrentPosition = new Position();
        }

        public void Process(AlgorthmResults analysisResults)
        {
            switch(analysisResults.Action)
            {
                case TradeAction.Buy:
                    BuyAction(analysisResults);
                    break;
                case TradeAction.Sell:
                    SellAction(analysisResults);
                    break;
                default: //wait
                    WaitAction(analysisResults);
                    break;
            }
        }

        public void BuyAction(AlgorthmResults results)
        {
            if (CurrentPosition != null && CurrentPosition.Open) return;
            CurrentPosition.Add(broker.MakeTransaction(results.Action));
        }

        public void SellAction(AlgorthmResults results)
        {
            if(CurrentPosition is null || CurrentPosition.Open)
            {
                CurrentPosition.Add(broker.MakeTransaction(results.Action));
                if(!CurrentPosition.Open)
                {
                    positionHistory.Add(CurrentPosition);
                }
            }
        }

        public void WaitAction(AlgorthmResults results)
        {

        }
    }
}
