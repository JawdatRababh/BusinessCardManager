using Domin.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Domin.Models;

[Serializable]
public class BusinessCard : BaseModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public string Gender { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Phone]
    public string Phone { get; set; }

    [MaxLength(200)]
    public string Address { get; set; }

    // Prevent serialization of this collection
    [XmlIgnore]
    public ICollection<Attachment> Attachments { get; set; }

    public BusinessCard() { }
}
