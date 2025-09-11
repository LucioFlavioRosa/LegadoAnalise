using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Model
{
    public class DashboardModel
    {
        public int QtdAssociados { get; set; }
        public int QtdMentores { get; set; }
        public int QtdGestores { get; set; }
        public int QtdAvaliadores { get; set; }
        public int QtdAvaliacoes { get; set; }
        public string Status { get; set; }
        public int QtdStatus { get; set; }
        public List<DashboardModel> ListAndamento { get; set; }
    }
}
