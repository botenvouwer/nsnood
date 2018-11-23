using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using nsnood.model;
using nsnood.repo;

namespace nsnood.Controllers
{
    [Route("api/")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        
        private readonly NotificationRepo _notificationRepo;
        public NotificationController(NotificationRepo notificationRepo)
        {
            _notificationRepo = notificationRepo;
        }

        [Route("meldingen")]
        public IEnumerable<Notification> Index()
        {
            return this._notificationRepo.All();
        }

        [Route("meldingen/id/{id}", Order = int.MaxValue)]
        public ActionResult GetNotification(string id)
        {
            if (Guid.TryParse(id, out Guid guid))
            {
                try
                {
                    var notification = _notificationRepo.GetNotification(guid);
                    if (notification != null)
                    {
                        
                        return new JsonResult(notification);
                    }
                    else
                    {
                        return NotFound(new {});
                    }
                }
                catch (Exception e)
                {
                    return NotFound(new {});
                }
            }
            return this.BadRequest(new {});
        }

        [Route("meldingen/treinstel/{id}")]
        public ActionResult GetNotificationsByMaterial(int id)
        {
            if (id < 0)
            {
                return NotFound();
            }

            try
            {
                var notification = _notificationRepo.GetNotificationFromTrain(id);
                
                if (notification != null && notification.Any())
                {
                    return new JsonResult(notification);
                }
                else
                {
                    return NotFound(new List<object>() {});
                }
                
            }
            catch (Exception)
            {
                return NotFound(new List<object>() {});
            }
            return BadRequest();
        }

        [Route("notificaties/medewerker/{id}")]
        public ActionResult GetNotificationByWorker(int id)
        {
            if (id < 0)
            {
                return NotFound();
            }

            try
            {
                return new JsonResult(this._notificationRepo.GetNotificationFromTrain(id));
            }
            catch (Exception e)
            {
                return NotFound(new List<object>() {});
            }
            return BadRequest();
        }
        
        



        [HttpPost("komtmeldingaan")]
        public ActionResult<string> KomtMeldingAan([FromBody] Notification notification)
        {
            var id = _notificationRepo.RegisterNotification();

            return id.ToString();
        }

        [HttpPost("maakmelding")]
        public ActionResult<string> MaakMelding([FromBody] Notification notification)
        {
            try
            {
                _notificationRepo.UpdateNotification(notification);

                return Ok("Noodmelding succesvol afgehandeld");
            }
            catch(Exception)
            {
                return NotFound("Er is iets misgegaan met het maken van de noodmelding");
            }
        }
        
        
        
    }
}