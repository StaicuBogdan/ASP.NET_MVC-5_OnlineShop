using Microsoft.AspNet.Identity;
using Proiect2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect2.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private int _perPage = 9;

        private List<Product> produsePtSortare;
        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include("Category").Include("User").Where(a => a.Cerere == true).OrderBy(a => a.Date);
            if (produsePtSortare == null)
            {
                produsePtSortare = products.ToList();
            }

            if (TempData.ContainsKey("Produse"))
            {
                produsePtSortare = TempData["Produse"] as List<Product>;
            }
            

            var search = "";

            if (Request.Params.Get("search") != null)
            {
                search = Request.Params.Get("search").Trim();
                List<int> productIds = db.Products.Where(pr => pr.Nume.Contains(search) || pr.Descriere.Contains(search)).Select(p => p.Id).ToList();

                List<int> reviewIds = db.Reviews.Where(rev => rev.Content.Contains(search)).Select(rev => rev.ProductId).ToList();
                List<int> mergedIds = productIds.Union(reviewIds).ToList();

                products = db.Products.Where(product => mergedIds.Contains(product.Id)).Include("Category").Include("User").Where(a => a.Cerere == true).OrderBy(p => p.Date);
                produsePtSortare = products.ToList();
            }

            foreach (var prod in produsePtSortare)
            {
                CalculeazaRating(prod);
            }

            var totalProducts = produsePtSortare.Count();

            var currentPage = Convert.ToInt32(Request.Params.Get("page"));

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }
            var paginatedProducts = produsePtSortare.Skip(offset).Take(this._perPage);

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            //ViewBag.perPage = this._perPage;
            ViewBag.total = totalProducts;
            ViewBag.lastPage = Math.Ceiling((float)totalProducts / (float)this._perPage);
            ViewBag.Products = paginatedProducts;

            ViewBag.SearchString = search;

            return View();
        }

        public ActionResult SortareProduse(int id)
        {
            switch (id)
            {
                case 1:
                    produsePtSortare = db.Products.Include("Category").Include("User").Where(a => a.Cerere == true).OrderBy(a => a.Pret).ToList();
                    break;
                case 2:
                    produsePtSortare = db.Products.Include("Category").Include("User").Where(a => a.Cerere == true).OrderBy(a => a.Pret).ToList();
                    produsePtSortare.Reverse();
                    break;
                case 3:
                    produsePtSortare = db.Products.Include("Category").Include("User").Where(a => a.Cerere == true).OrderBy(a => a.Rating).ToList();
                    break;
                case 4:
                    produsePtSortare = db.Products.Include("Category").Include("User").Where(a => a.Cerere == true).OrderBy(a => a.Rating).ToList();
                    produsePtSortare.Reverse();
                    break;
                case 5:
                    produsePtSortare = db.Products.Include("Category").Include("User").Where(a => a.Cerere == true).OrderBy(a => a.Date).ToList();
                    produsePtSortare.Reverse();
                    break;
                default:
                    break;
            }
            TempData["Produse"] = produsePtSortare;
            return Redirect("/Products/Index");
        }

        public ActionResult Show(int id)
        {
            Product product = db.Products.Find(id);
            CalculeazaRating(product);
            SetAccessRights();
            var reviews = db.Reviews.Where(a => a.ProductId == id);
            if (reviews.Count() != 0)
            {
                db.SaveChanges();
            }
            else
            {
                product.Rating = 0;
                db.SaveChanges();
            }
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            return View(product);

        }

        [HttpPost]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public ActionResult NewReview(Review rev)
        {
            rev.Date = DateTime.Now;
            rev.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Reviews.Add(rev);
                    db.SaveChanges();
                    return Redirect("/Products/Show/" + rev.ProductId);
                }
                else
                {
                    Product p = db.Products.Find(rev.ProductId);
                    SetAccessRights();
                    TempData["message"] = "Campul nu poate fi necompletat!";
                    return Redirect("/Products/Show/" + rev.ProductId);

                }
            }

            catch (Exception e)
            {
                Product p = db.Products.Find(rev.ProductId);
                SetAccessRights();
                return View(p);
            }

        }

        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult New()
        {
            Product product = new Product();

            // preluam lista de categorii din metoda GetAllCategories()
            product.Categ = GetAllCategories();

            product.UserId = User.Identity.GetUserId();

            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult New(Product product)
        {

            product.Date = DateTime.Now;
            product.UserId = User.Identity.GetUserId();
            if (User.IsInRole("Admin"))
            {
                product.Cerere = true;
            }
            try
            {
                if (ModelState.IsValid)
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    TempData["message"] = "Produsul a fost adaugat!";
                    return RedirectToAction("Index");
                }
                else
                {
                    product.Categ = GetAllCategories();
                    return View(product);
                }
            }
            catch (Exception e)
            {
                product.Categ = GetAllCategories();
                return View(product);
            }
        }

        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult Edit(int id)
        {

            Product product = db.Products.Find(id);
            product.Categ = GetAllCategories();
            if (product.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(product);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs care nu va apartine";
                return RedirectToAction("Index");
            }
        }


        [HttpPut]
        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult Edit(int id, Product requestProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Product product = db.Products.Find(id);

                    if (product.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(product))
                        {
                            product = requestProduct;
                            db.SaveChanges();
                            TempData["message"] = "Produsul a fost modificat!";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs care nu va apartine";
                        return RedirectToAction("Index");
                    }
                }
                else {
                    requestProduct.Categ = GetAllCategories();
                    return View(requestProduct);
                }
            }
            catch (Exception e)
            {
                requestProduct.Categ = GetAllCategories();
                return View(requestProduct);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult Delete(int id)
        {
            Product product = db.Products.Find(id);

            if (product.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost sters!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un produs care nu va apartine";
                return RedirectToAction("Index");
            }
        }

        private void SetAccessRights()
        {
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.UtilizatorCurent = User.Identity.GetUserId();
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();

            var categories = from cat in db.Categories
                             select cat;

            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            return selectList;
        }

        [NonAction]
        public void CalculeazaRating(Product prod)
        {
            float rating_val = 0;
            int nr_reviews = 0;
            var reviews = db.Reviews.Where(a => a.ProductId == prod.Id);
            foreach (var rev in reviews)
            {
                rating_val = rating_val + rev.Rating;
                nr_reviews++;
            }
            rating_val = rating_val / nr_reviews;

            prod.Rating = rating_val;
            
        }
    }
}