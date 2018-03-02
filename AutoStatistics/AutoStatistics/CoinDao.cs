using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoStatistics
{
    public class CoinDao
    {
        public CoinDao()
        {
            string connectionString = "server=localhost;port=3306;user id=root; password=lyx123456; database=studyplan; pooling=true; charset=utf8mb4";
            var connection = new MySqlConnection(connectionString);
            Database = new DapperConnection(connection);

        }
        protected IDapperConnection Database { get; private set; }

        public SpotStatistics GetSpotStatistics(string statisticsTime)
        {
            var sql = $"select * from t_spot_statistics where StatisticsTime='{statisticsTime}'";
            var r = Database.Query(sql).FirstOrDefault();
            return r;
        }

        public void CreateSpotStatistics(SpotStatistics spotStatistics)
        {
            using (var tx = Database.BeginTransaction())
            {
                Database.Insert(spotStatistics);
                tx.Commit();
            }
        }

        public List<SpotRecord> ListAll()
        {
            string sql = "select * from t_spot_record";
            var r = Database.Query<SpotRecord>(sql).ToList();
            return r;
        }
    }

    [Table("t_spot_statistics")]
    public class SpotStatistics
    {
        public long Id { get; set; }
        public decimal TLCount { get; set; }
        public decimal TLAmount { get; set; }
        public decimal TLNowAmount { get; set; }
        public decimal TotalSy { get; set; }
        public decimal TotalLoss { get; set; }
        public decimal AllEarning { get; set; }
        public string StatisticsTime { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class SpotRecord
    {
        public long Id { get; set; }
        public string Coin { get; set; }
        public decimal Selltotalquantity { get; set; }
        public decimal Selltradeprice { get; set; }
        public decimal Buytotalquantity { get; set; }
        public decimal Buytradeprice { get; set; }
        public bool SellSuccess { get; set; }

        //selltradeprice, selltotalquantity, buytradeprice,buytotalquantity, selltradeprice* selltotalquantity-buytradeprice* buytotalquantity
    }
}
