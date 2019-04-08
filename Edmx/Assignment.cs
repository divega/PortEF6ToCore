using System;
using System.Collections.Generic;

namespace PortEF6toCore.Edmx
{
    public partial class Assignment
    {
        public int IssueId { get; set; }
        public string UserName { get; set; }

        public virtual Issue Issue { get; set; }
        public virtual User User { get; set; }
    }
}