using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services
{
    public class DiscountService 
        (DiscountContext dbcontext, ILogger<DiscountService> logger)
        : DiscountProtoService.DiscountProtoServiceBase
    {
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await dbcontext.Coupons
                .FirstOrDefaultAsync(c => c.ProductName == request.ProductName);
            if (coupon is null)
                coupon = new Coupon{ProductName = "No Discount", Description = "No Discount", Amount = 0 };
            logger.LogInformation("Discount is retrieved for ProductName : {ProductName}, Amount : {Amount}", 
                coupon.ProductName, coupon.Amount);
            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;

        }
        public override async Task<GetAllDiscountsResponse> GetAllDiscounts(
            GetAllDiscountsRequest request, ServerCallContext context)
        {
            var entities = await dbcontext.Coupons
                .AsNoTracking()
                .ToListAsync(context.CancellationToken);

            var resp = new GetAllDiscountsResponse();
            resp.Coupons.AddRange(entities.Adapt<IEnumerable<CouponModel>>());
            logger.LogInformation("Returned {Count} discounts", resp.Coupons.Count);
            return resp;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = request.Coupon.Adapt<Coupon>();
            if (coupon is null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid coupon data"));

            dbcontext.Coupons.Add(coupon);
            await dbcontext.SaveChangesAsync();
            logger.LogInformation("Discount is successfully created for ProductName : {ProductName}, Amount : {Amount}", 
                coupon.ProductName, coupon.Amount);
            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }
        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = request.Coupon.Adapt<Coupon>();
            if (coupon is null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid coupon data"));

            dbcontext.Coupons.Update(coupon);
            await dbcontext.SaveChangesAsync();
            logger.LogInformation("Discount is successfully updated for ProductName : {ProductName}, Amount : {Amount}",
                coupon.ProductName, coupon.Amount);
            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }
        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var coupon = dbcontext.Coupons
                .FirstOrDefault(c => c.ProductName == request.ProductName);
            if (coupon is null)
                throw new RpcException(new Status(StatusCode.NotFound, "Coupon not found"));
            dbcontext.Coupons.Remove(coupon);
            dbcontext.SaveChanges();

            logger.LogInformation("Discount is successfully deleted for ProductName : {ProductName}", 
                coupon.ProductName);

            return new DeleteDiscountResponse { Success = true };
        }

        
    }
}