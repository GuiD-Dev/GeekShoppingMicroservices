using System.ComponentModel.DataAnnotations.Schema;

namespace CartAPI.Models;

[Table("cart_detail")]
public class CartDetail : BaseEntity
{
    [ForeignKey("cart_header_id")]
    public CartHeader CartHeader { get; set; }

    [Column("product_id")]
    public long ProductId { get; set; }

    [Column("count")]
    public int Count { get; set; }
}