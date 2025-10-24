using System.ComponentModel.DataAnnotations.Schema;

namespace CartAPI.Models;

[Table("cart_detail")]
public class CartDetail : BaseEntity
{
    [ForeignKey("cart_header_id")]
    public CartHeader CartHeader { get; set; }

    [ForeignKey("product_id")]
    public Product Product { get; set; }

    [Column("count")]
    public int Count { get; set; }
}