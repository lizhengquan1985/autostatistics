using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStatistics
{
    public class SpotStatisticsBiz
    {
        public static void Statistics()
        {
            while (true)
            {
                var dt = DateTime.Now.ToString("yyyyMMddHH");
                var spotStatisticsItem = new CoinDao().GetSpotStatistics(dt);
                if (spotStatisticsItem != null)
                {
                    Thread.Sleep(1000 * 60);

                    Console.WriteLine("当前统计 ->  " + JsonConvert.SerializeObject(spotStatisticsItem));
                    continue;
                }

                // 统计 TLCount TLAmount TLNowAmount TotalSy TotalLoss AllEarning StatisticsTime
                var spotRecords = new CoinDao().ListAll();
                Console.WriteLine($"总记录数{spotRecords}");
                int tlCount = 0;
                decimal tlAmount = 0;
                decimal tlNowAmount = 0;
                decimal totalSy = 0;

                Dictionary<string, decimal> coinCount = new Dictionary<string, decimal>();

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
                    }
                }

                foreach (var coin in coinCount.Keys)
                {
                    // 获取当前价位

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
                    AllEarning = allEarning
                });
            }
        }
    }
}
