using Microsoft.AspNet.Identity;
using Proiect2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect2.Controllers
{
    public class ReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reviews
        public ActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public ActionResult Delete(int id)
        {
            Review rev = db.Reviews.Find(id);
            if (rev.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Reviews.Remove(rev);
                db.SaveChanges();
                return Redirect("/Products/Show/" + rev.ProductId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un review care nu va apartine";
                return RedirectToAction("Index", "Articles");
            }
        }


        [Authorize(Roles = "User,Colaborator,Admin")]
        public ActionResult Edit(int id)
        {
            Review rev = db.Reviews.Find(id);
            if (rev.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                ViewBag.Review = rev;
                return View(rev);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Articles");
            }

        }

        [HttpPut]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public ActionResult Edit(int id, Review requestReview)
        {
            try
            {
                Review rev = db.Reviews.Find(id);

                if (rev.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    if (TryUpdateModel(rev))
                    {
                        rev.Content = requestReview.Content;
                        rev.Rating = requestReview.Rating;
                        db.SaveChanges();
                    }
                    return Redirect("/Products/Show/" + rev.ProductId);
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                    return RedirectToAction("Index", "Articles");
                }

            }
            catch (Exception e)
            {
                return View(requestReview);
            }

        }
    }
}