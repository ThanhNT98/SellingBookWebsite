using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebsiteBanSach.Models;

namespace WebsiteBanSach.Controllers
{
    public class UserHomeController : Controller
    {
        BookShopEntities db = new BookShopEntities();
        // GET: UserHome
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult LoadDanhMucSach()
        {
            var DanhMucSachList = db.Categories;
            return PartialView(DanhMucSachList);
        }

        public ActionResult LoadSachTheoDanhMuc(int? CageID)//dâu hỏi vì int 32 hoặc int 64
        {
            
            if (CageID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);//lỗi 400-k tìm thấy
            }
            var ListSach = db.Books.Where(s => s.CategoryId == CageID && s.isDelete == false);
            if (ListSach == null)
            {
                return HttpNotFound();
            }

            return View(ListSach);
        }

        public ActionResult XemChiTietSach(int? BookID)
        {
            if (BookID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);//lỗi 400-k tìm thấy
            }
            var Sach = db.Books.SingleOrDefault(s => s.BookID == BookID && s.isDelete == false);
            if (Sach == null)
            {
                return HttpNotFound();
            }

            return View(Sach);
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {
            string Username = f["txtUsername"].ToString();
            string Password = f["txtPassword"].ToString();
            Customer CurrentUser = db.Customers.SingleOrDefault(u => u.CusUserName == Username && u.CusPass == Password);
            if (CurrentUser != null)
            {
                Session["Current_User"] = CurrentUser;
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        public ActionResult DangXuat()
        {
            Session["Current_User"] = null;
            return RedirectToAction("Index");
        }
    }
}