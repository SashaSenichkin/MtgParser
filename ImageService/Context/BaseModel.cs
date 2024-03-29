﻿#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageService.Context;

/// <summary>
/// общие для сущностей поля
/// </summary>
public class BaseModel
{
    /// <summary>
    /// ну.. id как будто везде нужен, так что я вынес в базовый класс
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
}