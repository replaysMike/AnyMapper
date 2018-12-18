using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnyMapper.Tests.EFCore
{
    public class DbObject
    {
        public DbObject()
        {
            ChildDbObjects = new HashSet<ChildDbObject>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreatedUtc { get; set; }
        public DateTime DateModifiedUtc { get; set; }

        public virtual ICollection<ChildDbObject> ChildDbObjects { get; set; }

        public override string ToString()
        {
            return $"Id: {Id} Name: {Name} Description: {Description}";
        }
    }
}
