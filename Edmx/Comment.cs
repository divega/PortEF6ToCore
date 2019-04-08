using System;
using System.Collections.Generic;

namespace PortEF6toCore.Edmx
{
    public partial class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int IssueId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedByName { get; set; }

        public virtual User CreatedBy { get; set; }
        public virtual Issue Issue { get; set; }
    }
}