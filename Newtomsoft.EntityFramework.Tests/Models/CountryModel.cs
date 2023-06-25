using System.ComponentModel.DataAnnotations.Schema;

namespace Newtomsoft.EntityFramework.Tests.Models;

[Table("Country")]
public class CountryModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsDemocracy { get; set; }

    public CountryModel(string name, bool isDemocracy)
    {
        Name = name;
        IsDemocracy = isDemocracy;
    }
}