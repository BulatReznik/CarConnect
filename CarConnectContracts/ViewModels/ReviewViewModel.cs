using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarConnectContracts.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int? CarId { get; set; }
    }
}
