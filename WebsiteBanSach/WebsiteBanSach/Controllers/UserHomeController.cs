using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;
using WebsiteBanSach.Models;

namespace WebsiteBanSach.Controllers
{
    public class UserHomeController : Controller
    {
        BookShopEntities db = new BookShopEntities();
        // GET: UserHome
        public ActionResult Index()
        {
            int CageID = 1;
            var ListSach = db.Books.Where(s => s.CategoryId == CageID && s.isDelete == false);
            if (ListSach == null)
            {
                return HttpNotFound();
            }

            return View(ListSach);
            
        }
        //menu: danh mục >> tác giả>>sách của tác giả
        public ActionResult LoadDanhMucSach(int? CageID)
        {
            var SachList = db.Books;
            

            return PartialView(SachList);
        }

        

        public ActionResult LoadSachTheoDanhMuc(int? CageID, int? Page)//dâu hỏi vì int 32 hoặc int 64; page: trang hiên tại
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

            

            //phân trang bằng thư viện pagedlist
            int pageSize = 4;
            int pageNumber = (Page ?? 1);//phép gán: nếu Page k chưa giá trị thì mặc định bằng 1
            ViewBag.CageID = CageID;
            return View(ListSach.OrderBy(s=>s.CategoryId).ToPagedList(pageNumber,pageSize));
        }
        public ActionResult LoadSachTheoDanhMucVaTacGia(int? CageID, int? AuthorID)//dâu hỏi vì int 32 hoặc int 64
        {

            if (CageID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);//lỗi 400-k tìm thấy
            }
            var ListSach = db.Books.Where(s => s.CategoryId == CageID && s.AuthorID==AuthorID && s.isDelete == false);
            if (ListSach == null)
            {
                return HttpNotFound();
            }
            Author ThongTinTacGia = db.Authors.SingleOrDefault(tg => tg.AuthorID == AuthorID);
            ViewBag.ThongTinTacGia = ThongTinTacGia;

            
            return View(ListSach);
        }


        public ActionResult XemChiTietSach(int? BookID, int? AuthorID, string AuthorName)
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
            if (AuthorName != null)
            {
                ViewBag.AuthorName = AuthorName;
            }
            else
            {
                Author TacGia = db.Authors.SingleOrDefault(tg => tg.AuthorID == Sach.AuthorID);
                ViewBag.AuthorName = TacGia.AuthorName;
            }
            
            ////mang tên tác giả qau trang chi tiết
            //var TenTacGia = db.Authors.SingleOrDefault(tg=>tg.AuthorID==AuthorID);
            //ViewBag.TenTacGia = TenTacGia;
            return View(Sach);
        }

     
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