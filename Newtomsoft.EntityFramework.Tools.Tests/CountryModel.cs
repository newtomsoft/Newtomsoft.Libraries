using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newtomsoft.EntityFramework.Tools.Tests
{
    [Table("Country")]
    public class CountryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDemocracy { get; set; }
    }
}
