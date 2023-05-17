using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectContracts.BindingModels
{
    public class ReviewBindingModel
    {
        public int? Id { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public int? CarId { get; set; }
    }
}
