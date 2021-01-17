using Microsoft.AspNet.Identity;
using Proiect2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect2.Controllers
{
    [Authorize(Roles ="User,Colaborator,Admin")]
    public class QuantitiesController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Quantities
        public ActionResult Index()
        {
            var userCurent = User.Identity.GetUserId();
            Cart cos = db.Carts.Where(a => a.UserId == userCurent).First();
           
            var quantities = from q in db.Quantities where q.CartId == cos.CartId select q;
            ViewBag.Quantities = quantities;
            ViewBag.UserCurent = db.Users.Find(User.Identity.GetUserId());

            CalculeazaTotal();

            return View();
        }

        [HttpPost]
        public ActionResult New(Quantity q)
        {
            //iau id-ul userului si id-ul cosului corespunzator acelui user
            var userCurent = User.Identity.GetUserId();
            Cart cos = db.Carts.Where(a => a.UserId == userCurent).First();
            //setez parametrul CartId la obiectul q primit dupa apasarea butonului "Adauga in cos", ceilalti 2 parametri fiind primiti din form
            q.CartId = cos.CartId;

            //verific daca exista deja produsul in cosul userului curent
            if (cos.Quantities.Where(a => a.Id == q.Id).Count() != 0)
            {
                //daca exista, iau produsul din cos printr-un obiect quantity_temp, incrementez cantitatea
                Quantity quantity_temp = cos.Quantities.Where(a => a.Id == q.Id).First();
                quantity_temp.Cantitate++;
                db.SaveChanges();
            }
            else
            {
                //daca nu exista acest produs in cos las cantitatea 1 si il adaug direct in baza de date
                db.Quantities.Add(q);
                db.SaveChanges();
            }
            
            return Redirect("/Products/Index");
        }

        [HttpPut]
        public ActionResult EditPlus(int id)
        {
            Quantity q = db.Quantities.Find(id);
            q.Cantitate++;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPut]
        public ActionResult EditMinus(int id)
        {
            Quantity q = db.Quantities.Find(id);
            if (q.Cantitate == 1)
            {
                db.Quantities.Remove(q);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                q.Cantitate--;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Quantity q = db.Quantities.Find(id);
            db.Quantities.Remove(q);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        void CalculeazaTotal()
        {
            var userCurent = User.Identity.GetUserId();
            Cart cos = db.Carts.Where(a => a.UserId == userCurent).First();

            float sumaTotala = 0;


            foreach(var q in cos.Quantities)
            {
                sumaTotala = sumaTotala + q.Cantitate * q.Product.Pret;
            }

            ViewBag.Suma = sumaTotala;
        }

    }
}