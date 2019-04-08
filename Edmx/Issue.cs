using System;
using System.Collections.Generic;

namespace PortEF6toCore.Edmx
{
    public partial class Issue
    {
        public Issue()
        {
            Assignments = new HashSet<Assignment>();
            Comments = new HashSet<Comment>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int RepoId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedByName { get; set; }

        public virtual User CreatedBy { get; set; }
        public virtual Repo Repo { get; set; }
        public virtual Bug Bug { get; set; }
        public virtual Enhancement Enhancement { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}