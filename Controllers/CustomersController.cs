﻿using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using Vidly.Models;
using Vidly.ViewModels;

namespace Vidly.Controllers
{
    public class CustomersController : Controller
    {
        private ApplicationDbContext _context;

        public CustomersController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult New()
        {
            var memberShipTypes = _context.MemberShipTypes.ToList();

            var viewModel = new CustomerFormViewModel
            {
                Customer = new Customer(),
                MemberShipTypes = memberShipTypes
            };
            return View("CustomerForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Customer customer) 
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new CustomerFormViewModel {
                    Customer = customer,
                    MemberShipTypes = _context.MemberShipTypes.ToList()
                };
                
                return View("CustomerForm", viewModel);
            }

            if(customer.Id == 0)
                _context.Customers.Add(customer);
            else
            {
                var customerInBD = _context.Customers.Single(c => c.Id == customer.Id);
                customerInBD.Name = customer.Name;
                customerInBD.BirthDate = customer.BirthDate;
                customerInBD.MemberShipTypeId = customer.MemberShipTypeId;
                customerInBD.IsSubscribedToNewsLetter = customer.IsSubscribedToNewsLetter;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Customers");
        }

        //[Route("Customers/Index")]
        public ActionResult Index()
        {

            /*var customers = _context.Customers.Include(c => c.MemberShipType).ToList();         
            return View(customers);*/
            return View();
        }

        [Route("Customers/Details/{id}")]
        public ActionResult Details(int id) 
        {
            var customer = _context.Customers.Include(c => c.MemberShipType).SingleOrDefault(c => c.Id == id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        public ActionResult Edit(int id) {
            var customer = _context.Customers.SingleOrDefault(c => c.Id == id);
            
            if (customer == null)
                return HttpNotFound();

            var viewModel = new CustomerFormViewModel
            {
                Customer = customer,
                MemberShipTypes = _context.MemberShipTypes.ToList()
            };
            
            return View("CustomerForm", viewModel);
        }

    }
}