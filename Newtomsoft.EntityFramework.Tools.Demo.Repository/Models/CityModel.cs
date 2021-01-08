using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Newtomsoft.EntityFramework.Tools.Demo.Repository.Models
{
    [Table("City")]
    public class CityModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public DateTime CensusDate { get; set; }
        public int Census { get; set; }
        public int AverageAnnualIncome { get; set; }
        public float AverageHouseholdPersons { get; set; }

        public virtual CountryModel Country { get; set; }
    }
}
