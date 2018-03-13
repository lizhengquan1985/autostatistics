using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoStatistics
{
    public class SpotStatisticsBiz
    {
        public static void Statistics()
        {
            while (true)
            {
                try
                {
                    DoStatistics();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message, ex);
                }
            }
        }

        public static void DoStatistics()
        {
            var dt = DateTime.Now.ToString("yyyy-MM-dd HH");
            var spotStatisticsItem = new CoinDao().GetSpotStatistics(dt);
            if (spotStatisticsItem != null)
            {
                Thread.Sleep(1000 * 60);

                Console.WriteLine("当前统计 ->  " + JsonConvert.SerializeObject(spotStatisticsItem));
                return;
            }

            // 统计 TLCount TLAmount TLNowAmount TotalSy TotalLoss AllEarning StatisticsTime
            var spotRecords = new CoinDao().ListAll();
            Console.WriteLine($"总记录数{spotRecords}");
            int tlCount = 0;
            decimal tlAmount = 0;
            decimal tlNowAmount = 0;
            decimal totalSy = 0;

            Dictionary<string, decimal> coinCount = new Dictionary<string, decimal>();
            Dictionary<string, decimal> coinOtherCount = new Dictionary<string, decimal>();

            foreach (var spotRecord in spotRecords)
            {
                if (!spotRecord.SellSuccess)
                {
                    tlCount++;
                    tlAmount += spotRecord.Buytotalquantity * spotRecord.Buytradeprice;
                    if (coinCount.ContainsKey(spotRecord.Coin))
                    {
                        coinCount[spotRecord.Coin] += spotRecord.Buytotalquantity;
                    }
                    else
                    {
                        coinCount.Add(spotRecord.Coin, spotRecord.Buytotalquantity);
                    }
                }
                else
                {
                    totalSy += spotRecord.Selltotalquantity * spotRecord.Selltradeprice -
                               spotRecord.Buytotalquantity * spotRecord.Buytradeprice;

                    if (coinOtherCount.ContainsKey(spotRecord.Coin))
                    {
                        coinOtherCount[spotRecord.Coin] += spotRecord.Buytotalquantity - spotRecord.Selltotalquantity;
                    }
                    else
                    {
                        coinOtherCount.Add(spotRecord.Coin, spotRecord.Buytotalquantity - spotRecord.Selltotalquantity);
                    }
                }
            }

            foreach (var coin in coinCount.Keys)
            {
                // 获取当前价位
                var res = new AnaylyzeApi().Merged(coin + "usdt");
                tlNowAmount += res.tick.close * coinCount[coin];
            }

            decimal otherSy = 0;
            foreach (var coin in coinOtherCount.Keys)
            {
                // 获取当前价位
                var res = new AnaylyzeApi().Merged(coin + "usdt");
                otherSy += res.tick.close * coinOtherCount[coin];
            }

            decimal totalLoss = tlAmount - tlNowAmount;
            decimal allEarning = totalSy - totalLoss;

            new CoinDao().CreateSpotStatistics(new SpotStatistics()
            {
                CreateTime = DateTime.Now,
                StatisticsTime = dt,

                TLCount = tlCount,
                TLAmount = tlAmount,
                TLNowAmount = tlNowAmount,
                TotalSy = totalSy,

                TotalLoss = totalLoss,
                AllEarning = allEarning,
                OtherSy = otherSy
            });
        }
    }
}
