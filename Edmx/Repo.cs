using System;
using System.Collections.Generic;

namespace PortEF6toCore.Edmx
{
    public partial class Repo
    {
        public Repo()
        {
            Issues = new HashSet<Issue>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedByName { get; set; }

        public virtual User CreatedBy { get; set; }
        public virtual ICollection<Issue> Issues { get; set; }
    }
}