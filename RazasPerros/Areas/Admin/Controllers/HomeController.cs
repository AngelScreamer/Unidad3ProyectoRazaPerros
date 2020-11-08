using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RazasPerros.Models;
using RazasPerros.Models.ViewModels;
using RazasPerros.Repositories;

namespace RazasPerros.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IWebHostEnvironment Enviroment { get; set; }
        public HomeController(IWebHostEnvironment env)
        {
            Enviroment = env;
        }
        public IActionResult Index(string id)
        {
            RazasRepository razasRepository = new RazasRepository();
            IndexViewModel vm = new IndexViewModel();
            vm.Razas = razasRepository.GetRazas();
            return View(vm);
        }
        public IActionResult Agregar()
        {
            AdminRazaViewModel vm = new AdminRazaViewModel();
            RazasRepository razasRepository = new RazasRepository();
            vm.Paises = razasRepository.GetPaises();
            return View(vm);
        }
        [HttpPost]
        public IActionResult Agregar(AdminRazaViewModel vm)
        {
            RazasRepository repos = new RazasRepository();
            try
            {
                if (vm.Archivo == null)
                {
                    ModelState.AddModelError("", "Seleccione la imágen de la raza.");

                    vm.Paises = repos.GetPaises();
                    return View(vm);
                }
                else
                {
                    if (vm.Archivo.ContentType != "image/jpeg" || vm.Archivo.Length > 1024 * 1024 * 2)
                    {
                        ModelState.AddModelError("", "Debe seleccionar un archivo jpg de menos de 2MB.");
                        vm.Paises = repos.GetPaises();
                        return View(vm);
                    }
                }

                repos.Insert(vm.Raza);

                if (vm.Archivo != null)
                {
                    FileStream fs = new FileStream(Enviroment.WebRootPath + "/imgs_perros/" + vm.Raza.Id + "_0.jpg", FileMode.Create);
                    vm.Archivo.CopyTo(fs);
                    fs.Close();
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                vm.Paises = repos.GetPaises();
                return View(vm);
            }
        }
        
        public IActionResult Editar(uint id)
        {
            AdminRazaViewModel vm = new AdminRazaViewModel();
            RazasRepository razasRepository = new RazasRepository();
            vm.Paises = razasRepository.GetPaises();
            vm.Raza = razasRepository.GetRazaById(id);
            if (vm.Raza == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (System.IO.File.Exists(Enviroment.WebRootPath + $"/imgs_perros/{vm.Raza.Id}_0.jpg"))
            {
                vm.Imagen = vm.Raza.Id + "_0.jpg";
            }
            else
            {
                vm.Imagen = "NoPhoto.jpg";
            }

            return View(vm);
        }
        [HttpPost]
        
        public IActionResult Editar(AdminRazaViewModel vm)
        {
            RazasRepository razasRepository = new RazasRepository();

            if (vm.Archivo != null)
            {
                if (vm.Archivo.ContentType != "image/jpeg" || vm.Archivo.Length > 1024 * 1024 * 2)
                {
                    ModelState.AddModelError("", "Debe seleccionar un archivo jpg de menos de 2MB.");
                    vm.Paises = razasRepository.GetPaises();
                    return View(vm);
                }
            }

            try
            {
                var raza = razasRepository.GetRazaById(vm.Raza.Id);
                vm.Paises = razasRepository.GetPaises();
                if (raza != null)
                {                    
                    raza.Estadisticasraza.NivelEnergia = vm.Raza.Estadisticasraza.NivelEnergia;
                    raza.Estadisticasraza.FacilidadEntrenamiento = vm.Raza.Estadisticasraza.FacilidadEntrenamiento;
                    raza.Estadisticasraza.EjercicioObligatorio = vm.Raza.Estadisticasraza.EjercicioObligatorio;
                    raza.Estadisticasraza.AmistadDesconocidos = vm.Raza.Estadisticasraza.AmistadDesconocidos;
                    raza.Estadisticasraza.AmistadPerros = vm.Raza.Estadisticasraza.AmistadPerros;
                    raza.Estadisticasraza.NecesidadCepillado = vm.Raza.Estadisticasraza.NecesidadCepillado;

                    raza.Caracteristicasfisicas.Patas = vm.Raza.Caracteristicasfisicas.Patas;
                    raza.Caracteristicasfisicas.Cola = vm.Raza.Caracteristicasfisicas.Cola;
                    raza.Caracteristicasfisicas.Pelo = vm.Raza.Caracteristicasfisicas.Pelo;
                    raza.Caracteristicasfisicas.Hocico = vm.Raza.Caracteristicasfisicas.Hocico;
                    raza.Caracteristicasfisicas.Color = vm.Raza.Caracteristicasfisicas.Color;

                    raza.IdPais = vm.Raza.IdPais;
                    raza.PesoMin = vm.Raza.PesoMin;
                    raza.PesoMax = vm.Raza.PesoMax;
                    raza.AlturaMin = vm.Raza.AlturaMin;
                    raza.AlturaMax = vm.Raza.AlturaMax;
                    raza.EsperanzaVida = vm.Raza.EsperanzaVida;
                    raza.OtrosNombres = vm.Raza.OtrosNombres;
                    raza.Descripcion = vm.Raza.Descripcion;
                    razasRepository.Update(raza);

                    if (vm.Archivo != null)
                    {
                        FileStream fs = new FileStream(Enviroment.WebRootPath + "/imgs_perros/" + vm.Raza.Id + "_0.jpg", FileMode.Create);
                        vm.Archivo.CopyTo(fs);
                        fs.Close();
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                    return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                vm.Paises = razasRepository.GetPaises();
                return View(vm);
            }

        }

        public IActionResult Eliminar(uint id)
        {
            RazasRepository razasRepository = new RazasRepository();
            var raza = razasRepository.GetRazaById(id);
            return View(raza);
        }

        [HttpPost]
        public IActionResult Eliminar(Razas raza)
        {
            RazasRepository razasRepository = new RazasRepository();
            var razas = razasRepository.GetRazaById(raza.Id);
            if (razas != null)
            {
                razas.Eliminado = 1;
                razasRepository.Update(razas);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
