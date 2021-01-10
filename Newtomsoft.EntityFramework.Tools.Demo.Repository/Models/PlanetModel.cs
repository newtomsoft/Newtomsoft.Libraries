using System.ComponentModel.DataAnnotations.Schema;

namespace Newtomsoft.EntityFramework.Tools.Demo.Repository.Models
{
    [Table("Planet")]
    public class PlanetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsLifeForm { get; set; }
    }
}
