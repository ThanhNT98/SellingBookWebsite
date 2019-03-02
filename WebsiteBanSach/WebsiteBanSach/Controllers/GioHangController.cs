using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteBanSach.Models;

namespace WebsiteBanSach.Controllers
{
    public class GioHangController : Controller
    {
        BookShopEntities db = new BookShopEntities();
        //lấy giỏ hàng
        public List<ItemGioHang> LayGioHang()
        {
            List<ItemGioHang> listGioHang = Session["GioHang"] as List<ItemGioHang>;
            //nếu chưa có giỏ hàng >> tạo mới
            if (listGioHang == null)
            {
                listGioHang = new List<ItemGioHang>();
                Session["GioHang"] = listGioHang;
                
            }
            //có rồi thì lấy nó ra
            return listGioHang;
        }
        //thêm giỏ hàng

        public ActionResult ThemGioHang(int _BookID, string urlPath)
        {
            //kiểm tra sách có tồn tại trong CSDL không
            Book book = db.Books.Single(s => s.BookID == _BookID);
            //nếu sách k tồn tại >> đưa về trang 404
            if (book == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //nếu tồn tại >> lấy giỏ hàng ra
            List<ItemGioHang> listGioHang = LayGioHang();
            //1: sách đã có trong giỏ hàng
            ItemGioHang isExistedBook = listGioHang.SingleOrDefault(s => s.BookID == _BookID);
            if (isExistedBook != null)
            {
                //kiểm tra số lượng tồn <= số lượng trong giỏ hàng
                if (book.Quantity <= isExistedBook.Quantity)
                {
                    return View("ThongBao");
                }
                isExistedBook.Quantity++;
                isExistedBook.TotalPrice = isExistedBook.Quantity * isExistedBook.Price;
                return Redirect(urlPath);
            }
            //2: sách chưa có trong giỏ >> thêm sách mới với số lượng =1
            
            ItemGioHang itemGH = new ItemGioHang(_BookID);
            if (book.Quantity <= itemGH.Quantity)
            {
                return View("ThongBao");
            }
            listGioHang.Add(itemGH);
            return Redirect(urlPath);

        }

        public int TinhTongSoLuong()
        {
            //lấy giỏ hàng trên session
            List<ItemGioHang> gioHang = Session["GioHang"] as List<ItemGioHang>;
            if (gioHang == null)
            {
                return 0;
            }
            
            return gioHang.Sum(itemGH => itemGH.Quantity);
        }

        public decimal TinhTongTien()
        {
            //lấy giỏ hàng trên session
            List<ItemGioHang> gioHang = Session["GioHang"] as List<ItemGioHang>;
            if (gioHang == null)
            {
                return 0;
            }
            
            return gioHang.Sum(itemGH => itemGH.TotalPrice);
        }

        //để sau này dùng ajax dễ quản lý
        public ActionResult GioHangPartial()
        {
            //mang tổng số lượng và tổng tiền qua trang partial
            if (TinhTongSoLuong() == 0)//số lượng =0 >> tông tiền bằng 0 >> kiểm tra 1 cái là đủ
            {
                ViewBag.TongSoLuong = 0;
                ViewBag.TongTien = 0;
                return PartialView();
            }
            ViewBag.TongSoLuong = TinhTongSoLuong();
            ViewBag.TongTien = TinhTongTien();
            ViewBag.ExistGH = Session["GioHang"];
            return PartialView();
        }


        // GET: GioHang
        public ActionResult XemGioHang()
        {
            //lấy giỏ hàng
            List<ItemGioHang> GioHang = LayGioHang();
            //lấy tổng tiền
            ViewBag.TongTien = TinhTongTien();
            return View(GioHang);
        }

        public ActionResult EditGioHang(int _BookID)
        {
            //kiểm tra giỏ hàng đã tồn tại trên sesion chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index","UserHome");
            }
            //kiểm tra sách có tồn tại trong CSDL hay k
            Book sach = db.Books.SingleOrDefault(s => s.BookID == _BookID);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null; 
            }
            //lấy giỏ hàng từ session
            List<ItemGioHang> listGH = LayGioHang();
            //kiểm tra sản phẩm đã có trong giỏ hàng chưa
            ItemGioHang ExistedBook = listGH.SingleOrDefault(s => s.BookID == _BookID);
            if (ExistedBook == null)
            {
                return RedirectToAction("Index", "UserHome");
            }
            //lấy giỏ hàng để thiết kế giao diện
            ViewBag.GioHang = listGH;
            //lấy tổng tiền
            ViewBag.TongTien = TinhTongTien();
            return View(ExistedBook);
        }

        //xử lý button Cập Nhật
        [HttpPost]
        public ActionResult CapNhatGioHang(ItemGioHang itemGH)
        {
            //kiểm tra số lượng tồn
            Book bookCheck = db.Books.Single(s => s.BookID == itemGH.BookID);
            if (bookCheck.Quantity < itemGH.Quantity)
            {
                return View("ThongBao");
            }

            //cập nhật số lượng trong sesion giỏ hàng
            //B1:Lấy List<GioHang> tu session["GioHang"]
            List<ItemGioHang> listGH = LayGioHang();
            //b2: Lấy sản phẩm cần cập nhật từ trong list giỏ hàng ra
            ItemGioHang itemGHUpdate = listGH.Find(s => s.BookID == itemGH.BookID);
            //b3:cập nhật lại số lượng và thành tiền
            itemGHUpdate.Quantity = itemGH.Quantity;
            itemGHUpdate.TotalPrice = itemGHUpdate.Quantity * itemGHUpdate.Price;
          
            return RedirectToAction("XemGioHang");
        }

        //xóa giỏ hàng
        public ActionResult XoaItemGioHang(int _BookID) 
        {
            //kiểm tra giỏ hàng đã tồn tại trên sesion chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "UserHome");
            }
            //kiểm tra sách có tồn tại trong CSDL hay k
            Book sach = db.Books.SingleOrDefault(s => s.BookID == _BookID);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //lấy giỏ hàng từ session
            List<ItemGioHang> listGH = LayGioHang();
            //kiểm tra sản phẩm đã có trong giỏ hàng chưa
            ItemGioHang ExistedBook = listGH.SingleOrDefault(s => s.BookID == _BookID);
            if (ExistedBook == null)
            {
                return RedirectToAction("Index", "UserHome");
            }
            listGH.Remove(ExistedBook);
            return RedirectToAction("XemGioHang");
        }

        //chức năng đặt hàng
        public ActionResult DatHang()
        {
            //kiểm tra giỏ hàng đã tồn tại trên sesion chưa
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "UserHome");
            }

            //thêm đơn đặt hàng
            DonDatHang ddh = new DonDatHang();
            ddh.OrderDate = DateTime.Now;
            ddh.isDelivered = false;
            ddh.isPaid = false;
            ddh.isCanceled = false;
            ddh.isDeleted = false;
            db.DonDatHangs.Add(ddh);
            db.SaveChanges();
            //chi tiết đơn đặt hàng(cart)
            List<ItemGioHang> listGH = LayGioHang();
            foreach (var item in listGH)
            {
                Cart ctdh = new Cart();
                ctdh.DDH_ID = ddh.DDH_ID;
                ctdh.BookID = item.BookID;
                ctdh.BookName = item.BookName;
                ctdh.Quantity = item.Quantity;
                ctdh.Price =double.Parse(item.Price.ToString());
                db.Carts.Add(ctdh);
            }
            db.SaveChanges();
            Session["GioHang"] = null;
            return RedirectToAction("XemGioHang"); 
        }
    }
}