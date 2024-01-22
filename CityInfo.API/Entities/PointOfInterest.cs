﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Entities;

public class PointOfInterest(string name)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = name;

    [MaxLength(200)]
    public string? Description { get; set; }

    [ForeignKey("CityId")]
    public City? City { get; set; }
    public int CityId { get; set; }
}
