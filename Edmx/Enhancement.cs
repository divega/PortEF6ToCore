using System;
using System.Collections.Generic;

namespace PortEF6toCore.Edmx
{
    public partial class Enhancement
    {
        public int Id { get; set; }
        public int Votes { get; set; }

        public virtual Issue Issue { get; set; }
    }
}