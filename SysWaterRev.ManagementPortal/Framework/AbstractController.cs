﻿using AutoMapper;
using System.Web.Mvc;

namespace SimpleRevCollection.Management.Framework
{
    public abstract class AbstractController : Controller
    {
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return Mapper.Map(source, destination);
        }

        public virtual ViewResult ViewNotFound(string message)
        {
            ViewBag.Message = message;
            return View("~/Views/NotFound.cshtml");
        }
    }
}