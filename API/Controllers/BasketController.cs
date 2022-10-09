using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Route("api/[controller]")]
    public class BasketController : BaseApiController
    {
        private readonly StoreContext _context;
        public BasketController(StoreContext context)
        {
            _context = context;
        }
        [HttpGet(Name = "GetBasket")]
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            var basket = await RetrieveBasket(GetbuyerId());
            if (basket == null) return NotFound();

            return basket.MapBasketToDto();
        }



        private async Task<Basket> RetrieveBasket(string buyerId)
        {
            if (string.IsNullOrEmpty(buyerId))
            {
                Response.Cookies.Delete("buyerId");
                return null;
            }
            var basket = await _context.Baskets
            .Include(i => i.Items)
            .ThenInclude(p => p.Product)
            .FirstOrDefaultAsync(x => x.BuyerId == buyerId);
            return basket;
        }

        private Basket CreateBasket()
        {
            var buyerId = User.Identity?.Name;
            if (string.IsNullOrEmpty(buyerId))
            {
                buyerId = Guid.NewGuid().ToString();
                var cookieOptions = new CookieOptions
                {
                    IsEssential = true,
                    Expires =
                DateTime.Now.AddDays(30)
                };
                Response.Cookies.Append("buyerId", buyerId, cookieOptions);
            }
            var basket = new Basket { BuyerId = buyerId };
            _context.Baskets.Add(basket);
            return basket;
        }

        [HttpPost]
        public async Task<ActionResult<BasketDto>> AddItemToBasket(int productId, int quantity)
        {
            //ขั้นตอนกํารเพิ่มสินค้ําเข้ําตะกร้ําfsdfsdf
            //get basket
            //get product
            //add item
            //save changes
            var basket = await RetrieveBasket(GetbuyerId());

            if (basket == null) basket = CreateBasket();

            var product = await _context.Products.FindAsync(productId);

            if (product == null) return BadRequest(new ProblemDetails { Title = "Product Not found" });


            basket.AddItem(product, quantity);

            var result = await _context.SaveChangesAsync() > 0;
            //Redirect to Route
            if (result) return CreatedAtRoute("GetBasket", basket.MapBasketToDto());
            return BadRequest(new ProblemDetails { Title = "Problem saving item to basket" });
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItem(int productId, int quantity)
        {
            // นำตะกร้ามา
            var basket = await RetrieveBasket(GetbuyerId());
            // เช็คว่ามีตะกร้ามั้ย ถ้าไม่มีรีเทิน NotFound
            if (basket == null) return NotFound();
            // ถ้ามีก็ให้รีมูฟ
            basket.RemoveItem(productId, quantity);
            // เซฟค่า
            var result = await _context.SaveChangesAsync() > 0;
            if (result) return Ok();
            return BadRequest(new ProblemDetails
            {
                Title = "Problem removing item from the basket"
            });
        }

        private string GetbuyerId()
        {
            // ?? ถ้าเป็น Null ให้ทำRequest.Cookies["buyerId"];
            return User.Identity.Name ?? Request.Cookies["buyerId"];
        }

    }
}