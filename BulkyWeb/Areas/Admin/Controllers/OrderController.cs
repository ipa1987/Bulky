using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderViewModel orderViewModel = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };

            return View(orderViewModel);
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            switch (status)
            {
                case "pending":
                    objOrderHeaders = objOrderHeaders.Where(o => o.PaymentStatus == SD.PaymentStatusPending);
                    break;

                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(o => o.OrderStatus == SD.StatusInProcess);
                    break;

                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;

                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(o => o.OrderStatus == SD.StatusApproved);
                    break;

                default:
                    break;
            }

            return Json(new { data = objOrderHeaders });
        }

        #endregion API CALLS
    }
}