using System;
using System.Collections.Generic;

namespace PortEF6toCore.Edmx
{
    public partial class Bug
    {
        public int Id { get; set; }
        public string ReproSteps { get; set; }

        public virtual Issue Issue { get; set; }
    }
}